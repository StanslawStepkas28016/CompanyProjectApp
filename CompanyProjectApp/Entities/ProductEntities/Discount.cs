namespace CompanyProjectApp.Entities.ProductEntities;

public class Discount
{
    public int IdDiscount { get; set; }

    public string Name { get; set; }

    public int Amount { get; set; }

    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public ICollection<ProductDiscount> ProductDiscounts { get; set; }
}