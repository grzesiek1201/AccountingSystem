using AccountingSystem.Application.DTOs.ProductCategories;

namespace AccountingSystem.Application.Interfaces
{
    public interface IProductCategoryService
    {
        ProductCategoryAddResponse AddProductCategory(CreateProductCategoryRequest request);

        ProductCategoryEditResponse EditProductCategory(UpdateProductCategoryRequest request);

        List<ProductCategoryResponse> GetAllProductCategories();

        ProductCategoryResponse? GetProductCategoryById(int id);

        ProductCategoryStatusResponse ChangeProductCategoryStatus(int id, bool isActive);
    }
}