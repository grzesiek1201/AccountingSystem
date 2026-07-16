using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Seeder.Bogus
{
    public static class BogusProductCategoryGenerator
    {
        public static List<ProductCategory> Generate()
        {
            return new List<ProductCategory>
            {
                new ProductCategory
                {
                    Name = "Towary",
                    ProductCode = "TOW",
                    IsActive = true
                },

                new ProductCategory
                {
                    Name = "Usługi",
                    ProductCode = "USL",
                    IsActive = true
                },

                new ProductCategory
                {
                    Name = "Wyposażenie",
                    ProductCode = "WYP",
                    IsActive = true
                },

                new ProductCategory
                {
                    Name = "Materiały",
                    ProductCode = "MAT",
                    IsActive = true
                },

                new ProductCategory
                {
                    Name = "Inne",
                    ProductCode = "INN",
                    IsActive = true
                }
            };
        }
    }
}