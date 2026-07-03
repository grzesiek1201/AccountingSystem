using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Invoices
{
    public class StatusInvoiceRequest
    {
        public InvoiceStatus Status { get; set; }
    }
}
