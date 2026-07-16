using AccountingSystem.Domain.Entities;
using Bogus;

namespace AccountingSystem.Seeder.Bogus
{
    public static class BogusCustomerGenerator
    {
        private static readonly Faker Faker = new("pl");


        public static List<Customer> Generate(int count)
        {
            var customers = new List<Customer>();

            for (int i = 0; i < count; i++)
            {
                var company = Faker.Company.CompanyName();

                customers.Add(new Customer
                {
                    Name = company,

                    NIP =
                        Faker.Random.ReplaceNumbers(
                            "##########"),

                    Email =
                        Faker.Internet.Email(),

                    Street =
                        Faker.Address.StreetAddress(),

                    City =
                        Faker.Address.City(),

                    ZipCode =
                        Faker.Address.ZipCode(),

                    IsCustomerArchived = false,

                    InDebt =
                        Faker.Random.Bool(15)
                });
            }


            return customers;
        }
    }
}