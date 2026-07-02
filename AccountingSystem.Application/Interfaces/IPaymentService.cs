using AccountingSystem.Application.DTOs.Invoices;
using AccountingSystem.Application.DTOs.Payments;

namespace AccountingSystem.Application.Interfaces
{
    public interface IPaymentService
    {
        PaymentAddResponse AddPayment(CreatePaymentRequest request);

        IEnumerable<PaymentResponse> GetPaymentsForInvoice(int invoiceId);

        PaymentDeleteResult DeletePayment(int paymentId);
    }
}