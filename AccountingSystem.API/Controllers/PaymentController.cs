using AccountingSystem.Application.DTOs.Payments;
using AccountingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.API.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IPaymentService paymentService,
        ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    // ================= CREATE =================

    [HttpPost]
    public IActionResult Create(CreatePaymentRequest request)
    {
        _logger.LogInformation(
            "POST /api/payments InvoiceId={InvoiceId}, Amount={Amount}",
            request.InvoiceId,
            request.Amount);

        var result = _paymentService.AddPayment(request);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Payment create failed: {@Errors}", result.Errors);
            return BadRequest(result.Errors);
        }

        _logger.LogInformation("Payment created for invoice {InvoiceId}", request.InvoiceId);

        return Ok(result);
    }

    // ================= GET BY INVOICE =================

    [HttpGet("invoice/{invoiceId}")]
    public IActionResult GetForInvoice(int invoiceId)
    {
        _logger.LogInformation("GET /api/payments/invoice/{InvoiceId}", invoiceId);

        var payments = _paymentService.GetPaymentsForInvoice(invoiceId);

        return Ok(payments.Select(p => new PaymentResponse
        {
            Id = p.Id,
            InvoiceId = p.InvoiceId,
            Amount = p.Amount,
            PaymentDate = p.PaymentDate,
            Status = p.Status
        }));
    }

    // ================= DELETE =================

    [HttpDelete("{paymentId}")]
    public IActionResult Delete(int paymentId)
    {
        _logger.LogInformation("DELETE /api/payments/{PaymentId}", paymentId);

        var result = _paymentService.DeletePayment(paymentId);

        return result switch
        {
            PaymentDeleteResult.Success => NoContent(),
            PaymentDeleteResult.NotFound => NotFound(),
            _ => BadRequest()
        };
    }
}