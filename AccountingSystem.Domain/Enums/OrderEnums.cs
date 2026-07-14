namespace AccountingSystem.Domain.Enums
{

    public enum OrderAddResult
    {
        Success,
        InvalidData
    }

    public enum OrderEditResult
    {
        Success,
        NotFound,
        InvalidData,
        OrderArchived
    }

    public enum ArchiveOrderResult
    {
        Success,
        NotFound,
        InvalidOperation
    }

    public enum ConvertOrderResult
    {
        Success,
        NotFound,
        InvalidData
    }

    public enum ValidateOrderResult
    {
        IsValid,
        NotValid
    }

    public enum OrderStatusResult
    {
        Success,
        NotFound,
        InvalidOperation
    }
}
