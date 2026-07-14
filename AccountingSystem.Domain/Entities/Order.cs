using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Domain.Entities
{
    public class Order
    {
        public Order()
        {
            Status = OrderStatus.Draft;
        }


        public int Id { get; set; }

        public string OrderNumber { get; set; }

        public int QuotationId { get; set; }

        public OrderStatus Status { get; private set; }


        public DateTime DateCreated { get; set; }


        public int CustomerId { get; set; }

        public Customer Customer { get; set; }


        // SNAPSHOT CUSTOMER
        public string CustomerName { get; set; }
        public string CustomerNIP { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerStreet { get; set; }
        public string CustomerZipCode { get; set; }
        public string CustomerCity { get; set; }


        public bool IsOrderArchived { get; private set; }


        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();


        public void Confirm()
        {
            if (Status != OrderStatus.Draft)
                throw new InvalidOperationException(
                    "Only draft orders can be confirmed.");

            Status = OrderStatus.Confirmed;
        }


        public void Complete()
        {
            if (Status != OrderStatus.Confirmed)
                throw new InvalidOperationException(
                    "Only confirmed orders can be completed.");

            if (!Items.Any())
                throw new InvalidOperationException(
                    "Order must contain items.");

            Status = OrderStatus.Completed;
        }


        public void Cancel()
        {
            if (Status == OrderStatus.Completed)
                throw new InvalidOperationException(
                    "Completed orders cannot be cancelled.");

            if (Status == OrderStatus.Cancelled)
                throw new InvalidOperationException(
                    "Order is already cancelled.");

            Status = OrderStatus.Cancelled;
        }


        public void Archive()
        {
            if (Status == OrderStatus.Draft)
                throw new InvalidOperationException(
                    "Draft orders cannot be archived.");

            IsOrderArchived = true;
        }

        public void ConvertToInvoice()
        {
            if (Status != OrderStatus.Completed)
                throw new InvalidOperationException(
                    "Only completed order can be converted to invoice.");

            Status = OrderStatus.Invoiced;
        }
    }


    public class OrderItem
    {
        public int Id { get; set; }


        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public decimal VatRate { get; set; }

        public ProductUnit Unit { get; set; }


        public int OrderId { get; set; }

        public Order Order { get; set; }


        public int ProductId { get; set; }

        public Product Product { get; set; }


        public int Position { get; set; }

        public int Quantity { get; set; }

        public decimal DiscountPercent { get; set; }


        // SNAPSHOT
        public decimal BaseUnitPrice { get; set; }

        public decimal Total { get; set; }
    }
}