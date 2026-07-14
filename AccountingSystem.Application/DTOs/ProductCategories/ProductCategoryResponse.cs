namespace AccountingSystem.Application.DTOs.ProductCategories
{
    public class ProductCategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
