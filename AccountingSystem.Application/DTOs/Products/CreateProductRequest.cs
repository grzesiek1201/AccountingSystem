namespace AccountingSystem.Application.DTOs.Products
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string ProductCode { get; set; }
        public decimal VatRate { get; set; }
        public ProductUnit Unit { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }
}
