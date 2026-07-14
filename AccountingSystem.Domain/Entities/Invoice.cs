using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Domain.Entities
{
    public class Invoice
    {
        public Invoice()
        {
            Status = InvoiceStatus.Draft;
        }
        public int Id { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public int OrderId { get; set; }

        public InvoiceStatus Status { get; private set; }

        public DateTime DateCreated { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime DueDate { get; set; }

        public decimal TotalAmount { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        // SNAPSHOT CUSTOMER
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerNIP { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerStreet { get; set; } = string.Empty;
        public string CustomerZipCode { get; set; } = string.Empty;
        public string CustomerCity { get; set; } = string.Empty;

        public bool IsInvoiceArchived { get; set; }

        public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public void Issue()
        {
            if (Status != InvoiceStatus.Draft)
                throw new InvalidOperationException();

            Status = InvoiceStatus.Issued;
        }

        public void Cancel()
        {
            if (Status == InvoiceStatus.Cancelled)
                throw new InvalidOperationException();

            Status = InvoiceStatus.Cancelled;
        }

        public void MarkAsOverdue()
        {
            if (Status != InvoiceStatus.Issued)
                throw new InvalidOperationException();

            Status = InvoiceStatus.Overdue;
        }

        public void Archive()
        {
            if (Status == InvoiceStatus.Draft)
                throw new InvalidOperationException(
                    "Draft invoice cannot be archived.");

            IsInvoiceArchived = true;
        }
    }

    public class InvoiceItem
    {
        public int Id { get; set; }

        public  string ProductName { get; set; } = string.Empty;
        public  string ProductCode { get; set; } = string.Empty;
        public decimal VatRate { get; set; }
        public ProductUnit Unit { get; set; }


        public int InvoiceId { get; set; }

        public Invoice Invoice { get; set; } = null!;

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public int Position { get; set; }

        public int Quantity { get; set; }

        public decimal DiscountPercent { get; set; }

        // SNAPSHOT
        public decimal BaseUnitPrice { get; set; }

        public decimal Total { get; set; }
    }
}