using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Invoices
{
    public class InvoiceStatusResponse
    {
        public InvoiceOperationResult Result { get; set; }

        public List<InvoiceValidationError> Errors { get; set; } = new List<InvoiceValidationError>();

        public bool IsSuccess => Result == InvoiceOperationResult.Success;
    }
}
