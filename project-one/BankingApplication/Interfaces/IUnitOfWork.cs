namespace BankingApp.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();

        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}