namespace AccountingSystem.Domain.Enums
{

    public enum InvoiceAddResult
    {
        Success,
        InvalidData
    }

    public enum InvoiceEditResult
    {
        Success,
        NotFound,
        InvalidData,
        InvoiceArchived
    }

    public enum ArchiveInvoiceResult
    {
        Success,
        NotFound,
        InvalidOperation
    }

    public enum ValidateInvoiceResult
    {
        IsValid,
        NotValid
    }

    public enum PaymentAddResult
    {
        Success,
        InvoiceNotFound,
        InvoiceArchived,
        InvalidAmount,
        AmountExceedsRemaining
    }

    public enum InvoiceOperationResult
    {
        Success,
        NotFound,
        InvalidOperation,
    }
}
