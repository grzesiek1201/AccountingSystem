using AccountingSystem.Application.DTOs.Quotations;
using AccountingSystem.Application.Helpers;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.Factories
{
    public class QuotationFactory
    {
        private readonly INumberSequenceService _numberSequenceService;

        public QuotationFactory(
            INumberSequenceService numberSequenceService)
        {
            _numberSequenceService = numberSequenceService;
        }

        public Quotation Create(
            CreateQuotationRequest request,
            Customer customer,
            IDictionary<int, Product> products)
        {
            var quotation = new Quotation
            {
                CustomerId = request.CustomerId,
                DateCreated = DateTime.UtcNow,
                QuotationNumber = _numberSequenceService.GetNext(DocumentType.Quotation)
            };

            quotation.ApplyCustomerSnapshot(customer);

            var domainItems = request.Items?
                .Select(x => new QuotationItem
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    DiscountPercent = x.DiscountPercent
                })
                .ToList() ?? new List<QuotationItem>();

            quotation.Items = ItemSnapshotHelper.SnapshotQuotationItems(
                domainItems,
                products);

            return quotation;
        
        }
    }
}