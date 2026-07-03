using AccountingSystem.Application.DTOs.ProductCategories;
using AccountingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.API.Controllers;

[ApiController]
[Route("api/product-categories")]
public class ProductCategoriesController : ControllerBase
{
    private readonly IProductCategoryService _productCategoryService;
    private readonly ILogger<ProductCategoriesController> _logger;

    public ProductCategoriesController(
        IProductCategoryService productCategoryService,
        ILogger<ProductCategoriesController> logger)
    {
        _productCategoryService = productCategoryService;
        _logger = logger;
    }

    // ================= GET ALL =================

    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("GET /api/product-categories");

        var categories = _productCategoryService.GetAllProductCategories();

        _logger.LogInformation("Product categories count: {Count}", categories.Count);

        return Ok(categories.Select(c => new ProductCategoryResponse
        {
            Id = c.Id,
            Name = c.Name,
            IsActive = c.IsActive
        }));
    }

    // ================= GET BY ID =================

    [HttpGet("{id}")]
    public IActionResult Find(int id)
    {
        _logger.LogInformation("GET /api/product-categories/{Id}", id);

        var category = _productCategoryService.GetProductCategoryById(id);

        if (category == null)
        {
            _logger.LogWarning("Product category not found: {Id}", id);
            return NotFound();
        }

        return Ok(new ProductCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            IsActive = category.IsActive
        });
    }

    // ================= CREATE =================

    [HttpPost]
    public IActionResult Create(CreateProductCategoryRequest request)
    {
        _logger.LogInformation("POST product category {Name}", request.Name);

        var result = _productCategoryService.AddProductCategory(request);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Product category create failed: {@Errors}", result.Errors);
            return BadRequest(result.Errors);
        }

        _logger.LogInformation("Product category created");

        return Ok(result);
    }

    // ================= UPDATE =================

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateProductCategoryRequest request)
    {
        _logger.LogInformation("PUT product category {Id}", id);

        request.Id = id;

        var result = _productCategoryService.EditProductCategory(request);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Product category update failed {Id}", id);
            return BadRequest(result.Errors);
        }

        _logger.LogInformation("Product category updated: {Id}", id);

        return Ok(result);
    }
}