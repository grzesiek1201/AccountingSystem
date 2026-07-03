using AccountingSystem.Application.DTOs.Invoices;
using AccountingSystem.Application.DTOs.Orders;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.Interfaces
{
    public interface IInvoiceService
    {
        InvoiceAddResponse AddInvoice(CreateInvoiceRequest request);
        InvoiceEditResponse EditInvoice(UpdateInvoiceRequest request);
        List<InvoiceResponse> GetAllInvoices();
        InvoiceResponse? FindInvoice(int id);
        InvoiceStatusResponse ChangeInvoiceStatus(int id, StatusInvoiceRequest request);
        ArchiveInvoiceResult ArchiveInvoice(int id);
    }
}
