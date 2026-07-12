using AccountingSystem.Application.Helpers;
using AccountingSystem.Domain.Entities;
using Xunit;

namespace AccountingSystem.Tests.HelpersTests
{
    public class ItemSnapshotHelperTests
    {
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


        [Fact]
        public void SnapshotOrderItems_ShouldCreateSnapshotWithCorrectValues()
        {
            var items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 1,
                    Quantity = 2,
                    DiscountPercent = 10,
                    Position = 1
                }
            };

            var products = CreateProducts();

            var result = ItemSnapshotHelper.SnapshotOrderItems(items, products);

            Assert.NotNull(result);
            Assert.Single(result);

            var item = result.First();

            Assert.Equal(1, item.ProductId);
            Assert.Equal("Laptop", item.ProductName);
            Assert.Equal(2, item.Quantity);
            Assert.Equal(10, item.DiscountPercent);
            Assert.Equal(1000m, item.BaseUnitPrice);

            // 2 * 1000 - 10%
            Assert.Equal(1800m, item.Total);
        }


        [Fact]
        public void SnapshotQuotationItems_ShouldCreateSnapshotWithCorrectValues()
        {
            var items = new List<QuotationItem>
            {
                new QuotationItem
                {
                    ProductId = 1,
                    Quantity = 3,
                    DiscountPercent = 20,
                    Position = 2
                }
            };

            var products = CreateProducts();

            var result = ItemSnapshotHelper.SnapshotQuotationItems(items, products);

            Assert.Single(result);

            var item = result.First();

            Assert.Equal("Laptop", item.ProductName);
            Assert.Equal(3, item.Quantity);
            Assert.Equal(20, item.DiscountPercent);

            // 3 * 1000 - 20%
            Assert.Equal(2400m, item.Total);
        }


        [Fact]
        public void SnapshotInvoiceItems_ShouldCreateSnapshotWithCorrectValues()
        {
            var items = new List<InvoiceItem>
            {
                new InvoiceItem
                {
                    ProductId = 1,
                    Quantity = 1,
                    DiscountPercent = 0,
                    Position = 1
                }
            };

            var products = CreateProducts();

            var result = ItemSnapshotHelper.SnapshotInvoiceItems(items, products);

            Assert.Single(result);

            var item = result.First();

            Assert.Equal("Laptop", item.ProductName);
            Assert.Equal(1000m, item.BaseUnitPrice);
            Assert.Equal(1000m, item.Total);
        }


        [Fact]
        public void SnapshotOrderItems_ShouldThrow_WhenProductDoesNotExist()
        {
            var items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 99,
                    Quantity = 1
                }
            };

            var products = CreateProducts();


            Assert.Throws<InvalidOperationException>(() =>
                ItemSnapshotHelper.SnapshotOrderItems(items, products));
        }


        [Fact]
        public void SnapshotOrderItems_ShouldThrow_WhenProductNameIsEmpty()
        {
            var items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 1,
                    Quantity = 1
                }
            };

            var products = new Dictionary<int, Product>
            {
                {
                    1,
                    new Product
                    {
                        Id = 1,
                        Name = "",
                        Price = 100m
                    }
                }
            };


            Assert.Throws<InvalidOperationException>(() =>
                ItemSnapshotHelper.SnapshotOrderItems(items, products));
        }
    }
}