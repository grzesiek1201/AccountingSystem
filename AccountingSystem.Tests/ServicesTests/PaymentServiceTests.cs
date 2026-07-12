using AccountingSystem.Application.DTOs.Payments;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AccountingSystem.Tests.ServicesTests
{
    public class PaymentServiceTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepoMock;
        private readonly Mock<IInvoiceRepository> _invoiceRepoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILogger<PaymentService>> _loggerMock;
        private readonly Mock<IInvoiceStatusCalculator> _statusCalculatorMock;

        private readonly PaymentService _service;


        public PaymentServiceTests()
        {
            _paymentRepoMock = new Mock<IPaymentRepository>();
            _invoiceRepoMock = new Mock<IInvoiceRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<PaymentService>>();
            _statusCalculatorMock = new Mock<IInvoiceStatusCalculator>();


            _service = new PaymentService(
                _paymentRepoMock.Object,
                _invoiceRepoMock.Object,
                _uowMock.Object,
                _loggerMock.Object,
                _statusCalculatorMock.Object
            );
        }


        private CreatePaymentRequest CreateRequest(decimal amount = 100)
        {
            return new CreatePaymentRequest
            {
                InvoiceId = 1,
                Amount = amount
            };
        }


        private Invoice CreateInvoice()
        {
            return new Invoice
            {
                Id = 1,
                TotalAmount = 500,
                Status = InvoiceStatus.Issued
            };
        }



        [Fact]
        public void AddPayment_Valid_ShouldReturnSuccess()
        {
            var invoice = CreateInvoice();

            _invoiceRepoMock
                .Setup(x => x.GetById(1))
                .Returns(invoice);

            _paymentRepoMock
                .Setup(x => x.GetTotalPaidForInvoice(1))
                .Returns(0);


            var result = _service.AddPayment(CreateRequest());


            Assert.Equal(
                PaymentAddResult.Success,
                result.Result);


            _paymentRepoMock.Verify(
                x => x.Add(It.IsAny<Payment>()),
                Times.Once);


            _uowMock.Verify(
                x => x.Save(),
                Times.Once);
        }



        [Fact]
        public void AddPayment_InvoiceNotFound_ShouldReturnInvoiceNotFound()
        {
            _invoiceRepoMock
                .Setup(x => x.GetById(1))
                .Returns((Invoice)null);


            var result = _service.AddPayment(CreateRequest());


            Assert.Equal(
                PaymentAddResult.InvoiceNotFound,
                result.Result);
        }



        [Fact]
        public void AddPayment_InvoiceArchived_ShouldReturnInvoiceArchived()
        {
            var invoice = CreateInvoice();
            invoice.IsInvoiceArchived = true;


            _invoiceRepoMock
                .Setup(x => x.GetById(1))
                .Returns(invoice);


            var result = _service.AddPayment(CreateRequest());


            Assert.Equal(
                PaymentAddResult.InvoiceArchived,
                result.Result);
        }



        [Fact]
        public void AddPayment_InvalidAmount_ShouldReturnInvalidAmount()
        {
            var invoice = CreateInvoice();


            _invoiceRepoMock
                .Setup(x => x.GetById(1))
                .Returns(invoice);


            var result = _service.AddPayment(
                CreateRequest(0));


            Assert.Equal(
                PaymentAddResult.InvalidAmount,
                result.Result);
        }



        [Fact]
        public void AddPayment_AmountExceedsRemaining_ShouldReturnAmountExceedsRemaining()
        {
            var invoice = CreateInvoice();


            _invoiceRepoMock
                .Setup(x => x.GetById(1))
                .Returns(invoice);


            _paymentRepoMock
                .Setup(x => x.GetTotalPaidForInvoice(1))
                .Returns(450);


            var result = _service.AddPayment(
                CreateRequest(100));


            Assert.Equal(
                PaymentAddResult.AmountExceedsRemaining,
                result.Result);
        }



        [Fact]
        public void DeletePayment_NotFound_ShouldReturnNotFound()
        {
            _paymentRepoMock
                .Setup(x => x.GetById(1))
                .Returns((Payment)null);


            var result = _service.DeletePayment(1);


            Assert.Equal(
                PaymentDeleteResult.NotFound,
                result);
        }



        [Fact]
        public void DeletePayment_Valid_ShouldDelete()
        {
            var payment = new Payment
            {
                Id = 1,
                InvoiceId = 1,
                Amount = 100
            };


            var invoice = CreateInvoice();


            _paymentRepoMock
                .Setup(x => x.GetById(1))
                .Returns(payment);


            _invoiceRepoMock
                .Setup(x => x.GetById(1))
                .Returns(invoice);


            _paymentRepoMock
                .Setup(x => x.GetTotalPaidForInvoice(1))
                .Returns(0);



            var result = _service.DeletePayment(1);



            Assert.Equal(
                PaymentDeleteResult.Success,
                result);


            _paymentRepoMock.Verify(
                x => x.Delete(payment),
                Times.Once);


            _uowMock.Verify(
                x => x.Save(),
                Times.Once);
        }



        [Fact]
        public void GetPaymentsForInvoice_ShouldReturnPayments()
        {
            var payments = new List<Payment>
            {
                new Payment
                {
                    Id = 1,
                    InvoiceId = 1,
                    Amount = 100,
                    Status = PaymentStatus.Paid
                },
                new Payment
                {
                    Id = 2,
                    InvoiceId = 1,
                    Amount = 200,
                    Status = PaymentStatus.Paid
                }
            };


            _paymentRepoMock
                .Setup(x => x.GetByInvoiceId(1))
                .Returns(payments);



            var result = _service.GetPaymentsForInvoice(1);



            Assert.Equal(2, result.Count());
        }
    }
}