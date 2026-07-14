using AccountingSystem.Application.Converters;
using AccountingSystem.Application.DTOs.Orders;
using AccountingSystem.Application.Factories;
using AccountingSystem.Application.Helpers;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Mappers;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Orders;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AccountingSystem.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;
        private readonly OrderFactory _orderFactory;
        private readonly INumberSequenceService _numberSequenceService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly OrderResponseMapper _mapper;
        private readonly IQuotationRepository _quotationRepository;
        private readonly QuotationToOrderConverter _quotationToOrderMapper;


        public OrderService(
            IOrderRepository orderRepository,
            OrderValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<OrderService> logger,
            OrderFactory orderFactory,
            INumberSequenceService numberSequenceService,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            OrderResponseMapper mapper,
            IQuotationRepository quotationRepository,
            QuotationToOrderConverter quotationToOrderMapper)
        {
            _orderRepository = orderRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _orderFactory = orderFactory;
            _numberSequenceService = numberSequenceService;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _quotationRepository = quotationRepository;
            _quotationToOrderMapper = quotationToOrderMapper;
        }


        // ================= ADD =================

        public OrderAddResponse AddOrder(CreateOrderRequest request)
        {
            var customer = _customerRepository.GetById(request.CustomerId);

            if (customer == null)
                return new OrderAddResponse
                {
                    Result = OrderAddResult.InvalidData
                };


            var productIds = request.Items?
                .Select(i => i.ProductId)
                .ToList() ?? new List<int>();


            var products = _productRepository
                .GetByIds(productIds)
                .ToDictionary(p => p.Id);


            var order = _orderFactory.Create(
                request,
                customer,
                products);


            var validation = _validator.Validate(
                order,
                _orderRepository.GetAll());


            if (!validation.IsValid)
                return new OrderAddResponse
                {
                    Result = OrderAddResult.InvalidData,
                    Errors = validation.Errors
                };


            _orderRepository.Add(order);
            _unitOfWork.Save();


            return new OrderAddResponse
            {
                Result = OrderAddResult.Success
            };
        }



        // ================= CONVERT QUOTATION TO ORDER =================

        public OrderAddResponse CreateOrderFromQuotation(int quotationId)
        {
            var quotation = _quotationRepository.GetById(quotationId);


            if (quotation == null || quotation.IsQuotationArchived)
                return new OrderAddResponse
                {
                    Result = OrderAddResult.InvalidData
                };


            var order = _quotationToOrderMapper.Map(quotation);


            if (order == null)
                return new OrderAddResponse
                {
                    Result = OrderAddResult.InvalidData
                };


            order.OrderNumber =
                _numberSequenceService.GetNext(DocumentType.Order);

            order.DateCreated = DateTime.UtcNow;


            var validation =
                _validator.Validate(order, _orderRepository.GetAll());


            if (!validation.IsValid)
                return new OrderAddResponse
                {
                    Result = OrderAddResult.InvalidData,
                    Errors = validation.Errors
                };


            try
            {
                _unitOfWork.BeginTransaction();

                _orderRepository.Add(order);

                quotation.ConvertToOrder();

                _quotationRepository.Update(quotation);

                _unitOfWork.Save();

                _unitOfWork.Commit();

                return new OrderAddResponse
                {
                    Result = OrderAddResult.Success
                };
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();

                _logger.LogError(
                    ex,
                    "Error while converting quotation {QuotationId} to order",
                    quotationId);

                return new OrderAddResponse
                {
                    Result = OrderAddResult.InvalidData
                };
            }
        }



        // ================= EDIT =================

        public OrderEditResponse EditOrder(UpdateOrderRequest request)
        {
            var existing = _orderRepository.GetById(request.Id);


            if (existing == null)
                return new OrderEditResponse
                {
                    Result = OrderEditResult.NotFound
                };


            if (existing.IsOrderArchived)
                return new OrderEditResponse
                {
                    Result = OrderEditResult.OrderArchived
                };


            if (request.Items != null && request.Items.Any())
            {
                var domainItems = request.Items
                    .Select(x => new OrderItem
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        DiscountPercent = x.DiscountPercent
                    })
                    .ToList();


                var productIds =
                    domainItems.Select(i => i.ProductId).ToList();


                var products =
                    _productRepository
                    .GetByIds(productIds)
                    .ToDictionary(p => p.Id);


                existing.Items =
                    ItemSnapshotHelper.SnapshotOrderItems(
                        domainItems,
                        products);
            }


            if (request.CustomerId != 0 &&
                request.CustomerId != existing.CustomerId)
            {
                var customer =
                    _customerRepository.GetById(request.CustomerId);


                if (customer == null)
                    return new OrderEditResponse
                    {
                        Result = OrderEditResult.InvalidData
                    };


                existing.ApplyCustomerSnapshot(customer);
            }



            var validation =
                _validator.Validate(
                    existing,
                    _orderRepository.GetAll()
                    .Where(x => x.Id != existing.Id)
                    .ToList(),
                    isEdit: true);



            if (!validation.IsValid)
                return new OrderEditResponse
                {
                    Result = OrderEditResult.InvalidData
                };


            _orderRepository.Update(existing);
            _unitOfWork.Save();


            return new OrderEditResponse
            {
                Result = OrderEditResult.Success
            };
        }



        // ================= DOMAIN OPERATIONS =================


        public OrderStatusResponse ConfirmOrder(int id)
        {
            var order = _orderRepository.GetById(id);


            if (order == null)
                return new OrderStatusResponse
                {
                    Result = OrderStatusResult.NotFound
                };


            if (order.IsOrderArchived)
                return new OrderStatusResponse
                {
                    Result = OrderStatusResult.InvalidOperation
                };


            try
            {
                order.Confirm();
            }
            catch (InvalidOperationException)
            {
                return new OrderStatusResponse
                {
                    Result = OrderStatusResult.InvalidOperation
                };
            }


            _orderRepository.Update(order);
            _unitOfWork.Save();


            return new OrderStatusResponse
            {
                Result = OrderStatusResult.Success
            };
        }



        public OrderStatusResponse CompleteOrder(int id)
        {
            var order = _orderRepository.GetById(id);


            if (order == null)
                return new OrderStatusResponse
                {
                    Result = OrderStatusResult.NotFound
                };


            try
            {
                order.Complete();
            }
            catch (InvalidOperationException)
            {
                return new OrderStatusResponse
                {
                    Result = OrderStatusResult.InvalidOperation
                };
            }


            _orderRepository.Update(order);
            _unitOfWork.Save();


            return new OrderStatusResponse
            {
                Result = OrderStatusResult.Success
            };
        }



        public OrderStatusResponse CancelOrder(int id)
        {
            var order = _orderRepository.GetById(id);


            if (order == null)
                return new OrderStatusResponse
                {
                    Result = OrderStatusResult.NotFound
                };


            try
            {
                order.Cancel();
            }
            catch (InvalidOperationException)
            {
                return new OrderStatusResponse
                {
                    Result = OrderStatusResult.InvalidOperation
                };
            }


            _orderRepository.Update(order);
            _unitOfWork.Save();


            return new OrderStatusResponse
            {
                Result = OrderStatusResult.Success
            };
        }



        // ================= READ =================

        public List<OrderResponse> GetAllOrders()
        {
            return _orderRepository
                .GetAll()
                .Select(o => _mapper.Map(o))
                .ToList();
        }



        public OrderResponse? FindOrder(int id)
        {
            var order = _orderRepository.GetById(id);

            return order == null
                ? null
                : _mapper.Map(order);
        }



        // ================= ARCHIVE =================

        public ArchiveOrderResult ArchiveOrder(int id)
        {
            var existing = _orderRepository.GetById(id);


            if (existing == null)
                return ArchiveOrderResult.NotFound;


            try
            {
                existing.Archive();
            }
            catch (InvalidOperationException)
            {
                return ArchiveOrderResult.InvalidOperation;
            }


            _orderRepository.Update(existing);
            _unitOfWork.Save();


            return ArchiveOrderResult.Success;
        }
    }
}