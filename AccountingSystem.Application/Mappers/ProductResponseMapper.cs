using AccountingSystem.Application.DTOs.Products;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Mappers
{
    public class ProductResponseMapper
    {
        public ProductResponse Map(Product p)
        {
            return new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                ProductCode = p.ProductCode,
                VatRate = p.VatRate,
                Unit = p.Unit,
                Price = p.Price,
                CategoryId = p.CategoryId
            };
        }
    }
}

