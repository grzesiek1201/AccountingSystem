namespace AccountingSystem.Application.DTOs.Quotations
{
    public class CreateQuotationRequest
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string QuotationNumber { get; set; } = string.Empty;
        public List<CreateQuotationItemRequest> Items { get; set; } = new();
    }
}
