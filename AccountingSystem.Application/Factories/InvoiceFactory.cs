using AccountingSystem.Application.DTOs.Invoices;
using AccountingSystem.Application.Helpers;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;


namespace AccountingSystem.Application.Factories
{
    public class InvoiceFactory
    {
        private readonly INumberSequenceService _numberSequenceService;

        public InvoiceFactory(
            INumberSequenceService numberSequenceService)

        {
            _numberSequenceService = numberSequenceService;
        }

        public Invoice Create(
            CreateInvoiceRequest request,
            Customer customer,
            IDictionary<int, Product> products
            )
        {
            var invoice = new Invoice
            {
                CustomerId = request.CustomerId,
                Status = InvoiceStatus.Draft,
                DateCreated = DateTime.UtcNow,
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14),
                InvoiceNumber = _numberSequenceService.GetNext(DocumentType.Invoice)
            };

            invoice.ApplyCustomerSnapshot(customer);

            var domainItems = request.Items?
                .Select(x => new InvoiceItem
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    DiscountPercent = x.DiscountPercent
                })
                .ToList() ?? new List<InvoiceItem>();

            invoice.Items = ItemSnapshotHelper.SnapshotInvoiceItems(
                domainItems, 
                products);

            return invoice;
        }
    }
}
