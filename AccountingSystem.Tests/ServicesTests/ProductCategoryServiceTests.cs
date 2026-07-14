using AccountingSystem.Application.DTOs.ProductCategories;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.ProductCategories;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AccountingSystem.Tests.ServicesTests
{
    public class ProductCategoryServiceTests
    {
        private readonly Mock<IProductCategoryRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILogger<ProductCategoryService>> _loggerMock;

        private readonly ProductCategoryValidator _validator;
        private readonly ProductCategoryService _service;

        public ProductCategoryServiceTests()
        {
            _repoMock = new Mock<IProductCategoryRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ProductCategoryService>>();

            _validator = new ProductCategoryValidator();

            _service = new ProductCategoryService(
                _repoMock.Object,
                _validator,
                _uowMock.Object,
                _loggerMock.Object
            );
        }

        private ProductCategory CreateValidCategory()
        {
            return new ProductCategory
            {
                Id = 1,
                Name = "Sweets",
                IsActive = true
            };
        }

        private CreateProductCategoryRequest CreateValidRequest()
        {
            return new CreateProductCategoryRequest
            {
                Name = "Sweets"
            };
        }

        private UpdateProductCategoryRequest CreateValidUpdateRequest(int id = 1)
        {
            return new UpdateProductCategoryRequest
            {
                Id = id,
                Name = "Updated"
            };
        }

        // ================= ADD =================

        [Fact]
        public void AddProductCategory_Valid_ShouldReturnSuccess()
        {
            var request = CreateValidRequest();

            _repoMock.Setup(x => x.GetAll())
                .Returns(new List<ProductCategory>());

            var result = _service.AddProductCategory(request);

            Assert.Equal(ProductCategoryAddResult.Success, result.Result);

            _repoMock.Verify(x => x.Add(It.IsAny<ProductCategory>()), Times.Once);
            _uowMock.Verify(x => x.Save(), Times.Once);
        }

        [Fact]
        public void AddProductCategory_Invalid_ShouldReturnInvalidData()
        {
            var request = CreateValidRequest();
            request.Name = "";

            _repoMock.Setup(x => x.GetAll())
                .Returns(new List<ProductCategory>());

            var result = _service.AddProductCategory(request);

            Assert.Equal(ProductCategoryAddResult.InvalidData, result.Result);

            _repoMock.Verify(x => x.Add(It.IsAny<ProductCategory>()), Times.Never);
            _uowMock.Verify(x => x.Save(), Times.Never);
        }

        [Fact]
        public void AddProductCategory_Duplicate_ShouldReturnInvalidData()
        {
            var existing = CreateValidCategory();

            _repoMock.Setup(x => x.GetAll())
                .Returns(new List<ProductCategory> { existing });

            var request = CreateValidRequest();

            var result = _service.AddProductCategory(request);

            Assert.Equal(ProductCategoryAddResult.InvalidData, result.Result);
        }

        // ================= EDIT =================

        [Fact]
        public void EditProductCategory_NotFound_ShouldReturnNotFound()
        {
            _repoMock.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns((ProductCategory)null!);

            var result = _service.EditProductCategory(CreateValidUpdateRequest());

            Assert.Equal(ProductCategoryEditResult.NotFound, result.Result);
        }

        [Fact]
        public void EditProductCategory_Inactive_ShouldReturnInactive()
        {
            var category = CreateValidCategory();
            category.IsActive = false;

            _repoMock.Setup(x => x.GetById(1))
                .Returns(category);

            var result = _service.EditProductCategory(CreateValidUpdateRequest());

            Assert.Equal(ProductCategoryEditResult.ProductCategoryInactive, result.Result);
        }

        [Fact]
        public void EditProductCategory_Valid_ShouldReturnSuccess()
        {
            var category = CreateValidCategory();

            _repoMock.Setup(x => x.GetById(1))
                .Returns(category);

            _repoMock.Setup(x => x.GetAll())
                .Returns(new List<ProductCategory> { category });

            var result = _service.EditProductCategory(CreateValidUpdateRequest());

            Assert.Equal(ProductCategoryEditResult.Success, result.Result);

            _repoMock.Verify(x => x.Update(category), Times.Once);
            _uowMock.Verify(x => x.Save(), Times.Once);
        }

        [Fact]
        public void EditProductCategory_Invalid_ShouldReturnInvalidData()
        {
            var category = CreateValidCategory();

            _repoMock.Setup(x => x.GetById(1))
                .Returns(category);

            _repoMock.Setup(x => x.GetAll())
                .Returns(new List<ProductCategory>());

            var request = CreateValidUpdateRequest();
            request.Name = ""; // invalid

            var result = _service.EditProductCategory(request);

            Assert.Equal(ProductCategoryEditResult.InvalidData, result.Result);

            _repoMock.Verify(x => x.Update(It.IsAny<ProductCategory>()), Times.Never);
            _uowMock.Verify(x => x.Save(), Times.Never);
        }

        // ================= STATUS =================

        [Fact]
        public void ChangeStatus_NotFound_ShouldReturnNotFound()
        {
            _repoMock.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns((ProductCategory)null!);

            var result = _service.ChangeProductCategoryStatus(1, true);

            Assert.Equal(ProductCategoryStatusResult.NotFound, result.Result);
        }

        [Fact]
        public void ChangeStatus_Valid_ShouldReturnSuccess()
        {
            var category = CreateValidCategory();

            _repoMock.Setup(x => x.GetById(1))
                .Returns(category);

            var result = _service.ChangeProductCategoryStatus(1, false);

            Assert.Equal(ProductCategoryStatusResult.Success, result.Result);

            _repoMock.Verify(x => x.Update(category), Times.Once);
            _uowMock.Verify(x => x.Save(), Times.Once);
        }

        // ================= READ =================

        [Fact]
        public void GetAll_ShouldReturnList()
        {
            _repoMock.Setup(x => x.GetAll())
                .Returns(new List<ProductCategory>
                {
                    CreateValidCategory(),
                    CreateValidCategory()
                });

            var result = _service.GetAllProductCategories();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnCategory()
        {
            var category = CreateValidCategory();

            _repoMock.Setup(x => x.GetById(1))
                .Returns(category);

            var result = _service.GetProductCategoryById(1);

            Assert.NotNull(result);
            Assert.Equal(category.Id, result.Id);
        }

        [Fact]
        public void GetById_NotFound_ShouldReturnNull()
        {
            _repoMock.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns((ProductCategory)null!);

            var result = _service.GetProductCategoryById(1);

            Assert.Null(result);
        }
    }
}