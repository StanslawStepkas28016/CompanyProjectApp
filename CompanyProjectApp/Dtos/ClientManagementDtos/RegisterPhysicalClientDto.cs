namespace CompanyProjectApp.Dtos.ClientManagementDtos;

public class RegisterPhysicalClientDto
{
    public string? Pesel { get; set; }

    public string? Email { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public int PhoneNumber { get; set; }
}