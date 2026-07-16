using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Xunit;

namespace AccountingSystem.Tests.DomainTests
{
    public class OrderTests
    {
        [Fact]
        public void NewOrder_ShouldHaveDraftStatus()
        {
            var order = new Order();

            Assert.Equal(
                OrderStatus.Draft,
                order.Status);
        }


        [Fact]
        public void Confirm_DraftOrder_ShouldChangeStatusToConfirmed()
        {
            var order = new Order();

            order.Confirm();

            Assert.Equal(
                OrderStatus.Confirmed,
                order.Status);
        }


        [Fact]
        public void Complete_ConfirmedOrder_ShouldChangeStatusToCompleted()
        {
            var order = CreateOrderWithItem();

            order.Confirm();

            order.Complete();

            Assert.Equal(
                OrderStatus.Completed,
                order.Status);
        }


        [Fact]
        public void Complete_DraftOrder_ShouldThrowException()
        {
            var order = new Order();

            Assert.Throws<InvalidOperationException>(
                () => order.Complete());
        }


        [Fact]
        public void Cancel_CompletedOrder_ShouldThrowException()
        {
            var order = CreateOrderWithItem();

            order.Confirm();
            order.Complete();

            Assert.Throws<InvalidOperationException>(
                () => order.Cancel());
        }


        private static Order CreateOrderWithItem()
        {
            var order = new Order();

            order.Items.Add(new OrderItem
            {
                ProductId = 1,
                ProductName = "Test product",
                ProductCode = "TEST-001",
                Quantity = 1,
                BaseUnitPrice = 100m,
                DiscountPercent = 0m
            });

            return order;
        }
    }
}