using BankingApp.Interfaces;
using BankingApp.Models;
using BankingApp.Services.Results;

namespace BankingApp.Services
{
    public class AdminService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IServiceRequestRepository
            _serviceRequestRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly AccountNumberGenerator
            _accountNumberGenerator;
        private readonly AuthorizationService
            _authorizationService;
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(
            ICustomerRepository customerRepository,
            IAccountRepository accountRepository,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IServiceRequestRepository serviceRequestRepository,
            IPasswordHasher passwordHasher,
            AccountNumberGenerator accountNumberGenerator,
            AuthorizationService authorizationService,
            IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _serviceRequestRepository =
                serviceRequestRepository;
            _passwordHasher = passwordHasher;
            _accountNumberGenerator = accountNumberGenerator;
            _authorizationService = authorizationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Account> CreateCheckingAccountAsync(
            int adminUserId,
            int customerId,
            decimal openingBalance,
            decimal overdraftLimit)
        {
            await EnsureCustomerExistsAsync(
                adminUserId,
                customerId);

            string accountNumber =
                await _accountNumberGenerator.GenerateAsync();

            var account = new CheckingAccount(
                customerId,
                accountNumber,
                openingBalance,
                overdraftLimit);

            await _accountRepository.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return account;
        }

        public async Task<Account> CreateSavingsAccountAsync(
            int adminUserId,
            int customerId,
            decimal openingBalance,
            decimal annualInterestRate,
            decimal minimumBalance)
        {
            await EnsureCustomerExistsAsync(
                adminUserId,
                customerId);

            string accountNumber =
                await _accountNumberGenerator.GenerateAsync();

            var account = new SavingsAccount(
                customerId,
                accountNumber,
                openingBalance,
                annualInterestRate,
                minimumBalance);

            await _accountRepository.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return account;
        }

        public async Task CloseAccountAsync(
            int adminUserId,
            string accountNumber)
        {
            Account account = await GetAccountAsync(
                adminUserId,
                accountNumber);

            await EnsureNoPendingAccountRequestsAsync(
                account.CustomerId,
                account.AccountId);

            account.Close();

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Account> GetAccountAsync(
            int adminUserId,
            string accountNumber)
        {
            await _authorizationService
                .EnsureAdminAsync(adminUserId);

            return await _accountRepository
                .GetByAccountNumberAsync(accountNumber)
                ?? throw new InvalidOperationException(
                    "Account was not found.");
        }

        public async Task UpdateCheckingAccountAsync(
            int adminUserId,
            string accountNumber,
            decimal overdraftLimit)
        {
            Account account = await GetAccountAsync(
                adminUserId,
                accountNumber);

            if (account is not CheckingAccount checkingAccount)
            {
                throw new InvalidOperationException(
                    "The selected account is not a checking account.");
            }

            checkingAccount.UpdateOverdraftLimit(
                overdraftLimit);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateSavingsAccountAsync(
            int adminUserId,
            string accountNumber,
            decimal annualInterestRate,
            decimal minimumBalance)
        {
            Account account = await GetAccountAsync(
                adminUserId,
                accountNumber);

            if (account is not SavingsAccount savingsAccount)
            {
                throw new InvalidOperationException(
                    "The selected account is not a savings account.");
            }

            savingsAccount.UpdateTerms(
                annualInterestRate,
                minimumBalance);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<AdminBankingSummary>
            GetBankingSummaryAsync(int adminUserId)
        {
            await _authorizationService
                .EnsureAdminAsync(adminUserId);

            return new AdminBankingSummary
            {
                TotalAccounts =
                    await _accountRepository.CountAsync(),
                ActiveAccounts =
                    await _accountRepository.CountByStatusAsync(
                        AccountStatus.Active),
                FrozenAccounts =
                    await _accountRepository.CountByStatusAsync(
                        AccountStatus.Frozen),
                ClosedAccounts =
                    await _accountRepository.CountByStatusAsync(
                        AccountStatus.Closed),
                TotalBalance =
                    await _accountRepository.GetTotalBalanceAsync()
            };
        }

        public async Task ResetCustomerPasswordAsync(
            int adminUserId,
            string username,
            string newPassword)
        {
            await _authorizationService
                .EnsureAdminAsync(adminUserId);

            User user = await _userRepository
                .GetByUsernameAsync(username)
                ?? throw new InvalidOperationException(
                    "Customer user was not found.");

            bool isCustomer = await _userRoleRepository
                .UserHasRoleAsync(
                    user.UserId,
                    "Customer");

            if (!isCustomer)
            {
                throw new InvalidOperationException(
                    "The selected user is not a customer.");
            }

            user.PasswordHash = _passwordHasher.Hash(
                user,
                newPassword);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ApproveCheckbookRequestAsync(
            int adminUserId,
            int requestId)
        {
            await _authorizationService
                .EnsureAdminAsync(adminUserId);

            ServiceRequest request =
                await GetRequestAsync(requestId);

            if (request is not CheckbookRequest)
            {
                throw new InvalidOperationException(
                    "The selected request is not a cheque-book request.");
            }

            Account account = await _accountRepository
                .GetByIdAsync(
                    ((CheckbookRequest)request).AccountId)
                ?? throw new InvalidOperationException(
                    "The requested account was not found.");

            if (account.CustomerId != request.CustomerId)
            {
                throw new InvalidOperationException(
                    "The request does not match the account owner.");
            }

            if (account.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException(
                    "Cheque-book requests can only be approved for an active account.");
            }

            request.Approve();

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<AccountClosureRequest>>
            GetPendingAccountClosureRequestsAsync(
                int adminUserId)
        {
            await _authorizationService
                .EnsureAdminAsync(adminUserId);

            IReadOnlyList<ServiceRequest> requests =
                await _serviceRequestRepository
                    .GetPendingAsync();

            return requests
                .OfType<AccountClosureRequest>()
                .ToList();
        }

        public async Task ApproveAccountClosureRequestAsync(
            int adminUserId,
            int requestId)
        {
            await _authorizationService
                .EnsureAdminAsync(adminUserId);

            await _unitOfWork.ExecuteInTransactionAsync(
                async () =>
                {
                    ServiceRequest request =
                        await GetRequestAsync(requestId);

                    if (request is not AccountClosureRequest
                        closureRequest)
                    {
                        throw new InvalidOperationException(
                            "The selected request is not an account closure request.");
                    }

                    if (closureRequest.Status !=
                        RequestStatus.Pending)
                    {
                        throw new InvalidOperationException(
                            "Only pending account closure requests can be approved.");
                    }

                    Account account =
                        await _accountRepository
                            .GetByIdAsync(
                                closureRequest.AccountId)
                        ?? throw new InvalidOperationException(
                            "The requested account was not found.");

                    if (account.CustomerId !=
                        closureRequest.CustomerId)
                    {
                        throw new InvalidOperationException(
                            "The request does not match the account owner.");
                    }

                    if (account.Status == AccountStatus.Closed)
                    {
                        throw new InvalidOperationException(
                            "Account is already closed.");
                    }

                    await EnsureNoPendingAccountRequestsAsync(
                        account.CustomerId,
                        account.AccountId,
                        closureRequest.RequestId);

                    account.Close();
                    closureRequest.Approve();
                    closureRequest.MarkCompleted();

                    await _unitOfWork.SaveChangesAsync();
                });
        }

        public async Task<IReadOnlyList<ServiceRequest>>
            GetPendingRequestsAsync(int adminUserId)
        {
            await _authorizationService
                .EnsureAdminAsync(adminUserId);

            return await _serviceRequestRepository
                .GetPendingAsync();
        }

        public async Task ApproveRequestAsync(
            int adminUserId,
            int requestId)
        {
            await _authorizationService
                .EnsureAdminAsync(adminUserId);

            ServiceRequest request =
                await GetRequestAsync(requestId);

            request.Approve();

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RejectRequestAsync(
            int adminUserId,
            int requestId)
        {
            await _authorizationService
                .EnsureAdminAsync(adminUserId);

            ServiceRequest request =
                await GetRequestAsync(requestId);

            request.Reject();

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CompleteRequestAsync(
            int adminUserId,
            int requestId)
        {
            await _authorizationService
                .EnsureAdminAsync(adminUserId);

            ServiceRequest request =
                await GetRequestAsync(requestId);

            request.MarkCompleted();

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task EnsureCustomerExistsAsync(
            int adminUserId,
            int customerId)
        {
            await _authorizationService
                .EnsureAdminAsync(adminUserId);

            _ = await _customerRepository
                .GetByIdAsync(customerId)
                ?? throw new InvalidOperationException(
                    "Customer was not found.");
        }

        private async Task<ServiceRequest> GetRequestAsync(
            int requestId)
        {
            return await _serviceRequestRepository
                .GetByIdAsync(requestId)
                ?? throw new InvalidOperationException(
                    "Service request was not found.");
        }

        private async Task EnsureNoPendingAccountRequestsAsync(
            int customerId,
            int accountId,
            int? excludedRequestId = null)
        {
            IReadOnlyList<ServiceRequest> requests =
                await _serviceRequestRepository
                    .GetByCustomerIdAsync(customerId);

            bool hasPendingRequest = requests
                .OfType<AccountServiceRequest>()
                .Any(request =>
                    request.AccountId == accountId &&
                    request.Status == RequestStatus.Pending &&
                    request.RequestId != excludedRequestId);

            if (hasPendingRequest)
            {
                throw new InvalidOperationException(
                    "Resolve other pending account requests before closing the account.");
            }
        }
    }
}
