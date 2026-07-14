using AccountingSystem.Application.DTOs.Customers;

namespace AccountingSystem.Application.DTOs.Quotations
{
    public class QuotationResponse
    {
        public int Id { get; set; }
        public string QuotationNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }

        public CustomerResponse Customer { get; set; } = new();

        public List<QuotationItemResponse> Items { get; set; } = new();
    }
}
