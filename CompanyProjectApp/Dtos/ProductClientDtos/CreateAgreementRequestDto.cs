namespace CompanyProjectApp.Dtos.ProductClientDtos;

public class CreateAgreementRequestDto
{
    public int IdClient { get; set; }

    public string ClientType { get; set; }

    public int IdProduct { get; set; }

    public DateTime AgreementDateFrom { get; set; }

    public DateTime AgreementDateTo { get; set; }

    public int ProductUpdatesToInYears { get; set; }
}