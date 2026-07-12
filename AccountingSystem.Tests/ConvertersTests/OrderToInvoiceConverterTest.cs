using AccountingSystem.Application.Converters;
using AccountingSystem.Domain.Entities;
using Xunit;

namespace AccountingSystem.Tests.ConvertersTests
{
    public class OrderToInvoiceConverterTests
    {
        private readonly OrderToInvoiceConverter _converter;

        public OrderToInvoiceConverterTests()
        {
            _converter = new OrderToInvoiceConverter();
        }

        private Order CreateOrder()
        {
            return new Order
            {
                Id = 1,
                CustomerId = 10,
                CustomerName = "Test",
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 5,
                        Quantity = 2,
                        BaseUnitPrice = 100,
                        DiscountPercent = 10,
                        Position = 1
                    }
                }
            };
        }

        [Fact]
        public void Map_ShouldConvertOrderToInvoice()
        {
            var order = CreateOrder();

            var result = _converter.Map(order);

            Assert.NotNull(result);
            Assert.Equal(order.Id, result.OrderId);
            Assert.Equal(order.CustomerId, result.CustomerId);
            Assert.Equal(order.CustomerName, result.CustomerName);
        }

        [Fact]
        public void Map_ShouldReturnNull_WhenOrderIsNull()
        {
            Order order = null;

            var result = _converter.Map(order);

            Assert.Null(result);
        }

        [Fact]
        public void Map_ShouldMapOrderItemsToInvoiceItems()
        {
            var order = CreateOrder();

            var result = _converter.Map(order);

            Assert.NotNull(result.Items);
            Assert.Single(result.Items);

            var invoiceItem = result.Items.First();
            var orderItem = order.Items.First();

            Assert.Equal(orderItem.ProductId, invoiceItem.ProductId);
            Assert.Equal(orderItem.Quantity, invoiceItem.Quantity);
            Assert.Equal(orderItem.BaseUnitPrice, invoiceItem.BaseUnitPrice);
            Assert.Equal(orderItem.DiscountPercent, invoiceItem.DiscountPercent);
            Assert.Equal(orderItem.Position, invoiceItem.Position);
        }
    }
}