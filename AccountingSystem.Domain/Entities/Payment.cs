using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Domain.Entities
{
    public class Payment
    {
        public Payment()
        {
            Status = PaymentStatus.Pending;
        }
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public Invoice Invoice { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public PaymentMethod Method { get; set; }

        public PaymentStatus Status { get; private set; }


        public void Complete()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException();

            Status = PaymentStatus.Paid;
        }


        public void Cancel()
        {
            if (Status == PaymentStatus.Paid)
                throw new InvalidOperationException(
                    "Paid payments cannot be cancelled.");

            Status = PaymentStatus.Cancelled;
        }
    }
}