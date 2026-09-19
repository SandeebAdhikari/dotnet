using BankingApp.Models;

namespace BankingApp.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByUserIdAsync(int userId);

        Task<Customer?> GetByIdAsync(int customerId);

        Task AddAsync(Customer customer);
    }
}