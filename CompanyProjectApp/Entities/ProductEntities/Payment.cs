namespace CompanyProjectApp.Entities.ProductEntities;

public class Payment
{
    public int IdPayment { get; set; }

    public int IdAgreement { get; set; }

    public virtual Agreement Agreement { get; set; }
    
    public decimal MoneyOwedFull { get; set; }

    public decimal MoneyPaid { get; set; }
}