namespace BankingApp.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}