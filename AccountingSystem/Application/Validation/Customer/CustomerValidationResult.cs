using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccountingSystem.Application.Validation.Customer
{
    class CustomerValidationResult
    {
        List<CustomerValidationError> customerErrors = new List<CustomerValidationError>();
        bool IsValid => customerErrors.Count == 0;

    }
}
