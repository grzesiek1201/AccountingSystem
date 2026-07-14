using AccountingSystem.Application.DTOs.Orders;
using AccountingSystem.Application.Helpers;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.Factories
{
    public class OrderFactory
    {
        private readonly INumberSequenceService _numberSequenceService;

        public OrderFactory(
            INumberSequenceService numberSequenceService)
        {
            _numberSequenceService = numberSequenceService;
        }

        public Order Create(
            CreateOrderRequest request,
            Customer customer,
            IDictionary<int, Product> products)
        {
            var order = new Order
            {
                CustomerId = request.CustomerId,
                DateCreated = DateTime.UtcNow,
                OrderNumber = _numberSequenceService.GetNext(DocumentType.Order)
            };

            order.ApplyCustomerSnapshot(customer);

            var domainItems = request.Items?
                .Select(x => new OrderItem
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    DiscountPercent = x.DiscountPercent
                })
                .ToList() ?? new List<OrderItem>();

            order.Items = ItemSnapshotHelper.SnapshotOrderItems(
                domainItems,
                products);

            return order;
        }     
    }
}
