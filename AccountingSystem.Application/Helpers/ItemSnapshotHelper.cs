using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Helpers
{
    public static class ItemSnapshotHelper
    {
        public static List<QuotationItem> SnapshotQuotationItems(
            IEnumerable<QuotationItem> items,
            IDictionary<int, Product> products)
        {
            return items.Select(i =>
            {
                var snapshot = CreateSnapshotData(i.ProductId, i.Quantity, i.DiscountPercent, i.Position, products);

                return new QuotationItem
                {
                    ProductId = snapshot.ProductId,
                    ProductName = snapshot.ProductName,
                    ProductCode = snapshot.ProductCode,
                    VatRate = snapshot.VatRate,
                    Unit = snapshot.Unit,
                    Quantity = snapshot.Quantity,
                    DiscountPercent = snapshot.DiscountPercent,
                    Position = snapshot.Position,
                    BaseUnitPrice = snapshot.BaseUnitPrice,
                    Total = snapshot.Total
                };

            }).ToList();
        }


        public static List<OrderItem> SnapshotOrderItems(
            IEnumerable<OrderItem> items,
            IDictionary<int, Product> products)
        {
            return items.Select(i =>
            {
                var snapshot = CreateSnapshotData(i.ProductId, i.Quantity, i.DiscountPercent, i.Position, products);

                return new OrderItem
                {
                    ProductId = snapshot.ProductId,
                    ProductName = snapshot.ProductName,
                    ProductCode = snapshot.ProductCode,
                    VatRate = snapshot.VatRate,
                    Unit = snapshot.Unit,
                    Quantity = snapshot.Quantity,
                    DiscountPercent = snapshot.DiscountPercent,
                    Position = snapshot.Position,
                    BaseUnitPrice = snapshot.BaseUnitPrice,
                    Total = snapshot.Total
                };

            }).ToList();
        }


        public static List<InvoiceItem> SnapshotInvoiceItems(
            IEnumerable<InvoiceItem> items,
            IDictionary<int, Product> products)
        {
            return items.Select(i =>
            {
                var snapshot = CreateSnapshotData(i.ProductId, i.Quantity, i.DiscountPercent, i.Position, products);

                return new InvoiceItem
                {
                    ProductId = snapshot.ProductId,
                    ProductName = snapshot.ProductName,
                    ProductCode = snapshot.ProductCode,
                    VatRate = snapshot.VatRate,
                    Unit = snapshot.Unit,
                    Quantity = snapshot.Quantity,
                    DiscountPercent = snapshot.DiscountPercent,
                    Position = snapshot.Position,
                    BaseUnitPrice = snapshot.BaseUnitPrice,
                    Total = snapshot.Total
                };

            }).ToList();
        }


        private static ItemSnapshotData CreateSnapshotData(
            int productId,
            int quantity,
            decimal discountPercent,
            int position,
            IDictionary<int, Product> products)
        {
            if (!products.TryGetValue(productId, out var product))
                throw new InvalidOperationException($"Product not found: {productId}");

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new InvalidOperationException($"Product name missing: {productId}");

            return new ItemSnapshotData
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductCode = product.ProductCode,
                VatRate = product.VatRate,
                Unit = product.Unit,
                Quantity = quantity,
                DiscountPercent = discountPercent,
                Position = position,
                BaseUnitPrice = product.Price,
                Total = CalculateTotal(
                    quantity,
                    product.Price,
                    discountPercent)
            };
        }


        private static decimal CalculateTotal(
            decimal quantity,
            decimal price,
            decimal discountPercent)
        {
            return Math.Round(
                quantity * price * (1 - discountPercent / 100m),
                2,
                MidpointRounding.AwayFromZero);
        }
    }
}