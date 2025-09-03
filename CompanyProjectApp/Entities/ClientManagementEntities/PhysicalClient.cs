namespace CompanyProjectApp.Entities.ClientManagementEntities;

public class PhysicalClient
{
    public int IdPhysicalClient { get; set; }
    
    public string? Pesel { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Email { get; set; }

    public int PhoneNumber { get; set; }

    public bool IsDeleted { get; set; }
}