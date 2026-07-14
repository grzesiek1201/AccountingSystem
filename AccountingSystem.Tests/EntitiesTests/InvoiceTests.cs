using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Tests.EntitiesTests
{
    public class InvoiceTests
    {
        [Fact]
        public void NewInvoice_ShouldHaveDraftStatus()
        {
            var invoice = new Invoice();

            Assert.Equal(
                InvoiceStatus.Draft,
                invoice.Status);
        }


        [Fact]
        public void Issue_DraftInvoice_ShouldChangeStatusToIssued()
        {
            var invoice = new Invoice();

            invoice.Issue();

            Assert.Equal(
                InvoiceStatus.Issued,
                invoice.Status);
        }


        [Fact]
        public void Issue_AlreadyIssuedInvoice_ShouldThrowException()
        {
            var invoice = new Invoice();

            invoice.Issue();

            Assert.Throws<InvalidOperationException>(
                () => invoice.Issue());
        }


        [Fact]
        public void Cancel_IssuedInvoice_ShouldChangeStatusToCancelled()
        {
            var invoice = new Invoice();

            invoice.Issue();

            invoice.Cancel();

            Assert.Equal(
                InvoiceStatus.Cancelled,
                invoice.Status);
        }


        [Fact]
        public void Cancel_AlreadyCancelledInvoice_ShouldThrowException()
        {
            var invoice = new Invoice();

            invoice.Cancel();

            Assert.Throws<InvalidOperationException>(
                () => invoice.Cancel());
        }
    }
}