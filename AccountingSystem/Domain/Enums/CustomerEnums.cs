namespace AccountingSystem.Domain.Enums
{
    public enum CustomerEditResult
    {
        Success,
        NotFound,
        CustomerArchived
    }

    public enum ArchiveCustomerResult
    {
        Success,
        NotFound,
        CustomerInDebt
    } 

    public enum ValidateResult
    {
        bool IsValid,
        
    }

}