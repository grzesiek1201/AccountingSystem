namespace AccountingSystem.Application.Helpers
{
    public class ItemSnapshotData
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; }
        public decimal VatRate { get; set; }
        public ProductUnit Unit { get; set; }

        public int Quantity { get; set; }
        public decimal DiscountPercent { get; set; }
        public int Position { get; set; }
        public decimal BaseUnitPrice { get; set; }
        public decimal Total { get; set; }
    }
}