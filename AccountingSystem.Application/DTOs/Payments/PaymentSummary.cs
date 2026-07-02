namespace AccountingSystem.Application.DTOs.Payments
{
    public class PaymentSummary
    {
        public decimal TotalPaid { get; set; }
        public decimal Remaining { get; set; }
        public int InvoiceId { get; set; }
    }
}
