using AccountingSystem.Infrastructure.Data;
using AccountingSystem.Seeder.Bogus;
using AccountingSystem.Seeder.Helpers;
using Microsoft.EntityFrameworkCore;


var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlServer(
        "Server=localhost\\SQLEXPRESS;Database=AccountingSystemDb;Trusted_Connection=True;TrustServerCertificate=True")
    .Options;

using var context = new AppDbContext(options);

DatabaseCleaner.Clear(context);

// Kategorie
var categories = BogusProductCategoryGenerator.Generate();

context.ProductCategories.AddRange(categories);
context.SaveChanges();


// Produkty
var products = BogusProductGenerator.Generate(categories, 100);

context.Products.AddRange(products);
context.SaveChanges();


// Klienci
var customers = BogusCustomerGenerator.Generate(200);

context.Customers.AddRange(customers);
context.SaveChanges();



var documentGenerator = new BogusDocumentGenerator(context);

documentGenerator.Generate(100);