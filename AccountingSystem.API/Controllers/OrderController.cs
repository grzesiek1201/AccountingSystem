using AccountingSystem.Application.DTOs.Orders;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            IOrderService orderService,
            ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // ================= GET ALL =================
        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.LogInformation("GET /api/orders");

            var orders = _orderService.GetAllOrders();

            return Ok(orders);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public IActionResult Find(int id)
        {
            _logger.LogInformation("GET /api/orders/{Id}", id);

            var order = _orderService.FindOrder(id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // ================= CREATE =================
        [HttpPost]
        public IActionResult Create(CreateOrderRequest request)
        {
            _logger.LogInformation("POST /api/orders CustomerId={CustomerId}", request.CustomerId);

            var result = _orderService.AddOrder(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Order create failed: {@Errors}", result.Errors);
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateOrderRequest request)
        {
            _logger.LogInformation("PUT /api/orders/{Id}", id);

            request.Id = id;

            var result = _orderService.EditOrder(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Order update failed {Id}: {@Errors}", id, result.Errors);
                return BadRequest(result.Errors);
            }

            var updated = _orderService.FindOrder(id);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // ================= ARCHIVE =================
        [HttpPatch("{id}/archive")]
        public IActionResult Archive(int id)
        {
            _logger.LogInformation("PATCH archive order {Id}", id);

            var result = _orderService.ArchiveOrder(id);

            if (result == ArchiveOrderResult.NotFound)
                return NotFound();

            return NoContent();
        }
    }
}