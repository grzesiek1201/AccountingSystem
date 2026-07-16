using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Bogus;

namespace AccountingSystem.Seeder.Bogus
{
    public static class BogusProductGenerator
    {
        private static readonly Faker Faker = new("pl");


        public static List<Product> Generate(
            List<ProductCategory> categories,
            int count)
        {
            var products = new List<Product>();

            for (int i = 1; i <= count; i++)
            {
                var category = Faker.PickRandom(categories);

                products.Add(new Product
                {
                    Name = Faker.Commerce.ProductName(),

                    ProductCode =
                        $"{category.ProductCode}-{i:D4}",

                    Price =
                        Math.Round(
                            Faker.Random.Decimal(10, 5000),
                            2),

                    VatRate = 23,

                    Unit = Faker.PickRandom<ProductUnit>(),

                    CategoryId = category.Id,

                    IsProductArchived = false
                });
            }

            return products;
        }
    }
}