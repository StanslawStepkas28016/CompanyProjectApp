using System.Text.Json;

namespace CompanyProjectApp.Dtos;

public class ErrorDetailsDto
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string ExceptionMessage { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}