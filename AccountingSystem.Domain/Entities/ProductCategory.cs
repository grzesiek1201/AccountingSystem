namespace AccountingSystem.Domain.Entities
{
    public class ProductCategory
    {
        public int Id { get; set; }

        public  string Name { get; set; } = string.Empty;

        public  string ProductCode { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = new List<Product>();

        public bool IsActive { get; set; }
    }
}
