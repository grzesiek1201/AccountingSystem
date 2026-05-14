using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.Application.Validation.Products
{
    class ProductValidationResult
    {
        public List<ProductValidationError> Errors { get; set; } = new List<ProductValidationError>();
        public bool IsValid => Errors.Count == 0;

    }
}
