using AccountingSystem.Application.DTOs.Quotations;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.API.Controllers;

[ApiController]
[Route("api/quotations")]
public class QuotationsController : ControllerBase
{
    private readonly IQuotationService _quotationService;
    private readonly ILogger<QuotationsController> _logger;

    public QuotationsController(
        IQuotationService quotationService,
        ILogger<QuotationsController> logger)
    {
        _quotationService = quotationService;
        _logger = logger;
    }

    // ================= GET ALL =================
    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("GET /api/quotations");

        var quotations = _quotationService.GetAllQuotations();

        return Ok(quotations);
    }

    // ================= GET BY ID =================
    [HttpGet("{id}")]
    public IActionResult Find(int id)
    {
        _logger.LogInformation("GET /api/quotations/{Id}", id);

        var quotation = _quotationService.FindQuotation(id);

        if (quotation == null)
            return NotFound();

        return Ok(quotation);
    }

    // ================= CREATE =================
    [HttpPost]
    public IActionResult Create(CreateQuotationRequest request)
    {
        _logger.LogInformation("POST /api/quotations CustomerId={CustomerId}", request.CustomerId);

        var result = _quotationService.AddQuotation(request);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Quotation create failed: {@Errors}", result.Errors);
            return BadRequest(result.Errors);
        }

        return CreatedAtAction(nameof(Find), new { id = result.CreatedId }, null);
    }

    // ================= UPDATE =================
    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateQuotationRequest request)
    {
        _logger.LogInformation("PUT /api/quotations/{Id}", id);

        var result = _quotationService.EditQuotation(request);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Quotation update failed {Id}: {@Errors}", id, result.Errors);
            return BadRequest(result.Errors);
        }

        var updated = _quotationService.FindQuotation(id);

        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    // ================= ARCHIVE =================
    [HttpPatch("{id}/archive")]
    public IActionResult Archive(int id)
    {
        _logger.LogInformation("PATCH archive quotation {Id}", id);

        var result = _quotationService.ArchiveQuotation(id);

        if (result == QuotationArchiveResult.NotFound)
            return NotFound();

        return NoContent();
    }
}