using AccountingSystem.Application.DTOs.Quotations;
using AccountingSystem.Application.Factories;
using AccountingSystem.Application.Helpers;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Mappers;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Quotations;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AccountingSystem.Application.Services
{
    public class QuotationService : IQuotationService
    {
        private readonly IQuotationRepository _quotationRepository;
        private readonly QuotationValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuotationService> _logger;
        private readonly QuotationFactory _quotationFactory;

        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly QuotationResponseMapper _mapper;


        public QuotationService(
            IQuotationRepository quotationRepository,
            QuotationValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<QuotationService> logger,
            QuotationFactory quotationFactory,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            QuotationResponseMapper mapper)
        {
            _quotationRepository = quotationRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _quotationFactory = quotationFactory;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }



        // ================= ADD =================

        public QuotationAddResponse AddQuotation(CreateQuotationRequest request)
        {
            var customer = _customerRepository.GetById(request.CustomerId);

            if (customer == null)
                return new QuotationAddResponse
                {
                    Result = QuotationAddResult.InvalidData
                };


            var productIds = request.Items?
                .Select(i => i.ProductId)
                .ToList() ?? new List<int>();


            var products = _productRepository
                .GetByIds(productIds)
                .ToDictionary(p => p.Id);



            var quotation = _quotationFactory.Create(
                request,
                customer,
                products);



            var validation =
                _validator.Validate(
                    quotation,
                    _quotationRepository.GetAll());



            if (!validation.IsValid)
                return new QuotationAddResponse
                {
                    Result = QuotationAddResult.InvalidData,
                    Errors = validation.Errors
                };


            _quotationRepository.Add(quotation);
            _unitOfWork.Save();

            return new QuotationAddResponse
            {
                Result = QuotationAddResult.Success,
                CreatedId = quotation.Id
            };
        }



        // ================= EDIT =================

        public QuotationEditResponse EditQuotation(UpdateQuotationRequest request)
        {
            var existing = _quotationRepository.GetById(request.Id);


            if (existing == null)
                return new QuotationEditResponse
                {
                    Result = QuotationEditResult.NotFound
                };


            if (existing.IsQuotationArchived)
                return new QuotationEditResponse
                {
                    Result = QuotationEditResult.QuotationArchived
                };



            if (request.Items != null && request.Items.Any())
            {
                var productIds = request.Items
                    .Select(i => i.ProductId)
                    .ToList();


                var products = _productRepository
                    .GetByIds(productIds)
                    .ToDictionary(p => p.Id);



                var domainItems = request.Items
                    .Select(x => new QuotationItem
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        DiscountPercent = x.DiscountPercent
                    })
                    .ToList();



                existing.Items =
                    ItemSnapshotHelper.SnapshotQuotationItems(
                        domainItems,
                        products);
            }



            if (request.CustomerId != 0 &&
                request.CustomerId != existing.CustomerId)
            {
                var customer =
                    _customerRepository.GetById(request.CustomerId);


                if (customer == null)
                    return new QuotationEditResponse
                    {
                        Result = QuotationEditResult.InvalidData
                    };


                existing.ApplyCustomerSnapshot(customer);
            }




            var validation =
                _validator.Validate(
                    existing,
                    _quotationRepository.GetAll()
                    .Where(x => x.Id != existing.Id)
                    .ToList(),
                    isEdit: true);



            if (!validation.IsValid)
                return new QuotationEditResponse
                {
                    Result = QuotationEditResult.InvalidData,
                    Errors = validation.Errors
                };



            _quotationRepository.Update(existing);
            _unitOfWork.Save();



            return new QuotationEditResponse
            {
                Result = QuotationEditResult.Success
            };
        }




        // ================= DOMAIN OPERATIONS =================


        public QuotationStatusResponse SendQuotation(int id)
        {
            var quotation = _quotationRepository.GetById(id);


            if (quotation == null)
                return new QuotationStatusResponse
                {
                    Result = QuotationStatusResult.NotFound
                };


            if (quotation.IsQuotationArchived)
                return new QuotationStatusResponse
                {
                    Result = QuotationStatusResult.InvalidOperation
                };


            try
            {
                quotation.Send();
            }
            catch (InvalidOperationException)
            {
                return new QuotationStatusResponse
                {
                    Result = QuotationStatusResult.InvalidOperation
                };
            }



            _quotationRepository.Update(quotation);
            _unitOfWork.Save();



            return new QuotationStatusResponse
            {
                Result = QuotationStatusResult.Success
            };
        }




        public QuotationStatusResponse AcceptQuotation(int id)
        {
            var quotation = _quotationRepository.GetById(id);


            if (quotation == null)
                return new QuotationStatusResponse
                {
                    Result = QuotationStatusResult.NotFound
                };


            try
            {
                quotation.Accept();
            }
            catch (InvalidOperationException)
            {
                return new QuotationStatusResponse
                {
                    Result = QuotationStatusResult.InvalidOperation
                };
            }



            _quotationRepository.Update(quotation);
            _unitOfWork.Save();



            return new QuotationStatusResponse
            {
                Result = QuotationStatusResult.Success
            };
        }




        public QuotationStatusResponse RejectQuotation(int id)
        {
            var quotation = _quotationRepository.GetById(id);


            if (quotation == null)
                return new QuotationStatusResponse
                {
                    Result = QuotationStatusResult.NotFound
                };


            try
            {
                quotation.Reject();
            }
            catch (InvalidOperationException)
            {
                return new QuotationStatusResponse
                {
                    Result = QuotationStatusResult.InvalidOperation
                };
            }



            _quotationRepository.Update(quotation);
            _unitOfWork.Save();



            return new QuotationStatusResponse
            {
                Result = QuotationStatusResult.Success
            };
        }




        // ================= READ =================

        public List<QuotationResponse> GetAllQuotations()
        {
            return _quotationRepository
                .GetAll()
                .Select(q => _mapper.Map(q))
                .ToList();
        }



        public QuotationResponse? FindQuotation(int id)
        {
            var quotation = _quotationRepository.GetById(id);

            return quotation == null
                ? null
                : _mapper.Map(quotation);
        }

        // ================= ARCHIVE =================

        public QuotationArchiveResult ArchiveQuotation(int id)
        {
            var existing = _quotationRepository.GetById(id);


            if (existing == null)
                return QuotationArchiveResult.NotFound;

            try
            {
                existing.Archive();
            }
            catch (InvalidOperationException)
            {
                return QuotationArchiveResult.InvalidOperation;
            }


            _quotationRepository.Update(existing);
            _unitOfWork.Save();

            return QuotationArchiveResult.Success;
        }
    }
}