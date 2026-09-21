using BankingApp.Interfaces;
using BankingApp.Models;

namespace BankingApp.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(
            ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Customer> GetByUserIdAsync(int userId)
        {
            return await _customerRepository.GetByUserIdAsync(userId)
                ?? throw new InvalidOperationException(
                    "Customer record was not found.");
        }
    }
}