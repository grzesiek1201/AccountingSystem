using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
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

        public void AddProductFlow()
        {
            var product = GetProductInput();

            var response = _productService.AddProduct(product);

            if (!response.IsSuccess)
            {
                foreach (var error in response.Errors)
                {
                    Console.WriteLine(error);
                }
                return;
            }

            Console.WriteLine("Customer added successfully");
        }

        public void EditProductFlow()
        {

        }

        public void ArchiveProductFlow()
        {

        }

        public void FindProductFlow()
        {

        }

        public void GetAllProductsFlow()
        {

        }

        public Product GetProductInput()
        {
            Console.Write("add name of the product: ");
            string name = Console.ReadLine();

            Console.Write("add price: ");
            string price = Console.ReadLine();
            if (decimal.TryParse(price, out var priceValue))

            Console.Write("add category: ");
            string group = Console.ReadLine();

            var category = new Category
            {
                Id = nextId,
                Name = group
            };

            return new Product
            {
                Name = name,
                Price = priceValue,
                Category = category
            };
        }

    }
}
