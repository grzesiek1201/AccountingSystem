using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.Infrastructure.Repositories
{
    public class NumberSequenceRepository : INumberSequenceRepository
    {
        private readonly AppDbContext _context;

        public NumberSequenceRepository(AppDbContext context)
        {
            _context = context;
        }


        public NumberSequence? GetNextWithLock(
            DocumentType type,
            int year)
        {
            return _context.NumberSequences
                .FromSqlInterpolated($@"
                    SELECT *
                    FROM NumberSequences WITH (UPDLOCK, ROWLOCK)
                    WHERE DocumentType = {type}
                    AND Year = {year}")
                .FirstOrDefault();
        }


        public void Add(NumberSequence sequence)
        {
            _context.NumberSequences.Add(sequence);
        }


        public void Update(NumberSequence sequence)
        {
            _context.NumberSequences.Update(sequence);
        }
    }
}