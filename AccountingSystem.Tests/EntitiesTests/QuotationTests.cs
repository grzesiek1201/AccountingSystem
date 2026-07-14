using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Xunit;

namespace AccountingSystem.Tests.DomainTests
{
    public class QuotationTests
    {
        [Fact]
        public void NewQuotation_ShouldHaveDraftStatus()
        {
            var quotation = new Quotation();

            Assert.Equal(
                QuotationStatus.Draft,
                quotation.Status);
        }


        [Fact]
        public void Send_DraftQuotation_ShouldChangeStatusToSent()
        {
            var quotation = new Quotation();

            quotation.Send();

            Assert.Equal(
                QuotationStatus.Sent,
                quotation.Status);
        }


        [Fact]
        public void Accept_SentQuotation_ShouldChangeStatusToAccepted()
        {
            var quotation = new Quotation();

            quotation.Send();

            quotation.Accept();

            Assert.Equal(
                QuotationStatus.Accepted,
                quotation.Status);
        }


        [Fact]
        public void Reject_SentQuotation_ShouldChangeStatusToRejected()
        {
            var quotation = new Quotation();

            quotation.Send();

            quotation.Reject();

            Assert.Equal(
                QuotationStatus.Rejected,
                quotation.Status);
        }


        [Fact]
        public void Accept_DraftQuotation_ShouldThrowException()
        {
            var quotation = new Quotation();

            Assert.Throws<InvalidOperationException>(
                () => quotation.Accept());
        }
    }
}