namespace CompanyProjectApp.Entities.ProductEntities;

public class ProductDiscount
{
    public int IdProduct { get; set; }

    public Product Product { get; set; }
    
    public int IdDiscount { get; set; }

    public Discount Discount { get; set; }
}