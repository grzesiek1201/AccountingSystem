using AccountingSystem.Application.DTOs.Products;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AccountingSystem.Tests.ServicesTests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;

        private readonly ProductValidator _validator;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repoMock = new Mock<IProductRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ProductService>>();

            _validator = new ProductValidator();

            _service = new ProductService(
                _repoMock.Object,
                _validator,
                _uowMock.Object,
                _loggerMock.Object
            );
        }

        // ================= HELPERS =================

        private CreateProductRequest CreateValidRequest()
        {
            return new CreateProductRequest
            {
                Name = "Chocolate GOLD",
                Price = 100,
                CategoryId = 1
            };
        }

        private UpdateProductRequest CreateValidUpdateRequest()
        {
            return new UpdateProductRequest
            {
                Id = 1,
                Name = "Chocolate GOLD",
                Price = 120,
                CategoryId = 1
            };
        }

        // ================= ADD =================

        [Fact]
        public void AddProduct_Valid_ShouldReturnSuccess()
        {
            var request = CreateValidRequest();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Product>());

            var result = _service.AddProduct(request);

            Assert.Equal(ProductAddResult.Success, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void AddProduct_Invalid_ShouldReturnInvalidData()
        {
            var request = CreateValidRequest();
            request.Price = -10;

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Product>());

            var result = _service.AddProduct(request);

            Assert.Equal(ProductAddResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void AddProduct_DuplicateName_ShouldReturnInvalidData()
        {
            var request = CreateValidRequest();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Product>
                {
                    new Product
                    {
                        Id = 999,
                        Name = "Chocolate GOLD",
                        Price = 100,
                        CategoryId = 1
                    }
                });

            var result = _service.AddProduct(request);

            Assert.Equal(ProductAddResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Never);
        }

        // ================= EDIT =================

        [Fact]
        public void EditProduct_Valid_ShouldReturnSuccess()
        {
            var request = CreateValidUpdateRequest();

            _repoMock.Setup(r => r.GetById(request.Id))
                .Returns(new Product
                {
                    Id = 1,
                    Name = "Old",
                    Price = 50,
                    CategoryId = 1,
                    IsProductArchived = false
                });

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Product>());

            var result = _service.EditProduct(request);

            Assert.Equal(ProductEditResult.Success, result.Result);

            _repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void EditProduct_NotFound_ShouldReturnNotFound()
        {
            var request = CreateValidUpdateRequest();

            _repoMock.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Product)null!);

            var result = _service.EditProduct(request);

            Assert.Equal(ProductEditResult.NotFound, result.Result);
        }

        [Fact]
        public void EditProduct_Archived_ShouldReturnProductArchived()
        {
            var request = CreateValidUpdateRequest();

            _repoMock.Setup(r => r.GetById(request.Id))
                .Returns(new Product
                {
                    Id = 1,
                    IsProductArchived = true
                });

            var result = _service.EditProduct(request);

            Assert.Equal(ProductEditResult.ProductArchived, result.Result);
        }

        [Fact]
        public void EditProduct_Invalid_ShouldReturnInvalidData()
        {
            var request = CreateValidUpdateRequest();
            request.Price = -10;

            _repoMock.Setup(r => r.GetById(request.Id))
                .Returns(new Product
                {
                    Id = 1,
                    Name = request.Name,
                    Price = request.Price,
                    CategoryId = 1
                });

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Product>());

            var result = _service.EditProduct(request);

            Assert.Equal(ProductEditResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
        }

        // ================= ARCHIVE =================

        [Fact]
        public void ArchiveProduct_Existing_ShouldReturnSuccess()
        {
            var product = new Product { Id = 1 };

            _repoMock.Setup(r => r.GetById(1))
                .Returns(product);

            var result = _service.ArchiveProduct(1);

            Assert.Equal(ProductArchiveResult.Success, result);

            _repoMock.Verify(r => r.Update(product), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void ArchiveProduct_NotFound_ShouldReturnNotFound()
        {
            _repoMock.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Product)null!);

            var result = _service.ArchiveProduct(1);

            Assert.Equal(ProductArchiveResult.NotFound, result);
        }

        // ================= GET =================

        [Fact]
        public void GetProductById_Existing_ShouldReturnProduct()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Test",
                Price = 100,
                CategoryId = 1
            };

            _repoMock.Setup(r => r.GetById(1))
                .Returns(product);

            var result = _service.GetProductById(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void GetProductById_NotExisting_ShouldReturnNull()
        {
            _repoMock.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Product)null!);

            var result = _service.GetProductById(1);

            Assert.Null(result);
        }

        // ================= GET ALL =================

        [Fact]
        public void GetAllProducts_ShouldReturnAll()
        {
            var list = new List<Product>
            {
                new Product { Id = 1, Name = "A", Price = 10 },
                new Product { Id = 2, Name = "B", Price = 20 }
            };

            _repoMock.Setup(r => r.GetAll())
                .Returns(list);

            var result = _service.GetAllProducts();

            Assert.Equal(2, result.Count);
        }
    }
}