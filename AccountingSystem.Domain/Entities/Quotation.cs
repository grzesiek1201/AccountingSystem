using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Domain.Entities
{
    public class Quotation
    {
        public Quotation()
        {
            Status = QuotationStatus.Draft;
        }


        public int Id { get; set; }

        public string QuotationNumber { get; set; }


        public QuotationStatus Status { get; private set; }


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


        public bool IsQuotationArchived { get; private set; }


        public ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();



        public void Send()
        {
            if (Status != QuotationStatus.Draft)
                throw new InvalidOperationException(
                    "Only draft quotations can be sent.");

            Status = QuotationStatus.Sent;
        }



        public void Accept()
        {
            if (Status != QuotationStatus.Sent)
                throw new InvalidOperationException(
                    "Only sent quotations can be accepted.");

            Status = QuotationStatus.Accepted;
        }



        public void Reject()
        {
            if (Status != QuotationStatus.Sent)
                throw new InvalidOperationException(
                    "Only sent quotations can be rejected.");

            Status = QuotationStatus.Rejected;
        }



        public void Archive()
        {
            if (Status == QuotationStatus.Draft)
                throw new InvalidOperationException(
                    "Draft quotations cannot be archived.");

            IsQuotationArchived = true;
        }

        public void ConvertToOrder()
        {
            if (Status != QuotationStatus.Accepted)
                throw new InvalidOperationException(
                    "Only accepted quotation can be converted to order.");

            Status = QuotationStatus.Converted;
        }
    }



    public class QuotationItem
    {
        public int Id { get; set; }


        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public decimal VatRate { get; set; }

        public ProductUnit Unit { get; set; }


        public int QuotationId { get; set; }

        public Quotation Quotation { get; set; }


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