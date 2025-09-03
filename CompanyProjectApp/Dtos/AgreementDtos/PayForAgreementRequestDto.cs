namespace CompanyProjectApp.Dtos.AgreementDtos;

public class PayForAgreementRequestDto
{
    public int IdAgreement { get; set; }

    public int IdClient { get; set; }

    public string ClientType { get; set; }

    public int Amount { get; set; }
}