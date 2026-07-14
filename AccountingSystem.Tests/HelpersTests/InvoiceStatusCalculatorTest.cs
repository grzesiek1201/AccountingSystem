using AccountingSystem.Application.Helpers;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Xunit;

namespace AccountingSystem.Tests.HelpersTests
{
    public class InvoiceStatusCalculatorTests
    {
        private readonly InvoiceStatusCalculator _calculator;

        public InvoiceStatusCalculatorTests()
        {
            _calculator = new InvoiceStatusCalculator();
        }

        [Fact]
        public void Recalculate_ShouldSetOverdue_WhenInvoiceIsExpiredAndNotFullyPaid()
        {
            var invoice = new Invoice
            {
                TotalAmount = 100m,
                DueDate = DateTime.UtcNow.AddDays(-1),
            };

            _calculator.Recalculate(invoice, 50m);

            Assert.Equal(InvoiceStatus.Overdue, invoice.Status);
        }


        [Fact]
        public void Recalculate_ShouldSetIssued_WhenInvoiceIsFullyPaid()
        {
            var invoice = new Invoice
            {
                TotalAmount = 100m,
                DueDate = DateTime.UtcNow.AddDays(-1),
            };

            _calculator.Recalculate(invoice, 100m);

            Assert.Equal(InvoiceStatus.Issued, invoice.Status);
        }


        [Fact]
        public void Recalculate_ShouldSetIssued_WhenInvoiceIsBeforeDueDate()
        {
            var invoice = new Invoice
            {
                TotalAmount = 100m,
                DueDate = DateTime.UtcNow.AddDays(5),
            };

            _calculator.Recalculate(invoice, 0m);

            Assert.Equal(InvoiceStatus.Issued, invoice.Status);
        }


        [Fact]
        public void Recalculate_ShouldSetIssued_WhenPaidAmountIsGreaterThanTotalAmount()
        {
            var invoice = new Invoice
            {
                TotalAmount = 100m,
                DueDate = DateTime.UtcNow.AddDays(-5),
            };

            _calculator.Recalculate(invoice, 150m);

            Assert.Equal(InvoiceStatus.Issued, invoice.Status);
        }
    }
}