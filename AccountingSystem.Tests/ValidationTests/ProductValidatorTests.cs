using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Domain.Entities;
using Xunit;
using System.Collections.Generic;


namespace AccountingSystem.Tests.ServicesTests
{
    public class ProductValidatorTests
    {
        private readonly ProductValidator _validator = new();

        private Product CreateValidProduct()
        {
            return new Product
            {
                Id = 2,
                ProductCode = "CHOC-001",
                Name = "Chocolate GOLD",
                Price = 100m,
                VatRate = 23m,
                Unit = ProductUnit.Piece,
                CategoryId = 1,
                IsProductArchived = false
            };
        }

        [Fact]
        public void Validate_ValidProduct_ShouldReturnValidResult()
        {
            var product = CreateValidProduct();

            var result = _validator.Validate(product, new List<Product>());

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_EmptyName_ShouldContainEmptyNameError()
        {
            var product = CreateValidProduct();
            product.Name = "";

            var result = _validator.Validate(product, new List<Product>());

            Assert.Contains(ProductValidationError.EmptyName, result.Errors);
        }

        [Fact]
        public void Validate_NameTooLong_ShouldContainNameTooLongError()
        {
            var product = CreateValidProduct();
            product.Name = new string('A', 70);

            var result = _validator.Validate(product, new List<Product>());

            Assert.Contains(ProductValidationError.NameTooLong, result.Errors);
        }

        [Fact]
        public void Validate_DuplicateName_ShouldContainDuplicateNameError()
        {
            var existing = new Product
            {
                Id = 1,
                ProductCode = "CHOC-001",
                Name = "Chocolate GOLD",
                Price = 50,
                VatRate = 23m,
                Unit = ProductUnit.Piece,
                CategoryId = 1
            };

            var product = CreateValidProduct();
            product.Id = 2;

            var result = _validator.Validate(product, new List<Product> { existing });

            Assert.Contains(ProductValidationError.DuplicateName, result.Errors);
        }

        [Fact]
        public void Validate_InvalidPrice_ShouldContainInvalidPriceError()
        {
            var product = CreateValidProduct();
            product.Price = -10;

            var result = _validator.Validate(product, new List<Product>());

            Assert.Contains(ProductValidationError.InvalidPrice, result.Errors);
        }
    }
}
