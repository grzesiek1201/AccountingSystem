using AccountingSystem.Application.Converters;
using AccountingSystem.Application.DTOs.Orders;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Mappers;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Orders;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AccountingSystem.Tests.ServicesTests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly Mock<INumberSequenceService> _seqMock;
        private readonly Mock<ICustomerRepository> _customerRepo;
        private readonly Mock<IProductRepository> _productRepo;
        private readonly Mock<IQuotationRepository> _quotationRepoMock;
        private readonly Mock<OrderResponseMapper> _orderMapperMock;
        private readonly Mock<QuotationToOrderConverter> _quotationToOrderMapperMock;

        private readonly OrderValidator _validator;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _repoMock = new Mock<IOrderRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<OrderService>>();
            _seqMock = new Mock<INumberSequenceService>();
            _customerRepo = new Mock<ICustomerRepository>();
            _productRepo = new Mock<IProductRepository>();
            _quotationRepoMock = new Mock<IQuotationRepository>();
            _orderMapperMock = new Mock<OrderResponseMapper>();
            _quotationToOrderMapperMock = new Mock<QuotationToOrderConverter>();

            _seqMock.Setup(x => x.GetNext(It.IsAny<DocumentType>()))
                .Returns("O-2026-0001");

            _customerRepo.Setup(x => x.GetById(1))
                .Returns(new Customer { Id = 1, Name = "Test" });

            _productRepo.Setup(x => x.GetByIds(It.IsAny<List<int>>()))
                .Returns(new List<Product>
                {
                    new Product { Id = 1, Name = "Test", Price = 100m }
                });

            _validator = new OrderValidator();

            _service = new OrderService(
                _repoMock.Object,
                _validator,
                _uowMock.Object,
                _loggerMock.Object,
                _seqMock.Object,
                _customerRepo.Object,
                _productRepo.Object,
                _orderMapperMock.Object,
                _quotationRepoMock.Object,
                _quotationToOrderMapperMock.Object
            );
        }

        private CreateOrderRequest CreateValidRequest()
        {
            return new CreateOrderRequest
            {
                CustomerId = 1,
                Items = new List<CreateOrderItemRequest>
                {
                    new CreateOrderItemRequest
                    {
                        ProductId = 1,
                        Quantity = 2,
                        DiscountPercent = 0
                    }
                }
            };
        }

        private CreateOrderRequest CreateInvalidRequest_NoItems()
        {
            return new CreateOrderRequest
            {
                CustomerId = 1,
                Items = new List<CreateOrderItemRequest>()
            };
        }

        private UpdateOrderRequest CreateUpdateRequest()
        {
            return new UpdateOrderRequest
            {
                Id = 1,
                CustomerId = 1,
                Status = OrderStatus.Draft,
                Items = new List<UpdateOrderItemRequest>
                {
                    new UpdateOrderItemRequest
                    {
                        ProductId = 1,
                        Quantity = 3,
                        DiscountPercent = 0
                    }
                }
            };
        }

        // ================= ADD =================

        [Fact]
        public void AddOrder_Valid_ShouldReturnSuccess()
        {
            var request = CreateValidRequest();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Order>());

            var result = _service.AddOrder(request);

            Assert.Equal(OrderAddResult.Success, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void AddOrder_Invalid_ShouldReturnInvalidData()
        {
            var request = CreateInvalidRequest_NoItems();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Order>());

            var result = _service.AddOrder(request);

            Assert.Equal(OrderAddResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Order>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        // ================= EDIT =================

        [Fact]
        public void EditOrder_NotFound_ShouldReturnNotFound()
        {
            var request = CreateUpdateRequest();

            _repoMock.Setup(r => r.GetById(request.Id))
                .Returns((Order)null);

            var result = _service.EditOrder(request);

            Assert.Equal(OrderEditResult.NotFound, result.Result);
        }

        [Fact]
        public void EditOrder_Archived_ShouldReturnOrderArchived()
        {
            var order = new Order { Id = 1, IsOrderArchived = true };

            var request = CreateUpdateRequest();

            _repoMock.Setup(r => r.GetById(request.Id))
                .Returns(order);

            var result = _service.EditOrder(request);

            Assert.Equal(OrderEditResult.OrderArchived, result.Result);

            _repoMock.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void EditOrder_Valid_ShouldReturnSuccess()
        {
            var order = new Order { Id = 1 };

            var request = CreateUpdateRequest();

            _repoMock.Setup(r => r.GetById(request.Id))
                .Returns(order);

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Order>());

            var result = _service.EditOrder(request);

            Assert.Equal(OrderEditResult.Success, result.Result);

            _repoMock.Verify(r => r.Update(It.IsAny<Order>()), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        // ================= ARCHIVE =================

        [Fact]
        public void ArchiveOrder_Existing_ShouldReturnSuccess()
        {
            var order = new Order { Id = 1 };

            _repoMock.Setup(r => r.GetById(order.Id))
                .Returns(order);

            var result = _service.ArchiveOrder(order.Id);

            Assert.Equal(ArchiveOrderResult.Success, result);
        }

        // ================= READ =================

        [Fact]
        public void FindOrder_Existing_ShouldReturnOrder()
        {
            var order = new Order { Id = 1 };

            _repoMock.Setup(r => r.GetById(order.Id))
                .Returns(order);

            var result = _service.FindOrder(order.Id);

            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
        }

        [Fact]
        public void GetAllOrders_ShouldReturnAllOrders()
        {
            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Order> { new Order(), new Order() });

            var result = _service.GetAllOrders();

            Assert.Equal(2, result.Count);
        }
    }
}