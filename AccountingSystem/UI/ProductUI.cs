using AccountingSystem.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.UI
{
    internal class ProductUI
    {
        private ProductService _productService;

        public ProductUI(ProductService ProductService)
        {
            _productService = productService;
        }
    }
}
