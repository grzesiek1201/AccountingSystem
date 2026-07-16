using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using AccountingSystem.Infrastructure.Data;

namespace AccountingSystem.Seeder.Bogus;

public class BogusDocumentGenerator
{
    private readonly AppDbContext _context;

    public BogusDocumentGenerator(AppDbContext context)
    {
        _context = context;
    }


    public void Generate(int count = 100)
    {
        if (_context.Quotations.Any())
        {
            return;
        }


        var customers = _context.Customers.ToList();
        var products = _context.Products.ToList();

        var random = new Random();


        for (int i = 1; i <= count; i++)
        {
            var customer = customers[random.Next(customers.Count)];


            var quotation = new Quotation
            {
                QuotationNumber = $"OF/2026/{i:D4}",
                DateCreated = DateTime.Now.AddDays(-random.Next(1, 365)),

                CustomerId = customer.Id,

                CustomerName = customer.Name,
                CustomerNIP = customer.NIP,
                CustomerEmail = customer.Email,
                CustomerStreet = customer.Street,
                CustomerZipCode = customer.ZipCode,
                CustomerCity = customer.City
            };


            var selectedProducts = products
                .OrderBy(x => random.Next())
                .Take(random.Next(1, 5))
                .ToList();


            int position = 1;


            foreach (var product in selectedProducts)
            {
                var quantity = random.Next(1, 10);


                quotation.Items.Add(new QuotationItem
                {
                    Position = position++,

                    ProductId = product.Id,

                    ProductName = product.Name,
                    ProductCode = product.ProductCode,

                    VatRate = product.VatRate,
                    Unit = product.Unit,

                    Quantity = quantity,

                    DiscountPercent = random.Next(0, 20),

                    BaseUnitPrice = product.Price,

                    Total = product.Price * quantity
                });
            }


            quotation.Send();
            quotation.Accept();



            var order = new Order
            {
                OrderNumber = $"ZAM/2026/{i:D4}",

                DateCreated = quotation.DateCreated,

                QuotationId = quotation.Id,

                CustomerId = quotation.CustomerId,

                CustomerName = quotation.CustomerName,
                CustomerNIP = quotation.CustomerNIP,
                CustomerEmail = quotation.CustomerEmail,
                CustomerStreet = quotation.CustomerStreet,
                CustomerZipCode = quotation.CustomerZipCode,
                CustomerCity = quotation.CustomerCity
            };


            foreach (var item in quotation.Items)
            {
                order.Items.Add(new OrderItem
                {
                    Position = item.Position,

                    ProductId = item.ProductId,

                    ProductName = item.ProductName,
                    ProductCode = item.ProductCode,

                    VatRate = item.VatRate,
                    Unit = item.Unit,

                    Quantity = item.Quantity,

                    DiscountPercent = item.DiscountPercent,

                    BaseUnitPrice = item.BaseUnitPrice,

                    Total = item.Total
                });
            }


            order.Confirm();
            order.Complete();



            var invoice = new Invoice
            {
                InvoiceNumber = $"FV/2026/{i:D4}",

                OrderId = order.Id,

                DateCreated = order.DateCreated,

                IssueDate = order.DateCreated,

                DueDate = order.DateCreated.AddDays(14),

                CustomerId = order.CustomerId,


                CustomerName = order.CustomerName,
                CustomerNIP = order.CustomerNIP,
                CustomerEmail = order.CustomerEmail,
                CustomerStreet = order.CustomerStreet,
                CustomerZipCode = order.CustomerZipCode,
                CustomerCity = order.CustomerCity
            };


            foreach (var item in order.Items)
            {
                invoice.Items.Add(new InvoiceItem
                {
                    Position = item.Position,

                    ProductId = item.ProductId,

                    ProductName = item.ProductName,
                    ProductCode = item.ProductCode,

                    VatRate = item.VatRate,
                    Unit = item.Unit,

                    Quantity = item.Quantity,

                    DiscountPercent = item.DiscountPercent,

                    BaseUnitPrice = item.BaseUnitPrice,

                    Total = item.Total
                });
            }


            invoice.TotalAmount = invoice.Items.Sum(x => x.Total);

            invoice.Issue();



            var payment = new Payment
            {
                Invoice = invoice,

                Amount = invoice.TotalAmount,

                PaymentDate = invoice.DueDate
                    .AddDays(random.Next(-5, 10)),

                Method = PaymentMethod.BankTransfer
            };


            payment.Complete();



            _context.Quotations.Add(quotation);
            _context.Orders.Add(order);
            _context.Invoices.Add(invoice);
            _context.Payments.Add(payment);


            _context.SaveChanges();
        }
    }
}