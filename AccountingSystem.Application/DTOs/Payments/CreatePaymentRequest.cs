namespace AccountingSystem.Application.DTOs.Payments
{
    public class CreatePaymentRequest
    {
       public int InvoiceId { get; set; }
       public decimal Amount { get; set; }

    }
}
