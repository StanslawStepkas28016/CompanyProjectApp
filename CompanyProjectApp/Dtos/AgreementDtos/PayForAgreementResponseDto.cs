namespace CompanyProjectApp.Dtos.AgreementDtos;

public class PayForAgreementResponseDto
{
    public int IdPayment { get; set; }

    public int IdAgreement { get; set; }

    public decimal MoneyOwedFull { get; set; }

    public decimal MoneyPaid { get; set; }
}