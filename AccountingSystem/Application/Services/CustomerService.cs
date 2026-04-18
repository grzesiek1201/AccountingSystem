using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.Application.Services
{
    internal class CustomerService
    {

        private List<Customer> customers = new List<Customer>();
        public int nextId;

        public void AddCustomer(Customer customer)
        {

            customer.Id = nextId;
            nextId++;
            customers.Add(customer);
        }

        public Domain.Enums.CustomerEditResult EditCustomer(Customer customer)
        {
            var existing = customers.Find(x => x.Name == customer.Name);
            if (existing == null)
            {
                return Domain.Enums.CustomerEditResult.NotFound;
            }
            if (existing.IsArchived == true)
            {
                return Domain.Enums.CustomerEditResult.CustomerArchived;
            }
            existing.Name = customer.Name;
            existing.Address = customer.Address;
            existing.Wallet = customer.Wallet;
            existing.Email = customer.Email;
            return Domain.Enums.CustomerEditResult.Success;
        }

        public List<Customer> GetAllCustomers()
        {
            return customers;
        }


        public Customer FindCustomer(string Name)
        {
            var existing = customers.Find(x => x.Name == Name);
            if (existing != null)
            {
                return existing;
            }
            else
            {
                return null;
            }
        }

        public Domain.Enums.ArchiveCustomerResult ArchiveCustomer(string Name)
        {
            var existing = customers.Find(x => x.Name == Name);
            if (existing == null)
            {
                return Domain.Enums.ArchiveCustomerResult.NotFound;
            }

            if (existing.Wallet.InDebt == true)
            {
                return Domain.Enums.ArchiveCustomerResult.CustomerInDebt;
            }

            existing.IsArchived = true;
            return Domain.Enums.ArchiveCustomerResult.Success;
        }
    }
}
