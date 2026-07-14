using AccountingSystem.Application.DTOs.Invoices;
using AccountingSystem.Application.Factories;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Moq;
using Xunit;

namespace AccountingSystem.Tests.FactoriesTests
{
    public class InvoiceFactoryTests
    {
        private readonly Mock<INumberSequenceService> _seqMock;
        private readonly InvoiceFactory _factory;


        public InvoiceFactoryTests()
        {
            _seqMock = new Mock<INumberSequenceService>();

            _seqMock.Setup(x => x.GetNext(DocumentType.Invoice))
                .Returns("I-2026-0001");

            _factory = new InvoiceFactory(_seqMock.Object);
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
                        Price = 200m
                    }
                }
            };
        }


        private CreateInvoiceRequest CreateRequest()
        {
            return new CreateInvoiceRequest
            {
                CustomerId = 1,
                Items = new List<CreateInvoiceItemRequest>
                {
                    new CreateInvoiceItemRequest
                    {
                        ProductId = 1,
                        Quantity = 3,
                        DiscountPercent = 10
                    }
                }
            };
        }


        [Fact]
        public void Create_ShouldCreateInvoiceWithCorrectData()
        {
            var result = _factory.Create(
                CreateRequest(),
                CreateCustomer(),
                CreateProducts());


            Assert.Equal(1, result.CustomerId);
            Assert.Equal("I-2026-0001", result.InvoiceNumber);
            Assert.Equal(InvoiceStatus.Draft, result.Status);

            Assert.NotEqual(default,result.DateCreated);
            Assert.NotEqual(default,result.IssueDate);
            Assert.NotEqual(default,result.DueDate);
        }


        [Fact]
        public void Create_ShouldSetDueDate14DaysAfterIssueDate()
        {
            var result = _factory.Create(
                CreateRequest(),
                CreateCustomer(),
                CreateProducts());


            var difference = result.DueDate.Date -
                             result.IssueDate.Date;


            Assert.Equal(14, difference.Days);
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
            Assert.Equal(200m, item.BaseUnitPrice);
            Assert.Equal(540m, item.Total);
        }
    }
}