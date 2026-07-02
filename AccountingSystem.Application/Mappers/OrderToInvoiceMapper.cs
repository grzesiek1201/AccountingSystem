using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Mappers
{
    public class OrderToInvoiceMapper
    {
        public Invoice Map(Order order)
        {
            if (order == null)
                return null;

            var invoice = new Invoice
            {
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                CustomerStreet = order.CustomerStreet,
                CustomerZipCode = order.CustomerZipCode,
                
                OrderId = order.Id,

                Items = order.Items.Select(q => new InvoiceItem
                {
                    ProductId = q.ProductId,
                    Quantity = q.Quantity,
                    BaseUnitPrice = q.BaseUnitPrice,
                    DiscountPercent = q.DiscountPercent,
                    Position = q.Position
                }).ToList()
            };

            return invoice;
        }
    }
}