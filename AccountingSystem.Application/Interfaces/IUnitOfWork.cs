namespace AccountingSystem.Application.Interfaces
{
    public interface IUnitOfWork
    {
        void Save();

        void BeginTransaction();

        void Commit();

        void Rollback();
    }
}