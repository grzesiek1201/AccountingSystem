using AccountingSystem.Application.Validation.ProductCategories;
using AccountingSystem.Domain.Entities;
using Xunit;
using System.Collections.Generic;

namespace AccountingSystem.Tests.ValidationTests
{
    public class ProductCategoryValidatorTests
    {
        private readonly ProductCategoryValidator _validator = new();

        private ProductCategory CreateValidCategory()
        {
            return new ProductCategory
            {
                Id = 2,
                Name = "Sweets",
                IsActive = true
            };
        }

        // ================= VALID =================

        [Fact]
        public void Validate_ValidCategory_ShouldBeValid()
        {
            var category = CreateValidCategory();

            var result = _validator.Validate(category, new List<ProductCategory>());

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        // ================= EMPTY NAME =================

        [Fact]
        public void Validate_EmptyName_ShouldReturnEmptyNameError()
        {
            var category = CreateValidCategory();
            category.Name = "";

            var result = _validator.Validate(category, new List<ProductCategory>());

            Assert.Contains(ProductCategoryValidationError.EmptyName, result.Errors);
        }

        [Fact]
        public void Validate_NullName_ShouldReturnEmptyNameError()
        {
            var category = CreateValidCategory();
            category.Name = null;

            var result = _validator.Validate(category, new List<ProductCategory>());

            Assert.Contains(ProductCategoryValidationError.EmptyName, result.Errors);
        }

        // ================= NAME TOO LONG =================

        [Fact]
        public void Validate_NameTooLong_ShouldReturnError()
        {
            var category = CreateValidCategory();
            category.Name = new string('A', 70);

            var result = _validator.Validate(category, new List<ProductCategory>());

            Assert.Contains(ProductCategoryValidationError.NameTooLong, result.Errors);
        }

        // ================= DUPLICATE =================

        [Fact]
        public void Validate_DuplicateName_ShouldReturnError()
        {
            var existing = new ProductCategory
            {
                Id = 1,
                Name = "Sweets",
                IsActive = true
            };

            var category = CreateValidCategory();
            category.Id = 2;

            var result = _validator.Validate(category, new List<ProductCategory> { existing });

            Assert.Contains(ProductCategoryValidationError.DuplicateName, result.Errors);
        }

        [Fact]
        public void Validate_SameNameSameId_ShouldNotReturnDuplicateError()
        {
            var existing = new ProductCategory
            {
                Id = 1,
                Name = "Sweets",
                IsActive = true
            };

            var category = new ProductCategory
            {
                Id = 1,
                Name = "Sweets",
                IsActive = true
            };

            var result = _validator.Validate(category, new List<ProductCategory> { existing });

            Assert.DoesNotContain(ProductCategoryValidationError.DuplicateName, result.Errors);
        }
    }
}