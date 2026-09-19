using BankingApp.Models;

namespace BankingApp.Interfaces
{
    public interface IServiceRequestRepository
    {
        Task<ServiceRequest?> GetByIdAsync(int requestId);

        Task<IReadOnlyList<ServiceRequest>>
            GetByCustomerIdAsync(int customerId);

        Task<IReadOnlyList<ServiceRequest>>
            GetPendingAsync();

        Task AddAsync(ServiceRequest request);
    }
}