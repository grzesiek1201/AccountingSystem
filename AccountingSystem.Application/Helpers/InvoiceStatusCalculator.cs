using AccountingSystem.Application.Interfaces;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.Helpers
{
    public class InvoiceStatusCalculator : IInvoiceStatusCalculator
    {
        public void Recalculate(Invoice invoice, decimal totalPaid)
        {
            var now = DateTime.UtcNow;
            var isFullyPaid = totalPaid >= invoice.TotalAmount;

            if (invoice.DueDate < now && !isFullyPaid)
            {
                invoice.Status = InvoiceStatus.Overdue;
                return;
            }

            invoice.Status = InvoiceStatus.Issued;
        }
    }
}