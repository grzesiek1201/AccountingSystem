using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Products
{
    public class ProductAddResponse
    {
        public ProductAddResult Result { get; set; }

        public int? Id { get; set; }

        public List<ProductValidationError> Errors { get; set; } = new();

        public bool IsSuccess => Result == ProductAddResult.Success;

        public int CreatedId { get; set; }
    }
}
