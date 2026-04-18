using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace AccountingSystem.UI
{
    internal class CustomerUI
    {
        CustomerService _customerService;
        public CustomerUI(CustomerService customerService)
        {
            _customerService = customerService;

        }

        public void AddCustomerFlow()
        {
            var customer = GetCustomerInput();
            _customerService.AddCustomer(customer);
        }

        public void EditCustomerFlow()
        {
            Console.WriteLine("To edit customer just fill fields below. If there is a customer matching your name or Id, data will change");
            var customer = GetCustomerInput();
            _customerService.EditCustomer(customer);
        }

        public void GetAllCustomerFlow()
        {
            List<Customer> customers = _customerService.GetAllCustomers();
            foreach (var c in customers)
            {
                Console.WriteLine($"Name: {c.Name}, Id: {c.Id}, Email: {c.Email},\nZip Code: {c.Address.ZipCode}, Street: {c.Address.Street}, City: {c.Address.City},\nWallet: {c.Wallet} ");
            }
        }

        public void FindCustomerFlow()
        {
            string name = GetCustomerName();
            var result = _customerService.FindCustomer(name);
            if (result != null)
            {
                Console.WriteLine($"Name: {result.Name} Id: {result.Id},Email: {result.Email},\nZip Code: {result.Address.ZipCode}, Street: {result.Address.Street}, City: {result.Address.City},\nWallet: {result.Wallet} ");
            }
            else
            {
                Console.WriteLine("Customer not found. Try again");
            }
        }

        public void ArchiveCustomerFlow()
        {
            var customer = GetCustomerName();
            var result = _customerService.ArchiveCustomer(customer);
            if (result == Domain.Enums.ArchiveCustomerResult.NotFound)
            {
                Console.WriteLine("Customer is not found. Try again");
            }
            else if (result == Domain.Enums.ArchiveCustomerResult.CustomerInDebt)
            {
                Console.WriteLine("Customer didn't pay all debts. Only customers without debt can be archived\n");
            }
            else if (result == Domain.Enums.ArchiveCustomerResult.Success)
            {
                Console.WriteLine($"Customer has been archived and won't be longer available.");
            }

        }

        public Customer GetCustomerInput()
        {
            Console.Write("add name of the company/client: ");
            string name = Console.ReadLine();
            Console.Write("add zip code of the company/client: ");
            string zip = Console.ReadLine();
            Console.Write("add city of the company/client: ");
            string city = Console.ReadLine();
            Console.Write("add street of the company/client: ");
            string street = Console.ReadLine();

            var address = new Domain.Entities.Address
            {
                ZipCode = zip,
                City = city,
                Street = street
            };

            var customer = new Domain.Entities.Customer
            {
                Name = name,
                Address = address,
            };

            return customer;
        }

        public String GetCustomerName()
        {
            Console.WriteLine("Type the name of the customer to search: ");
            string searchInput = Console.ReadLine();
            return searchInput;
        }
    }
}
