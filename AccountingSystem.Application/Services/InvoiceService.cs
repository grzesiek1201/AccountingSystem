using AccountingSystem.Application.Converters;
using AccountingSystem.Application.DTOs.Invoices;
using AccountingSystem.Application.Factories;
using AccountingSystem.Application.Helpers;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Mappers;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Invoices;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AccountingSystem.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly InvoiceValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InvoiceService> _logger;
        private readonly InvoiceFactory _invoiceFactory;
        private readonly INumberSequenceService _numberSequenceService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly InvoiceResponseMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly OrderToInvoiceConverter _orderToInvoiceMapper;
        private readonly IInvoiceStatusCalculator _statusCalculator;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IPaymentRepository paymentRepository,
            InvoiceValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<InvoiceService> logger,
            InvoiceFactory invoiceFactory,
            INumberSequenceService numberSequenceService,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            InvoiceResponseMapper mapper,
            IOrderRepository orderRepository,
            OrderToInvoiceConverter orderToInvoiceMapper,
            IInvoiceStatusCalculator statusCalculator)
        {
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _invoiceFactory = invoiceFactory;
            _numberSequenceService = numberSequenceService;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _orderToInvoiceMapper = orderToInvoiceMapper;
            _statusCalculator = statusCalculator;
        }

        public InvoiceAddResponse AddInvoice(CreateInvoiceRequest request)
        {
            var customer = _customerRepository.GetById(request.CustomerId);
            if (customer == null)
                return new InvoiceAddResponse { Result = InvoiceAddResult.InvalidData };

            var productIds = request.Items?
                .Select(i => i.ProductId)
                .ToList() ?? new List<int>();

            var products = _productRepository
                .GetByIds(productIds)
                .ToDictionary(p => p.Id);

            var invoice = _invoiceFactory.Create(
                request,
                customer,
                products);
        
            var validation = _validator.Validate(
                invoice,
                _invoiceRepository.GetAll());

            if (!validation.IsValid)
                return new InvoiceAddResponse
                {
                    Result = InvoiceAddResult.InvalidData,
                    Errors = validation.Errors
                };

            _invoiceRepository.Add(invoice);
            _unitOfWork.Save();

            var totalPaid = _paymentRepository.GetTotalPaidForInvoice(invoice.Id);
            _statusCalculator.Recalculate(invoice, totalPaid);

            _logger.LogInformation("Invoice created: {Id}", invoice.Id);

            return new InvoiceAddResponse { Result = InvoiceAddResult.Success };
        }

        public InvoiceAddResponse CreateInvoiceFromOrder(int orderId)
        {
            var order = _orderRepository.GetById(orderId);

            if (order == null || order.IsOrderArchived)
                return new InvoiceAddResponse { Result = InvoiceAddResult.InvalidData };

            var invoice = _orderToInvoiceMapper.Map(order);

            if (invoice == null)
                return new InvoiceAddResponse { Result = InvoiceAddResult.InvalidData };

            invoice.InvoiceNumber = _numberSequenceService.GetNext(DocumentType.Invoice);
            invoice.Status = InvoiceStatus.Draft;
            invoice.DateCreated = DateTime.UtcNow;

            var validation = _validator.Validate(invoice, _invoiceRepository.GetAll());

            if (!validation.IsValid)
                return new InvoiceAddResponse
                {
                    Result = InvoiceAddResult.InvalidData,
                    Errors = validation.Errors
                };

            _invoiceRepository.Add(invoice);
            _unitOfWork.Save();

            var totalPaid = _paymentRepository.GetTotalPaidForInvoice(invoice.Id);
            _statusCalculator.Recalculate(invoice, totalPaid);

            return new InvoiceAddResponse { Result = InvoiceAddResult.Success };
        }

        public InvoiceEditResponse EditInvoice(UpdateInvoiceRequest request)
        {
            var existing = _invoiceRepository.GetById(request.Id);

            if (existing == null)
                return new InvoiceEditResponse { Result = InvoiceEditResult.NotFound };

            if (existing.IsInvoiceArchived)
                return new InvoiceEditResponse { Result = InvoiceEditResult.InvoiceArchived };

            if (request.Items != null && request.Items.Any())
            {
                var domainItems = request.Items.Select(x => new InvoiceItem
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    DiscountPercent = x.DiscountPercent
                }).ToList();

                var productIds = domainItems.Select(i => i.ProductId).ToList();
                var products = _productRepository.GetByIds(productIds).ToDictionary(p => p.Id);

                existing.Items = ItemSnapshotHelper.SnapshotInvoiceItems(domainItems, products);
            }

            if (request.CustomerId != 0 && request.CustomerId != existing.CustomerId)
            {
                var customer = _customerRepository.GetById(request.CustomerId);
                if (customer == null)
                    return new InvoiceEditResponse { Result = InvoiceEditResult.InvalidData };

                existing.ApplyCustomerSnapshot(customer);
            }

            if (request.Status != default)
                existing.Status = request.Status;

            var validation = _validator.Validate(
                existing,
                _invoiceRepository.GetAll().Where(x => x.Id != existing.Id).ToList(),
                isEdit: true);

            if (!validation.IsValid)
                return new InvoiceEditResponse { Result = InvoiceEditResult.InvalidData };

            _invoiceRepository.Update(existing);
            _unitOfWork.Save();

            var totalPaid = _paymentRepository.GetTotalPaidForInvoice(existing.Id);
            _statusCalculator.Recalculate(existing, totalPaid);

            return new InvoiceEditResponse { Result = InvoiceEditResult.Success };
        }

        public InvoiceStatusResponse ChangeInvoiceStatus(int id, StatusInvoiceRequest request)
        {
            var invoice = _invoiceRepository.GetById(id);

            if (invoice == null)
                return new InvoiceStatusResponse { Result = InvoiceStatusResult.NotFound };

            if (invoice.IsInvoiceArchived)
                return new InvoiceStatusResponse { Result = InvoiceStatusResult.InvalidOperation };

            if (invoice.Status == InvoiceStatus.Overdue)
                _logger.LogWarning("Invoice {Id} is overdue. Manual override.", id);

            invoice.Status = request.Status;

            _invoiceRepository.Update(invoice);
            _unitOfWork.Save();

            return new InvoiceStatusResponse { Result = InvoiceStatusResult.Success };
        }

        public List<InvoiceResponse> GetAllInvoices()
            => _invoiceRepository.GetAll().Select(_mapper.Map).ToList();

        public InvoiceResponse? FindInvoice(int id)
        {
            var invoice = _invoiceRepository.GetById(id);
            return invoice == null ? null : _mapper.Map(invoice);
        }

        public ArchiveInvoiceResult ArchiveInvoice(int id)
        {
            var existing = _invoiceRepository.GetById(id);

            if (existing == null)
                return ArchiveInvoiceResult.NotFound;

            existing.IsInvoiceArchived = true;

            _invoiceRepository.Update(existing);
            _unitOfWork.Save();

            return ArchiveInvoiceResult.Success;
        }
    }
}