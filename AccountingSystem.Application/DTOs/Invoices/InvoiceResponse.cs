using AccountingSystem.Application.DTOs.Customers;

namespace AccountingSystem.Application.DTOs.Invoices
{
    public class InvoiceResponse
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }

        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }

        public CustomerResponse Customer { get; set; } = new();

        public List<InvoiceItemResponse> Items { get; set; } = new();
    }
}
