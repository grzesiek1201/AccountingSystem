namespace AccountingSystem.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal VatRate { get; set; }
        public ProductUnit Unit { get; set; }

        public int CategoryId { get; set; }
        public ProductCategory Category { get; set; }

        public bool IsProductArchived { get; set; }
    }
}
