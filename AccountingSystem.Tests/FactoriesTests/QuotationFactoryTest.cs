using AccountingSystem.Application.DTOs.Quotations;
using AccountingSystem.Application.Factories;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Moq;
using Xunit;

namespace AccountingSystem.Tests.FactoriesTests
{
    public class QuotationFactoryTests
    {
        private readonly Mock<INumberSequenceService> _seqMock;
        private readonly QuotationFactory _factory;

        public QuotationFactoryTests()
        {
            _seqMock = new Mock<INumberSequenceService>();

            _seqMock.Setup(x => x.GetNext(DocumentType.Quotation))
                .Returns("Q-2026-0001");

            _factory = new QuotationFactory(_seqMock.Object);
        }


        private Customer CreateCustomer()
        {
            return new Customer
            {
                Id = 1,
                Name = "Test Customer",
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
                        Name = "Laptop",
                        Price = 1000m
                    }
                }
            };
        }


        private CreateQuotationRequest CreateRequest()
        {
            return new CreateQuotationRequest
            {
                CustomerId = 1,
                Items = new List<CreateQuotationItemRequest>
                {
                    new CreateQuotationItemRequest
                    {
                        ProductId = 1,
                        Quantity = 2,
                        DiscountPercent = 10
                    }
                }
            };
        }


        [Fact]
        public void Create_ShouldCreateQuotationWithCorrectData()
        {
            var result = _factory.Create(
                CreateRequest(),
                CreateCustomer(),
                CreateProducts());


            Assert.NotNull(result);

            Assert.Equal(1, result.CustomerId);
            Assert.Equal("Q-2026-0001", result.QuotationNumber);
            Assert.Equal(QuotationStatus.Draft, result.Status);

            Assert.NotNull(result.DateCreated);

            Assert.Single(result.Items);
        }


        [Fact]
        public void Create_ShouldApplyCustomerSnapshot()
        {
            var result = _factory.Create(
                CreateRequest(),
                CreateCustomer(),
                CreateProducts());


            Assert.Equal("Test Customer", result.CustomerName);
            Assert.Equal("test@test.com", result.CustomerEmail);
            Assert.Equal("Street", result.CustomerStreet);
            Assert.Equal("00-000", result.CustomerZipCode);
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


            Assert.Equal("Laptop", item.ProductName);
            Assert.Equal(1000m, item.BaseUnitPrice);
            Assert.Equal(2, item.Quantity);
            Assert.Equal(1800m, item.Total);
        }
    }
}