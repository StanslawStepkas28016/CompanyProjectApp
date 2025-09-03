namespace CompanyProjectApp.Entities.ProductEntities;

public class Agreement
{
    public int IdAgreement { get; set; }

    public int IdClient { get; set; }

    public string ClientType { get; set; }

    public int IdProduct { get; set; }

    public string? ProductVersionInfo { get; set; }

    public virtual Product Product { get; set; }

    public DateTime AgreementDateFrom { get; set; }

    public DateTime AgreementDateTo { get; set; }

    public decimal CalculatedPrice { get; set; }

    public int ProductUpdatesToInYears { get; set; } = 1;

    public bool IsSigned { get; set; }

    public virtual ICollection<Payment> Payments { get; set; }
}