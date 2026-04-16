using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace AccountingSystem.UI
{
    internal class MenuConsole
    {
        CustomerService _customerService;

        public void MainMenu()
        {
            bool isActiveMainMenu = true;
            _customerService = new CustomerService();


            while (isActiveMainMenu)
            {
                Console.WriteLine("1- Go to Customers Menu, 2- Go to Invoice Menu, 3- Go to Order Menu, 4- Go to Quantation Menu, 5- Go to Payment menu,\n s - to save, q -  to save and quit");
                Console.Write("Type your option: ");
                string inputChoiceMenu = Console.ReadLine();
                switch (inputChoiceMenu.Trim().ToLower())
                {
                    case "1":
                        CustomerMenu();
                        break;
                    case "2":
                        InvoiceMenu();
                        break;
                    case "3":
                        OrderMenu();
                        break;
                    case "4":
                        QuantationMenu();
                        break;
                    case "5":
                        PaymentMenu();
                        break;
                    case "s":

                        break;
                    case "q":
                        Console.WriteLine("Saving data and quitting...");
                        isActiveMainMenu = false;
                        break;
                    default:
                        Console.WriteLine("Invalid input. Try again");
                        break;

                }
            }

        }

        public void CustomerMenu()
        {
            bool isCustomerMenuActive = true;


            while (isCustomerMenuActive)
            {
                Console.WriteLine("1- to add client, 2- to edit client, 3- to show all clients name, 4- to search a client by name or ID, s - to save, q -  to save and quit");
                Console.Write("Type your option: ");
                string inputChoiceMenu = Console.ReadLine();
                switch (inputChoiceMenu.Trim().ToLower())
                {
                    case "1":
                        {
                            var customer = GetCustomerInput();
                            _customerService.AddClient(customer);
                        }
                        break;
                    case "2":
                        Console.WriteLine("To edit customer just fill fields below. If there is a customer matching your name or Id, data will change");
                        {
                            var customer = GetCustomerInput();
                            _customerService.EditClient(customer);
                        }
                        break;
                    case "3":
                            _customerService.ShowAllClients();
                        break;

                    case "4":
                        {
                            var customer = GetCustomerInput();
                            _customerService.ShowClient(customer);
                        }
                        break;
                    case "s":
                        break;

                    case "q":
                        isCustomerMenuActive = false;
                        break;

                    default:
                        Console.WriteLine("Invalid input. Try again");
                        break;
                }

            }
        }


        public void InvoiceMenu()
        {

        }


        public void OrderMenu()
        {

        }


        public void QuantationMenu()
        {

        }


        public void PaymentMenu()
        {

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
                Address = address
            };
            return customer;



        }




    }
}
