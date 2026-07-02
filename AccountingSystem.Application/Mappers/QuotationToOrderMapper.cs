using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Mappers
{
    public class QuotationToOrderMapper
    {
        public Order Map(Quotation quotation)
        {
            if (quotation == null)
                return null;

            return new Order
            {
                CustomerId = quotation.CustomerId,
                CustomerName = quotation.CustomerName,
                CustomerStreet = quotation.CustomerStreet,
                CustomerZipCode = quotation.CustomerZipCode,

                QuotationId = quotation.Id,

                Items = quotation.Items.Select(q => new OrderItem
                {
                    ProductId = q.ProductId,
                    Quantity = q.Quantity,
                    BaseUnitPrice = q.BaseUnitPrice,
                    DiscountPercent = q.DiscountPercent,
                    Position = q.Position
                }).ToList()
            };
        }
    }
}