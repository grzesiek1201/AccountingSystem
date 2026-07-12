using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Moq;
using Xunit;


namespace AccountingSystem.Tests.ServicesTests
{
    public class NumberSequenceServiceTests
    {
        private readonly Mock<INumberSequenceRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly NumberSequenceService _service;

        public NumberSequenceServiceTests()
        {
            _repoMock = new Mock<INumberSequenceRepository>();
            _uowMock = new Mock<IUnitOfWork>();

            _service = new NumberSequenceService(
                _repoMock.Object,
                _uowMock.Object
            );
        }

        // ================= FIRST CREATE =================

        [Fact]
        public void GetNext_NoSequence_ShouldCreateNewAndReturnFirstNumber()
        {
            _repoMock
                .Setup(r => r.GetNext(DocumentType.Invoice, It.IsAny<int>()))
                .Returns((NumberSequence)null);

            var result = _service.GetNext(DocumentType.Invoice);

            Assert.Contains("I-2026-0001", result);

            _repoMock.Verify(r => r.Add(It.IsAny<NumberSequence>()), Times.Once);
            _repoMock.Verify(r => r.Update(It.IsAny<NumberSequence>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        // ================= INCREMENT =================

        [Fact]
        public void GetNext_ExistingSequence_ShouldIncrementAndReturnNextNumber()
        {
            var seq = new NumberSequence
            {
                DocumentType = DocumentType.Invoice,
                Year = DateTime.UtcNow.Year,
                LastNumber = 5
            };

            _repoMock
                .Setup(r => r.GetNext(DocumentType.Invoice, It.IsAny<int>()))
                .Returns(seq);

            var result = _service.GetNext(DocumentType.Invoice);

            Assert.Contains("0006", result);

            _repoMock.Verify(r => r.Update(seq), Times.Once);
            _repoMock.Verify(r => r.Add(It.IsAny<NumberSequence>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        // ================= PREFIX CHECK =================

        [Theory]
        [InlineData(DocumentType.Invoice, "I")]
        [InlineData(DocumentType.Order, "O")]
        [InlineData(DocumentType.Quotation, "Q")]
        public void GetNext_ShouldReturnCorrectPrefix(DocumentType type, string expectedPrefix)
        {
            _repoMock
                .Setup(r => r.GetNext(type, It.IsAny<int>()))
                .Returns((NumberSequence)null);

            var result = _service.GetNext(type);

            Assert.StartsWith($"{expectedPrefix}-{DateTime.UtcNow.Year}-", result);
        }

        // ================= FORMAT CHECK =================

        [Fact]
        public void GetNext_ShouldAlwaysReturn4DigitNumber()
        {
            var seq = new NumberSequence
            {
                DocumentType = DocumentType.Invoice,
                Year = DateTime.UtcNow.Year,
                LastNumber = 9
            };

            _repoMock
                .Setup(r => r.GetNext(DocumentType.Invoice, It.IsAny<int>()))
                .Returns(seq);

            var result = _service.GetNext(DocumentType.Invoice);

            Assert.EndsWith("-0010", result);
        }
    }

}

