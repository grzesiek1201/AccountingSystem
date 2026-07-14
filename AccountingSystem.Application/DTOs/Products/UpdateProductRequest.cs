namespace AccountingSystem.Application.DTOs.Products
{
    public class UpdateProductRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public decimal VatRate { get; set; }
        public ProductUnit Unit { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }
}
