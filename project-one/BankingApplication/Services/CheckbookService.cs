using BankingApp.Interfaces;
using BankingApp.Models;

namespace BankingApp.Services
{
    public class CheckbookService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IServiceRequestRepository
            _serviceRequestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CheckbookService(
            IAccountRepository accountRepository,
            IServiceRequestRepository serviceRequestRepository,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _serviceRequestRepository =
                serviceRequestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task RequestCheckbookAsync(
            int customerId,
            string accountNumber)
        {
            Account account =
                await _accountRepository
                    .GetByAccountNumberAsync(accountNumber)
                ?? throw new InvalidOperationException(
                    "Account was not found.");

            if (account.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException(
                    "Account does not belong to this customer.");
            }

            if (account.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException(
                    "Cheque books can only be requested for an active account.");
            }

            var request = new CheckbookRequest(
                customerId,
                account.AccountId);

            await _serviceRequestRepository
                .AddAsync(request);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
