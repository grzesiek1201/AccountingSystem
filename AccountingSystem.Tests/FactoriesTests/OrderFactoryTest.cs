using AccountingSystem.Application.DTOs.Orders;
using AccountingSystem.Application.Factories;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Moq;
using Xunit;

namespace AccountingSystem.Tests.FactoriesTests
{
    public class OrderFactoryTests
    {
        private readonly Mock<INumberSequenceService> _seqMock;
        private readonly OrderFactory _factory;


        public OrderFactoryTests()
        {
            _seqMock = new Mock<INumberSequenceService>();

            _seqMock.Setup(x => x.GetNext(DocumentType.Order))
                .Returns("O-2026-0001");

            _factory = new OrderFactory(_seqMock.Object);
        }


        private Customer CreateCustomer()
        {
            return new Customer
            {
                Id = 1,
                Name = "Customer",
                Email = "test@test.com",
                Street = "Street",
                ZipCode = "00-000",
                City = "City"
            };
        }


        private Dictionary<int, Product> CreateProducts()
        {
            return new Dictionary<int, Product>
            {
                {
                    1,
                    new Product
                    {
                        Id = 1,
                        Name = "Product",
                        Price = 500m
                    }
                }
            };
        }


        private CreateOrderRequest CreateRequest()
        {
            return new CreateOrderRequest
            {
                CustomerId = 1,
                Items = new List<CreateOrderItemRequest>
                {
                    new CreateOrderItemRequest
                    {
                        ProductId = 1,
                        Quantity = 2,
                        DiscountPercent = 20
                    }
                }
            };
        }


        [Fact]
        public void Create_ShouldCreateOrderWithCorrectData()
        {
            var result = _factory.Create(
                CreateRequest(),
                CreateCustomer(),
                CreateProducts());


            Assert.Equal(1, result.CustomerId);
            Assert.Equal("O-2026-0001", result.OrderNumber);
            Assert.Equal(OrderStatus.Draft, result.Status);
            Assert.NotNull(result.DateCreated);
        }


        [Fact]
        public void Create_ShouldApplyCustomerSnapshot()
        {
            var result = _factory.Create(
                CreateRequest(),
                CreateCustomer(),
                CreateProducts());


            Assert.Equal("Customer", result.CustomerName);
            Assert.Equal("test@test.com", result.CustomerEmail);
            Assert.Equal("City", result.CustomerCity);
        }


        [Fact]
        public void Create_ShouldCreateItemSnapshot()
        {
            var result = _factory.Create(
                CreateRequest(),
                CreateCustomer(),
                CreateProducts());


            var item = result.Items.First();

            Assert.Equal("Product", item.ProductName);
            Assert.Equal(500m, item.BaseUnitPrice);
            Assert.Equal(800m, item.Total);
        }
    }
}