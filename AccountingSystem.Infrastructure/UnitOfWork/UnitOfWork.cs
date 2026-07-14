using AccountingSystem.Application.Interfaces;
using AccountingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace AccountingSystem.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;


        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }


        public void Save()
        {
            _context.SaveChanges();
        }


        public void BeginTransaction()
        {
            if (_transaction != null)
                return;

            _transaction = _context.Database.BeginTransaction();
        }


        public void Commit()
        {
            if (_transaction == null)
                return;

            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }


        public void Rollback()
        {
            if (_transaction == null)
                return;

            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }
    }
}