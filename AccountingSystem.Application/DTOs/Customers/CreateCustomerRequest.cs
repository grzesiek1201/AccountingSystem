namespace AccountingSystem.Application.DTOs.Customers;

public class CreateCustomerRequest
{
    public string Name { get; set; } = string.Empty;
    public string NIP { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}