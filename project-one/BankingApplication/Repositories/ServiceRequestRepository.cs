using BankingApp.Data;
using BankingApp.Interfaces;
using BankingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Repositories
{
    public class ServiceRequestRepository
        : IServiceRequestRepository
    {
        private readonly BankingDbContext _context;

        public ServiceRequestRepository(
            BankingDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceRequest?> GetByIdAsync(
            int requestId)
        {
            return await _context.ServiceRequests
                .FirstOrDefaultAsync(
                    r => r.RequestId == requestId);
        }

        public async Task<IReadOnlyList<ServiceRequest>>
            GetByCustomerIdAsync(int customerId)
        {
            return await _context.ServiceRequests
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.RequestedDate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<ServiceRequest>>
            GetPendingAsync()
        {
            return await _context.ServiceRequests
                .Where(r => r.Status == RequestStatus.Pending)
                .OrderBy(r => r.RequestedDate)
                .ToListAsync();
        }

        public async Task AddAsync(ServiceRequest request)
        {
            await _context.ServiceRequests.AddAsync(request);
        }
    }
}