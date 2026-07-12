using AccountingSystem.Application.Converters;
using AccountingSystem.Domain.Entities;
using Xunit;

namespace AccountingSystem.Tests.ConvertersTests
{
    public class QuotationToOrderConverterTests
    {
        private readonly QuotationToOrderConverter _converter;

        public QuotationToOrderConverterTests()
        {
            _converter = new QuotationToOrderConverter();
        }

        private Quotation CreateQuotation()
        {
            return new Quotation
            {
                Id = 1,
                CustomerId = 10,
                CustomerName = "Test",
                Items = new List<QuotationItem>
                {
                    new QuotationItem
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
        public void Map_ShouldConvertQuotationToOrder()
        {
            var quotation = CreateQuotation();

            var result = _converter.Map(quotation);

            Assert.NotNull(result);
            Assert.Equal(quotation.Id, result.QuotationId);
            Assert.Equal(quotation.CustomerId, result.CustomerId);
            Assert.Equal(quotation.CustomerName, result.CustomerName);
        }

        [Fact]
        public void Map_ShouldReturnNull_WhenQuotationIsNull()
        {
            Quotation quotation = null;

            var result = _converter.Map(quotation);

            Assert.Null(result);
        }

        [Fact]
        public void Map_ShouldMapQuotationItemsToOrderItems()
        {
            var quotation = CreateQuotation();

            var result = _converter.Map(quotation);

            Assert.NotNull(result.Items);
            Assert.Single(result.Items);

            var orderItem = result.Items.First();
            var quotationItem = quotation.Items.First();

            Assert.Equal(quotationItem.ProductId, orderItem.ProductId);
            Assert.Equal(quotationItem.Quantity, orderItem.Quantity);
            Assert.Equal(quotationItem.BaseUnitPrice, orderItem.BaseUnitPrice);
            Assert.Equal(quotationItem.DiscountPercent, orderItem.DiscountPercent);
            Assert.Equal(quotationItem.Position, orderItem.Position);
        }
    }
}