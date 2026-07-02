using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Interfaces
{
    public interface IInvoiceStatusCalculator
    {
        void Recalculate(Invoice invoice, decimal totalPaid);
    }
}
