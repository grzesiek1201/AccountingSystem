using AccountingSystem.Application.Converters;
using AccountingSystem.Application.DTOs.Invoices;
using AccountingSystem.Application.Factories;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Mappers;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Invoices;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AccountingSystem.Tests.ServicesTests
{
    public class InvoiceServiceTests
    {
        private readonly Mock<IInvoiceRepository> _repoMock;
        private readonly Mock<IPaymentRepository> _paymentMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILogger<InvoiceService>> _loggerMock;
        private readonly Mock<INumberSequenceService> _seqMock;
        private readonly Mock<ICustomerRepository> _customerRepo;
        private readonly Mock<IProductRepository> _productRepo;
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IInvoiceStatusCalculator> _statusCalculatorMock;
        private readonly InvoiceFactory _factory;

        private readonly InvoiceResponseMapper _mapper;
        private readonly OrderToInvoiceConverter _orderToInvoiceMapper;

        private readonly InvoiceValidator _validator;
        private readonly InvoiceService _service;

        public InvoiceServiceTests()
        {
            _repoMock = new Mock<IInvoiceRepository>();
            _paymentMock = new Mock<IPaymentRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<InvoiceService>>();
            _seqMock = new Mock<INumberSequenceService>();
            _factory = new InvoiceFactory(_seqMock.Object);
            _customerRepo = new Mock<ICustomerRepository>();
            _productRepo = new Mock<IProductRepository>();
            _orderRepoMock = new Mock<IOrderRepository>();
            _statusCalculatorMock = new Mock<IInvoiceStatusCalculator>();



            _mapper = new InvoiceResponseMapper();
            _orderToInvoiceMapper = new OrderToInvoiceConverter();
            _validator = new InvoiceValidator();

            _seqMock.Setup(x => x.GetNext(It.IsAny<DocumentType>()))
                .Returns("I-2026-0001");

            _customerRepo.Setup(x => x.GetById(1))
                .Returns(new Customer
                {
                    Id = 1,
                    Name = "Test",
                    Email = "test@test.com",
                    City = "X",
                    Street = "Y",
                    ZipCode = "00-000"
                });

            _productRepo.Setup(x => x.GetByIds(It.IsAny<List<int>>()))
                .Returns(new List<Product>
                {
                    new Product
                    {
                        Id = 1,
                        Name = "Test",
                        Price = 100m
                    }
                });

            _service = new InvoiceService(
                _repoMock.Object,
                _paymentMock.Object,
                _validator,
                _uowMock.Object,
                _loggerMock.Object,
                _factory,
                _seqMock.Object,
                _customerRepo.Object,
                _productRepo.Object,
                _mapper,
                _orderRepoMock.Object,
                _orderToInvoiceMapper,
                _statusCalculatorMock.Object
                );
        }

        private CreateInvoiceRequest CreateValidRequest()
        {
            return new CreateInvoiceRequest
            {
                CustomerId = 1,
                Items = new List<CreateInvoiceItemRequest>
                {
                    new CreateInvoiceItemRequest
                    {
                        ProductId = 1,
                        Quantity = 2,
                        DiscountPercent = 0
                    }
                }
            };
        }

        private UpdateInvoiceRequest CreateValidUpdateRequest()
        {
            return new UpdateInvoiceRequest
            {
                Id = 1,
                CustomerId = 1,
                Status = InvoiceStatus.Draft,
                Items = new List<UpdateInvoiceItemRequest>
                {
                    new UpdateInvoiceItemRequest
                    {
                        ProductId = 1,
                        Quantity = 2,
                        DiscountPercent = 0
                    }
                }
            };
        }

        [Fact]
        public void AddInvoice_Valid_ShouldReturnSuccess()
        {
            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Invoice>());

            var result = _service.AddInvoice(CreateValidRequest());

            Assert.Equal(InvoiceAddResult.Success, result.Result);
        }

        [Fact]
        public void AddInvoice_Invalid_ShouldReturnInvalidData()
        {
            var req = CreateValidRequest();
            req.Items = new List<CreateInvoiceItemRequest>();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Invoice>());

            var result = _service.AddInvoice(req);

            Assert.Equal(InvoiceAddResult.InvalidData, result.Result);
        }

        [Fact]
        public void EditInvoice_NotFound_ShouldReturnNotFound()
        {
            var req = CreateValidUpdateRequest();

            _repoMock.Setup(r => r.GetById(req.Id))
                .Returns((Invoice)null!);

            var result = _service.EditInvoice(req);

            Assert.Equal(InvoiceEditResult.NotFound, result.Result);
        }

        [Fact]
        public void EditInvoice_Archived_ShouldReturnArchived()
        {
            var req = CreateValidUpdateRequest();

            _repoMock.Setup(r => r.GetById(req.Id))
                .Returns(new Invoice { Id = 1, IsInvoiceArchived = true });

            var result = _service.EditInvoice(req);

            Assert.Equal(InvoiceEditResult.InvoiceArchived, result.Result);
        }

        [Fact]
        public void ArchiveInvoice_Existing_ShouldReturnSuccess()
        {
            var invoice = new Invoice { Id = 1 };

            _repoMock.Setup(r => r.GetById(1))
                .Returns(invoice);

            var result = _service.ArchiveInvoice(1);

            Assert.Equal(ArchiveInvoiceResult.Success, result);
        }

        [Fact]
        public void FindInvoice_Existing_ShouldReturnInvoice()
        {
            var invoice = new Invoice { Id = 1 };

            _repoMock.Setup(r => r.GetById(1))
                .Returns(invoice);

            var result = _service.FindInvoice(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void GetAllInvoices_ShouldReturnAll()
        {
            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Invoice>
                {
                    new Invoice { Id = 1 },
                    new Invoice { Id = 2 }
                });

            var result = _service.GetAllInvoices();

            Assert.Equal(2, result.Count);
        }
    }
}