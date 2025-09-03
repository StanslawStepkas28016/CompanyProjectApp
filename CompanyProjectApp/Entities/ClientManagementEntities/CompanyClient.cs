namespace CompanyProjectApp.Entities.ClientManagementEntities;

public class CompanyClient
{
    public int IdCompanyClient { get; set; }
    
    public string? KrsNumber { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public int PhoneNumber { get; set; }
}