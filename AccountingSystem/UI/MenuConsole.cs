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
        

        public void MainMenu()
        {
            bool isActiveMainMenu = true;


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
            var customerService = new CustomerService();
            var customerUI = new CustomerUI(customerService);


            while (isCustomerMenuActive)
            {
                Console.WriteLine("1- to add client, 2- to edit client, 3- to show all clients name, 4- to search a client by name or ID, r - to return to main menu");
                Console.Write("Type your option: ");
                string inputChoiceMenu = Console.ReadLine();
                switch (inputChoiceMenu.Trim().ToLower())
                {
                    case "1":
                        {
                            customerUI.AddCustomerFlow();
                        }
                        break;

                    case "2":
                        {
                            customerUI.EditCustomerFlow();

                        }
                        break;

                    case "3":
                        {
                            customerUI.GetAllCustomerFlow();

                        }
                        break;

                    case "4":
                        {
                            customerUI.FindCustomerFlow();
                        }
                        break;

                    case "5":
                        {
                            customerUI.FindCustomerFlow();
                        }
                        break;

                    case "r":
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
    }
}

