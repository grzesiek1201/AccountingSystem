using AccountingSystem.Application.Interfaces;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using System;

namespace AccountingSystem.Application.Services
{
    public class NumberSequenceService : INumberSequenceService
    {
        private readonly INumberSequenceRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public NumberSequenceService(
            INumberSequenceRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }


        public string GetNext(DocumentType type)
        {
            int year = DateTime.UtcNow.Year;

            _unitOfWork.BeginTransaction();

            try
            {
                var sequence = _repository
                    .GetNextWithLock(type, year);


                if (sequence == null)
                {
                    sequence = new NumberSequence(type, year);

                    _repository.Add(sequence);
                }
                else
                {
                    sequence.Increment();
                    _repository.Update(sequence);
                }


                _unitOfWork.Save();

                _unitOfWork.Commit();


                return FormatNumber(
                    type,
                    year,
                    sequence.LastNumber);
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }


        private string FormatNumber(
            DocumentType type,
            int year,
            int number)
        {
            return $"{GetPrefix(type)}-{year}-{number:0000}";
        }


        private string GetPrefix(DocumentType type)
        {
            return type switch
            {
                DocumentType.Quotation => "Q",
                DocumentType.Order => "O",
                DocumentType.Invoice => "I",

                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }
    }
}