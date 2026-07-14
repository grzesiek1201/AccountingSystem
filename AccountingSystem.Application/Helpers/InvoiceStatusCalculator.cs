using AccountingSystem.Application.Interfaces;
using AccountingSystem.Domain.Entities;

public class InvoiceStatusCalculator : IInvoiceStatusCalculator
{
    public void Recalculate(Invoice invoice, decimal totalPaid)
    {
        var now = DateTime.UtcNow;

        var isFullyPaid =
            totalPaid >= invoice.TotalAmount;


        if (invoice.DueDate < now && !isFullyPaid)
        {
            invoice.MarkAsOverdue();
        }
    }
}