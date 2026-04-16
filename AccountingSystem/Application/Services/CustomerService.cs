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

        public void AddClient(Customer customer)
        {

            customer.Id = nextId;
            nextId ++;
            customers.Add(customer);
        }

        public Domain.Enums.CustomerEditResult EditClient(Customer customer)
        {
            var existing = customers.Find(x => x.Name == customer.Name);
            if (existing != null)
            {
                existing.Name = customer.Name;
                existing.Address = customer.Address;
                return Domain.Enums.CustomerEditResult.Success;
            }
            else
            {
                return Domain.Enums.CustomerEditResult.NotFound;
            }
        }

        public void ShowAllClients()
        {
            foreach (var Customer in customers)
            {
                Console.WriteLine($"Name: {Customer.Name}(Id: {Customer.Id})");
            }
        }


        public void ShowClient(Customer customer)
        {
            var existing = customers.Find(x => x.Name == customer.Name);
            if (existing != null)
            {
                Console.WriteLine($"Name: {customer.Name} Id: {customer.Id} Address: {customer.Address} Wallet: {customer.Wallet} ");
            }
            else
            {
                Console.WriteLine("Customer not found. Try again");
            }
        }

        public void ArchiveClient(Customer customer)
        {
            var existing = customers.Find(x => x.Name == customer.Name);
            if (existing != null)
            {
                if(existing.Wallet.InDebt == false)
                {
                    existing.IsArchived = true;
                }
                else
                {
                    Console.WriteLine("Customer didn't pay all debt. Can't archive.");
                }

            }
            else
            {
                Console.WriteLine("Customer not found. Try again");
            }

        }
    }
}
