using BankingApp.Interfaces;
using BankingApp.Models;
using BankingApp.Services.Results;

namespace BankingApp.Services
{
    public class AccountClosureService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IServiceRequestRepository
            _serviceRequestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AccountClosureService(
            IAccountRepository accountRepository,
            IServiceRequestRepository serviceRequestRepository,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _serviceRequestRepository =
                serviceRequestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<AccountClosureOption>>
            GetClosureOptionsAsync(int customerId)
        {
            IReadOnlyList<Account> accounts =
                await _accountRepository
                    .GetByCustomerIdAsync(customerId);

            IReadOnlyList<ServiceRequest> requests =
                await _serviceRequestRepository
                    .GetByCustomerIdAsync(customerId);

            HashSet<int> pendingAccountIds = requests
                .OfType<AccountClosureRequest>()
                .Where(request =>
                    request.Status == RequestStatus.Pending)
                .Select(request => request.AccountId)
                .ToHashSet();

            return accounts
                .Where(account =>
                    account.Status != AccountStatus.Closed)
                .Select(account =>
                {
                    string? reason = null;

                    if (pendingAccountIds.Contains(
                        account.AccountId))
                    {
                        reason =
                            "Closure request already pending";
                    }
                    else if (account.Balance != 0)
                    {
                        reason = "Balance must be $0";
                    }

                    return new AccountClosureOption
                    {
                        AccountId = account.AccountId,
                        MaskedAccountNumber =
                            MaskAccountNumber(
                                account.AccountNumber),
                        AccountType = account.AccountType,
                        Status = account.Status,
                        Balance = account.Balance,
                        IsEligible = reason == null,
                        IneligibilityReason = reason
                    };
                })
                .ToList();
        }

        public async Task<AccountClosureRequest>
            RequestClosureAsync(
                int customerId,
                int accountId)
        {
            Account account = await _accountRepository
                .GetByIdAsync(accountId)
                ?? throw new InvalidOperationException(
                    "Account was not found.");

            if (account.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException(
                    "Account does not belong to this customer.");
            }

            if (account.Status == AccountStatus.Closed)
            {
                throw new InvalidOperationException(
                    "Account is already closed.");
            }

            if (account.Balance != 0)
            {
                throw new InvalidOperationException(
                    "Account balance must be zero before requesting closure.");
            }

            IReadOnlyList<ServiceRequest> customerRequests =
                await _serviceRequestRepository
                    .GetByCustomerIdAsync(customerId);

            bool alreadyPending = customerRequests
                .OfType<AccountClosureRequest>()
                .Any(request =>
                    request.AccountId == account.AccountId &&
                    request.Status == RequestStatus.Pending);

            if (alreadyPending)
            {
                throw new InvalidOperationException(
                    "An account closure request is already pending.");
            }

            var request = new AccountClosureRequest(
                customerId,
                account.AccountId);

            await _serviceRequestRepository.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            return request;
        }

        private static string MaskAccountNumber(
            string accountNumber)
        {
            if (accountNumber.Length <= 4)
            {
                return accountNumber;
            }

            return "****" + accountNumber[^4..];
        }
    }
}
