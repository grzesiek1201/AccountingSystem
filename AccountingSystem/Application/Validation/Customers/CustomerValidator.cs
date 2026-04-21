using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccountingSystem.Application.Validation.Customers
{
    internal class CustomerValidator
    {

        public CustomerValidationResult Validate(Customer customer, List<Customer> customers)
        {
            var result = new CustomerValidationResult();


            if (string.IsNullOrWhiteSpace(customer.Name))
                result.Errors.Add(CustomerValidationError.EmptyName);

            else
            {
                if (customer.Name.Length > 64)
                    result.Errors.Add(CustomerValidationError.NameTooLong);

                if (customers.Exists(x => x.Name == customer.Name && x.Id != customer.Id))
                    result.Errors.Add(CustomerValidationError.DuplicateName);
            }

            if (string.IsNullOrWhiteSpace(customer.Email))
                result.Errors.Add(CustomerValidationError.EmptyEmail);
            else
            {
                if (customer.Email.Length > 64)
                    result.Errors.Add(CustomerValidationError.EmailTooLong);

                if (customer.Email.Count(c => c == '@') != 1 ||
                    customer.Email.StartsWith("@") ||
                    customer.Email.EndsWith("@") ||
                    !customer.Email.Contains("."))
                {
                    result.Errors.Add(CustomerValidationError.InvalidEmail);
                }
            }
   
            if (customer.Address == null)
            {
                result.Errors.Add(CustomerValidationError.AddressNull);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(customer.Address.ZipCode))
                    result.Errors.Add(CustomerValidationError.EmptyZipCode);

                else
                {
                    if (customer.Address.ZipCode.Length != 5)
                        result.Errors.Add(CustomerValidationError.InvalidZipCode);

                    if (!customer.Address.ZipCode.All(char.IsDigit))
                        result.Errors.Add(CustomerValidationError.NotDigitsZipCode);
                }

                if (string.IsNullOrWhiteSpace(customer.Address.City))
                    result.Errors.Add(CustomerValidationError.EmptyCity);
                else
                {
                    if (customer.Address.City.Length > 50)
                        result.Errors.Add(CustomerValidationError.CityTooLong);
                }
                if (string.IsNullOrWhiteSpace(customer.Address.Street))
                    result.Errors.Add(CustomerValidationError.EmptyStreet);
                else
                {
                    if (customer.Address.Street.Length > 50)
                        result.Errors.Add(CustomerValidationError.StreetTooLong);
                }
            }
            return result;
        }
    }
}
