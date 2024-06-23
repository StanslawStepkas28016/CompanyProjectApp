namespace CompanyProjectApp.Entities.ProductEntities;

public class Product
{
    public int IdProduct { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string VersionInfo { get; set; }

    public string Category { get; set; }

    public decimal Price { get; set; }

    public ICollection<ProductDiscount> ProductDiscounts { get; set; }
    
    public virtual ICollection<Agreement> Agreements { get; set; }
}