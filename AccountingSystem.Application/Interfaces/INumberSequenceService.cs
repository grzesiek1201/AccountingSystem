using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.Interfaces
{
    public interface INumberSequenceService
    {
        string GetNext(DocumentType type);
    }
}
