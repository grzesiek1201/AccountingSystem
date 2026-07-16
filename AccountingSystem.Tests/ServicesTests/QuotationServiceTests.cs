using AccountingSystem.Application.DTOs.Quotations;
using AccountingSystem.Application.Factories;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Mappers;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Quotations;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AccountingSystem.Tests.ServicesTests
{
    public class QuotationServiceTests
    {
        private readonly Mock<IQuotationRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILogger<QuotationService>> _loggerMock;
        private readonly Mock<INumberSequenceService> _seqMock;
        private readonly Mock<ICustomerRepository> _customerRepo;
        private readonly Mock<IProductRepository> _productRepo;

        private readonly QuotationFactory _factory;
        private readonly QuotationResponseMapper _mapper;

        private readonly QuotationValidator _validator;
        private readonly QuotationService _service;

        public QuotationServiceTests()
        {
            _repoMock = new Mock<IQuotationRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<QuotationService>>();
            _seqMock = new Mock<INumberSequenceService>();

            _seqMock.Setup(x => x.GetNext(It.IsAny<DocumentType>()))
                .Returns("Q-2026-0001");

            _factory = new QuotationFactory(_seqMock.Object);

            _customerRepo = new Mock<ICustomerRepository>();
            _productRepo = new Mock<IProductRepository>();

            _mapper = new QuotationResponseMapper();
            _validator = new QuotationValidator();

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

            _service = new QuotationService(
                _repoMock.Object,
                _validator,
                _uowMock.Object,
                _loggerMock.Object,
                _factory,
                _customerRepo.Object,
                _productRepo.Object,
                _mapper
            );
        }

        private CreateQuotationRequest CreateValidRequest()
        {
            return new CreateQuotationRequest
            {
                CustomerId = 1,
                Items = new List<CreateQuotationItemRequest>
                {
                    new CreateQuotationItemRequest
                    {
                        ProductId = 1,
                        Quantity = 2,
                        DiscountPercent = 0
                    }
                }
            };
        }

        private UpdateQuotationRequest CreateValidUpdateRequest()
        {
            return new UpdateQuotationRequest
            {
                Id = 1,
                CustomerId = 1,
                Status = QuotationStatus.Draft,
                Items = new List<UpdateQuotationItemRequest>
                {
                    new UpdateQuotationItemRequest
                    {
                        ProductId = 1,
                        Quantity = 3,
                        DiscountPercent = 0
                    }
                }
            };
        }

        [Fact]
        public void AddQuotation_Valid_ShouldReturnSuccess()
        {
            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Quotation>());

            var result = _service.AddQuotation(CreateValidRequest());

            Assert.Equal(QuotationAddResult.Success, result.Result);
        }

        [Fact]
        public void AddQuotation_Invalid_ShouldReturnInvalidData()
        {
            var req = CreateValidRequest();
            req.Items = new List<CreateQuotationItemRequest>();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Quotation>());

            var result = _service.AddQuotation(req);

            Assert.Equal(QuotationAddResult.InvalidData, result.Result);
        }

        [Fact]
        public void EditQuotation_Valid_ShouldReturnSuccess()
        {
            var quotation = new Quotation
            {
                Id = 1,
                CustomerId = 1,
                Items = new List<QuotationItem>()
            };

            var req = CreateValidUpdateRequest();

            _repoMock.Setup(r => r.GetById(req.Id))
                .Returns(quotation);

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Quotation>());

            var result = _service.EditQuotation(req);

            Assert.Equal(QuotationEditResult.Success, result.Result);
        }

        [Fact]
        public void EditQuotation_NotFound_ShouldReturnNotFound()
        {
            var req = CreateValidUpdateRequest();

            _repoMock.Setup(r => r.GetById(req.Id))
                .Returns((Quotation)null!);

            var result = _service.EditQuotation(req);

            Assert.Equal(QuotationEditResult.NotFound, result.Result);
        }

        [Fact]
        public void EditQuotation_Archived_ShouldReturnQuotationArchived()
        {
            var req = CreateValidUpdateRequest();

            var quotation = new Quotation
            {
                Id = 1
            };

            quotation.Send();
            quotation.Archive();

            _repoMock.Setup(r => r.GetById(req.Id))
                .Returns(quotation);

            var result = _service.EditQuotation(req);

            Assert.Equal(QuotationEditResult.QuotationArchived, result.Result);
        }

        [Fact]
        public void ArchiveQuotation_Existing_ShouldReturnSuccess()
        {
            var quotation = new Quotation
            {
                Id = 1
            };

            quotation.Send();

            _repoMock.Setup(r => r.GetById(1))
                .Returns(quotation);

            var result = _service.ArchiveQuotation(1);

            Assert.Equal(QuotationArchiveResult.Success, result);
        }

        [Fact]
        public void FindQuotation_Existing_ShouldReturnQuotation()
        {
            var quotation = new Quotation { Id = 1 };

            _repoMock.Setup(r => r.GetById(1))
                .Returns(quotation);

            var result = _service.FindQuotation(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void FindQuotation_NotExisting_ShouldReturnNull()
        {
            _repoMock.Setup(r => r.GetById(1))
                .Returns((Quotation)null!);

            var result = _service.FindQuotation(1);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllQuotations_ShouldReturnAll()
        {
            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Quotation>
                {
                    new Quotation { Id = 1 },
                    new Quotation { Id = 2 }
                });

            var result = _service.GetAllQuotations();

            Assert.Equal(2, result.Count);
        }
    }
}