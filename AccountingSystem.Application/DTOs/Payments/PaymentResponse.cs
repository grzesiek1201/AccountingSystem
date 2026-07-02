using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Payments
{
    public class PaymentResponse
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
        public int InvoiceId { get; set; }
    }
}