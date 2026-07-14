using AccountingSystem.Application.DTOs.Customers;
using AccountingSystem.Application.DTOs.Invoices;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Mappers
{
    public class InvoiceResponseMapper
    {
        public InvoiceResponse Map(Invoice i)
        {
            return new InvoiceResponse
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                Status = i.Status.ToString(),
                DateCreated = i.DateCreated,
                IssueDate = i.IssueDate,
                DueDate = i.DueDate,

                Customer = new CustomerResponse
                {
                    Id = i.CustomerId,
                    NIP = i.CustomerNIP,
                    Email = i.CustomerEmail,
                    Name = i.CustomerName,
                    Street = i.CustomerStreet,
                    ZipCode = i.CustomerZipCode,
                    City = i.CustomerCity
                },

                Items = i.Items.Select(i => new InvoiceItemResponse
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductCode = i.ProductCode,
                    VatRate = i.VatRate,
                    Unit = i.Unit,
                    Quantity = i.Quantity,
                    DiscountPercent = i.DiscountPercent,
                    BaseUnitPrice = i.BaseUnitPrice,
                    Total = i.Total
                }).ToList()
            };
        }
    }
}
