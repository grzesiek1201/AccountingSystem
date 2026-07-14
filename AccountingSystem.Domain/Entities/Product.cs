namespace AccountingSystem.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public  string ProductCode { get; set; } = string.Empty;
        public  string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal VatRate { get; set; }
        public ProductUnit Unit { get; set; }

        public int CategoryId { get; set; }
        public  ProductCategory Category { get; set; } = null!;

        public bool IsProductArchived { get; set; }
    }
}
