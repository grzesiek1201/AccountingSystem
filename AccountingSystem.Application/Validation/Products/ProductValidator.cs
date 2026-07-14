using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Validation.Products
{
    public class ProductValidator
    {
        public ProductValidationResult Validate(Product product, List<Product> products)
        {
            var result = new ProductValidationResult();

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                result.Errors.Add(ProductValidationError.EmptyName);
            }
            else
            {
                if (product.Name.Length > 64)
                    result.Errors.Add(ProductValidationError.NameTooLong);

                if (products.Exists(x => x.Name == product.Name && x.Id != product.Id))
                    result.Errors.Add(ProductValidationError.DuplicateName);
            }

            if (string.IsNullOrWhiteSpace(product.ProductCode))
            {
                result.Errors.Add(ProductValidationError.EmptyProductCode);
            }
            else
            {
                if (product.ProductCode.Length > 64)
                    result.Errors.Add(ProductValidationError.ProductCodeTooLong);

                if (products.Exists(x => x.ProductCode == product.ProductCode && x.Id != product.Id))
                    result.Errors.Add(ProductValidationError.DuplicateProductCode);
            }

            if (product.Price <= 0)
                {
                result.Errors.Add(ProductValidationError.InvalidPrice);
                }

            if (product.VatRate < 0 || product.VatRate > 100)
                {
                result.Errors.Add(ProductValidationError.InvalidVatRate);
                }

            if (!Enum.IsDefined(typeof(ProductUnit), product.Unit))
            {
                result.Errors.Add(ProductValidationError.InvalidUnit);
            }

            if (product.CategoryId <= 0)
                {
                result.Errors.Add(ProductValidationError.EmptyCategory);
                }

            return result;
        }
    }
}
