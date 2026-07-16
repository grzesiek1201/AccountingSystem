using AccountingSystem.Infrastructure.Data;

namespace AccountingSystem.Seeder.Helpers;

public static class DatabaseCleaner
{
    public static void Clear(AppDbContext context)
    {
        context.Payments.RemoveRange(context.Payments);
        context.InvoiceItems.RemoveRange(context.InvoiceItems);
        context.Invoices.RemoveRange(context.Invoices);

        context.OrderItems.RemoveRange(context.OrderItems);
        context.Orders.RemoveRange(context.Orders);

        context.QuotationItems.RemoveRange(context.QuotationItems);
        context.Quotations.RemoveRange(context.Quotations);

        context.Products.RemoveRange(context.Products);
        context.ProductCategories.RemoveRange(context.ProductCategories);

        context.Customers.RemoveRange(context.Customers);

        context.SaveChanges();
    }
}