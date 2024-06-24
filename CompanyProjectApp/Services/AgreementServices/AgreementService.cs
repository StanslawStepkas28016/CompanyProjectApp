using CompanyProjectApp.Context;
using CompanyProjectApp.Dtos.AgreementDtos;
using CompanyProjectApp.Dtos.ProductClientDtos;
using CompanyProjectApp.Entities.ProductEntities;
using Microsoft.EntityFrameworkCore;

namespace CompanyProjectApp.Services.AgreementServices;

public class AgreementService : IAgreementService
{
    private readonly CompanyProjectAppContext _context;

    public AgreementService(CompanyProjectAppContext context)
    {
        _context = context;
    }

    public async Task<CreateAgreementResponseDto> CreateAgreement(CreateAgreementRequestDto request,
        CancellationToken cancellationToken)
    {
        if (AreAgreementDatesWithinTheLimit(request.AgreementDateFrom, request.AgreementDateTo) == false)
        {
            throw new ArgumentException("Agreement date has to be between 3 and 30 days!");
        }

        if (IsClientTypeInRightFormat(request.ClientType) == false)
        {
            throw new ArgumentException("ClientType has to be either \"company\" or \"physical\"!");
        }

        if (IsProductUpdatesToInYearsInRightFormat(request.ProductUpdatesToInYears) == false)
        {
            throw new ArgumentException("ProductUpdatesToInYears can be between " + MinYears + " and " + MaxYears +
                                        " years!");
        }

        if (await DoesClientExist(request.IdClient, request.ClientType, cancellationToken) ==
            false)
        {
            throw new ArgumentException("Client with the provided IdClient does not exist!");
        }

        if (await DoesProductExist(request.IdProduct, cancellationToken) == false)
        {
            throw new ArgumentException("Product with the provided IdProduct does not exist!");
        }

        if (await DoesClientAlreadyHaveAnUnsignedAgreementForTheProduct(request.IdClient, request.IdProduct,
                cancellationToken))
        {
            throw new ArgumentException("Provided client already has a yet unsigned agreement for the specified product!");
        }

        decimal calculatedPrice;

        if (await DoesProductHaveAnyAssociatedDiscountNow(request.IdProduct, cancellationToken))
        {
            // Wyszukanie ceny produktu, razem z listą zniżek.
            var res = await _context
                .Products
                .Include(p => p.ProductDiscounts)
                .ThenInclude(pd => pd.Discount)
                .Where(p => p.IdProduct == request.IdProduct)
                .Select(p => new
                {
                    PriceWithoutDiscounts = p.Price,
                    Discounts = p.ProductDiscounts.Select(pd => pd.Discount.Amount).ToList()
                })
                .SingleOrDefaultAsync(cancellationToken);

            // Zaaplikowanie najwyższej zniżki.
            calculatedPrice = res!.PriceWithoutDiscounts * (1 - (decimal)res.Discounts.Max() / 100);
        }
        else
        {
            // Wyszukanie ceny produktu, jeżeli nie ma żadnej zniżki z nim związanej.
            var res = await _context
                .Products
                .Where(p => p.IdProduct == request.IdProduct)
                .Select(p => p.Price)
                .FirstOrDefaultAsync(cancellationToken);

            calculatedPrice = res;
        }

        // Zaaplikowanie zniżki dla stałego klienta.
        if (await IsClientBuyingRegularly(request.IdClient, cancellationToken))
        {
            calculatedPrice *= (decimal)0.95;
        }

        var agreement = new Agreement
        {
            IdClient = request.IdClient,
            ClientType = request.ClientType,
            IdProduct = request.IdProduct,
            ProductVersionInfo = await _context.Products.Where(p => p.IdProduct == request.IdProduct)
                .Select(p => p.VersionInfo).FirstOrDefaultAsync(cancellationToken),
            AgreementDateFrom = request.AgreementDateFrom,
            AgreementDateTo = request.AgreementDateTo,
            CalculatedPrice = calculatedPrice + (1000 * request.ProductUpdatesToInYears),
            ProductUpdatesToInYears = request.ProductUpdatesToInYears,
            IsSigned = false
        };

        await _context
            .Agreements
            .AddAsync(agreement, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new CreateAgreementResponseDto
        {
            IdAgreement = agreement.IdAgreement,
            IdClient = agreement.IdClient,
            ClientType = agreement.ClientType,
            IdProduct = agreement.IdProduct,
            ProductVersionInfo = agreement.ProductVersionInfo!,
            AgreementDateFrom = agreement.AgreementDateFrom,
            AgreementDateTo = agreement.AgreementDateTo,
            CalculatedPrice = agreement.CalculatedPrice,
            ProductUpdatesToInYears = agreement.ProductUpdatesToInYears,
            IsSigned = agreement.IsSigned,
        };
    }

    public async Task<PayForAgreementResponseDto> PayForAgreement(PayForAgreementRequestDto request,
        CancellationToken cancellationToken)
    {
        Agreement agreement;
        Payment payment;

        if ((agreement = await GetAgreement(request, cancellationToken)) == null)
        {
            throw new ArgumentException(
                "Agreement with the provided IdAgreement, IdClient and ClientType does not exist!");
        }

        if (agreement.IsSigned)
        {
            throw new ArgumentException("The provided agreement has been already paid for (it is already signed)!");
        }

        if (request.Amount > agreement.CalculatedPrice)
        {
            throw new ArgumentException(
                "You cannot overpay for your agreement (you are trying to pay more than full price)!");
        }

        if (DateTime.Now < agreement.AgreementDateFrom)
        {
            throw new Exception("The agreement has not begun, you cannot pay for it!");
        }

        if ((payment = await GetPayment(request.IdAgreement, cancellationToken)) == null)
        {
            payment = new Payment
            {
                IdAgreement = agreement.IdAgreement,
                MoneyOwedFull = agreement.CalculatedPrice,
                MoneyPaid = 0,
            };

            await _context
                .Payments
                .AddAsync(payment, cancellationToken);
        }

        if (DateTime.Now > agreement.AgreementDateTo)
        {
            _context
                .Payments
                .Remove(payment);

            _context
                .Agreements
                .Remove(agreement);

            await _context.SaveChangesAsync(cancellationToken);

            throw new Exception(
                "You are late with your payment! The money you have paid, will be returned and the agreement will be dismissed!");
        }

        if (request.Amount + payment.MoneyPaid > payment.MoneyOwedFull)
        {
            throw new ArgumentException("You cannot overpay for your agreement (while paying in fractions)!");
        }

        if (payment.MoneyPaid < payment.MoneyOwedFull)
        {
            payment.MoneyPaid += request.Amount;
        }

        if (payment.MoneyPaid == payment.MoneyOwedFull)
        {
            agreement.IsSigned = true;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new PayForAgreementResponseDto
        {
            IdAgreement = payment.IdAgreement,
            IdPayment = payment.IdPayment,
            MoneyOwedFull = payment.MoneyOwedFull,
            MoneyPaid = payment.MoneyPaid
        };
    }

    private const int MinDays = 3;

    private const int MaxDays = 30;

    private bool AreAgreementDatesWithinTheLimit(DateTime from, DateTime to)
    {
        var days = (to - from).Days;
        return days is <= MaxDays and >= MinDays;
    }

    private static readonly List<string> ClientTypes = ["physical", "company"];

    private bool IsClientTypeInRightFormat(string clientType)
    {
        return ClientTypes.Contains(clientType);
    }

    private async Task<bool> DoesClientExist(int idClient, string clientType, CancellationToken cancellationToken)
    {
        if (clientType == ClientTypes[0])
        {
            var res = await _context
                .PhysicalClients
                .Where(pc => pc.IdPhysicalClient == idClient)
                .CountAsync(cancellationToken);

            return res == 1;
        }

        if (clientType == ClientTypes[1])
        {
            var res = await _context
                .CompanyClients
                .Where(cc => cc.IdCompanyClient == idClient)
                .CountAsync(cancellationToken);

            return res == 1;
        }

        // W razie modyfikacji (dodania nowego typu klienta) wystarczy dodać element
        // do listy "ClientTypes", dodając dodatkową kolejną instrukcję if.

        return false;
    }

    private const int MinYears = 1;

    private const int MaxYears = 3;

    private bool IsProductUpdatesToInYearsInRightFormat(int years)
    {
        return years is >= MinYears and <= MaxYears;
    }

    private async Task<bool> DoesProductExist(int idProduct, CancellationToken cancellationToken)
    {
        var res = await _context
            .Products
            .Where(p => p.IdProduct == idProduct)
            .CountAsync(cancellationToken);

        return res == 1;
    }

    private async Task<bool> DoesClientAlreadyHaveAnUnsignedAgreementForTheProduct(int idClient,
        int idProduct, CancellationToken cancellationToken)
    {
        var res = await _context
            .Agreements
            .Where(a => a.IdProduct == idProduct && a.IdClient == idClient && a.IsSigned == false)
            .CountAsync(cancellationToken);

        return res == 1;
    }


    private async Task<bool> IsClientBuyingRegularly(int idClient, CancellationToken cancellationToken)
    {
        var res = await _context
            .Agreements
            .Where(a => a.IdClient == idClient)
            .CountAsync(cancellationToken);

        return res >= 1;
    }

    private async Task<bool> DoesProductHaveAnyAssociatedDiscountNow(int idProduct, CancellationToken cancellationToken)
    {
        var res = await _context
            .Products
            .Include(p => p.ProductDiscounts)
            .Where(p =>
                p.IdProduct == idProduct
            )
            .AnyAsync(cancellationToken);

        return res;
    }

    private async Task<Agreement> GetAgreement(PayForAgreementRequestDto request,
        CancellationToken cancellationToken)
    {
        var agreement = await _context
            .Agreements
            .Where(
                a =>
                    a.IdAgreement == request.IdAgreement &&
                    a.IdClient == request.IdClient &&
                    a.ClientType == request.ClientType
            )
            .FirstOrDefaultAsync(cancellationToken);

        return agreement!;
    }

    private async Task<Payment> GetPayment(int idAgreement, CancellationToken cancellationToken)
    {
        var payment = await _context
            .Payments
            .Where(p => p.IdAgreement == idAgreement)
            .FirstOrDefaultAsync(cancellationToken);

        return payment!;
    }
}