using BankingApp.Data;
using BankingApp.Interfaces;
using BankingApp.Models;
using BankingApp.Repositories;
using BankingApp.Services;
using BankingApp.Services.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BankingApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                string connectionString =
                    Environment.GetEnvironmentVariable(
                        "BANKING_DB_CONNECTION")
                    ?? throw new InvalidOperationException(
                        "BANKING_DB_CONNECTION environment variable is not set.");

                using ServiceProvider serviceProvider =
                    ConfigureServices(connectionString);

                await BootstrapAdministratorAsync(
                    serviceProvider);

                await RunWelcomeMenuAsync(serviceProvider);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Application startup failed: {ex.Message}");
            }
        }

        private static ServiceProvider ConfigureServices(
            string connectionString)
        {
            var services = new ServiceCollection();

            services.AddDbContext<BankingDbContext>(
                options =>
                    options.UseSqlServer(connectionString));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IPasswordHasher, PasswordHasherService>();
            services.AddSingleton<
                IDataAnnotationsValidator,
                DataAnnotationsValidator>();

            services.AddScoped<AdminBootstrapService>();
            services.AddScoped<AccountNumberGenerator>();
            services.AddScoped<RegistrationInputValidator>();
            services.AddScoped<RegistrationService>();
            services.AddScoped<AuthService>();
            services.AddScoped<AuthorizationService>();
            services.AddScoped<CustomerService>();
            services.AddScoped<AccountService>();
            services.AddScoped<TransactionService>();
            services.AddScoped<TransferService>();
            services.AddScoped<CheckbookService>();
            services.AddScoped<AccountClosureService>();
            services.AddScoped<AdminService>();

            return services.BuildServiceProvider();
        }

        private static async Task BootstrapAdministratorAsync(
            ServiceProvider serviceProvider)
        {
            using IServiceScope scope =
                serviceProvider.CreateScope();

            AdminBootstrapService bootstrapService =
                scope.ServiceProvider
                    .GetRequiredService<AdminBootstrapService>();

            await bootstrapService.BootstrapAsync(
                Environment.GetEnvironmentVariable(
                    "BANKING_ADMIN_USERNAME"),
                Environment.GetEnvironmentVariable(
                    "BANKING_ADMIN_PASSWORD"));
        }

        private static async Task RunWelcomeMenuAsync(
            ServiceProvider serviceProvider)
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("===== BANKING APPLICATION =====");
                Console.WriteLine();
                Console.WriteLine("1. Customer");
                Console.WriteLine("2. Admin");
                Console.WriteLine("3. Exit");
                Console.WriteLine();
                Console.Write("Select an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await RunCustomerEntryMenuAsync(
                            serviceProvider);
                        break;

                    case "2":
                        await RunAdminLoginAsync(
                            serviceProvider);
                        break;

                    case "3":
                        return;

                    default:
                        ShowMessage("Invalid choice");
                        break;
                }
            }
        }

        private static async Task RunCustomerEntryMenuAsync(
            ServiceProvider serviceProvider)
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("===== CUSTOMER =====");
                Console.WriteLine();
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Back");
                Console.WriteLine();
                Console.Write("Select an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await LoginCustomerAsync(serviceProvider);
                        return;

                    case "2":
                        await RegisterCustomerAsync(
                            serviceProvider);
                        break;

                    case "3":
                        return;

                    default:
                        ShowMessage("Invalid choice");
                        break;
                }
            }
        }

        private static async Task RegisterCustomerAsync(
            ServiceProvider serviceProvider)
        {
            Console.Clear();

            Console.WriteLine("===== CUSTOMER SIGN UP =====");
            Console.WriteLine();

            try
            {
                using IServiceScope scope =
                    serviceProvider.CreateScope();

                RegistrationInputValidator inputValidator =
                    scope.ServiceProvider
                        .GetRequiredService<RegistrationInputValidator>();

                string firstName =
                    ReadValidated(
                        "First Name: ",
                        inputValidator.ValidateFirstName);
                string lastName =
                    ReadValidated(
                        "Last Name: ",
                        inputValidator.ValidateLastName);
                DateTime dateOfBirth =
                    ReadDate("Date of Birth (MM/dd/yyyy): ");

                Console.WriteLine();
                Console.WriteLine("Address");

                var address = new Address
                {
                    Street = ReadValidated(
                        "Street: ",
                        inputValidator.ValidateStreet),
                    City = ReadValidated(
                        "City: ",
                        inputValidator.ValidateCity),
                    State = ReadValidated(
                        "State: ",
                        inputValidator.ValidateState),
                    Country = ReadValidated(
                        "Country: ",
                        inputValidator.ValidateCountry),
                    ZipCode = ReadValidated(
                        "ZIP Code: ",
                        inputValidator.ValidateZipCode)
                };

                string phoneNumber =
                    ReadValidated(
                        "Phone Number: ",
                        inputValidator.ValidatePhoneNumber);
                string email =
                    ReadValidated(
                        "Email: ",
                        inputValidator.ValidateEmail);
                string ssn =
                    ReadValidated(
                        "SSN: ",
                        inputValidator
                            .ValidateSocialSecurityNumber);
                string username =
                    await ReadValidatedAsync(
                        "Username: ",
                        inputValidator.ValidateUsernameAsync);
                string password =
                    ReadValidatedPassword(
                        "Password: ",
                        inputValidator.ValidatePassword);

                ReadPasswordConfirmation(password);

                AccountType accountType =
                    ReadRegistrationAccountType();

                decimal openingBalance =
                    ReadValidatedDecimal(
                        "Opening Deposit: ",
                        value => inputValidator
                            .ValidateOpeningBalance(
                                accountType,
                                value));

                RegistrationService registrationService =
                    scope.ServiceProvider
                        .GetRequiredService<RegistrationService>();

                Account account =
                    await registrationService
                        .RegisterCustomerAsync(
                    username,
                    password,
                    firstName,
                    lastName,
                    dateOfBirth,
                    address,
                    phoneNumber,
                    email,
                    ssn,
                    accountType,
                    openingBalance);

                ShowMessage(
                    "Customer registration successful.\n" +
                    $"{account.AccountType} account number: " +
                    $"{account.AccountNumber}\n" +
                    "Please log in.");
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        private static async Task LoginCustomerAsync(
            ServiceProvider serviceProvider)
        {
            Console.Clear();

            Console.WriteLine("===== CUSTOMER LOGIN =====");
            Console.WriteLine();

            string username =
                ReadRequired("Username: ");
            string password =
                ReadPassword("Password: ");

            int customerId;

            try
            {
                using IServiceScope scope =
                    serviceProvider.CreateScope();

                AuthService authService =
                    scope.ServiceProvider
                        .GetRequiredService<AuthService>();
                AuthorizationService authorizationService =
                    scope.ServiceProvider
                        .GetRequiredService<AuthorizationService>();
                CustomerService customerService =
                    scope.ServiceProvider
                        .GetRequiredService<CustomerService>();

                User? user = await authService
                    .AuthenticateAsync(username, password);

                if (user == null ||
                    !await authorizationService
                        .IsCustomerAsync(user.UserId))
                {
                    ShowMessage("Invalid Credential");
                    return;
                }

                Customer customer = await customerService
                    .GetByUserIdAsync(user.UserId);

                customerId = customer.CustomerId;
                username = user.Username;
            }
            catch (Exception ex)
            {
                ShowMessage(
                    $"Customer login could not be completed: {ex.Message}");
                return;
            }

            await RunCustomerMenuAsync(
                serviceProvider,
                username,
                customerId);
        }

        private static async Task RunCustomerMenuAsync(
            ServiceProvider serviceProvider,
            string username,
            int customerId)
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("===== CUSTOMER MENU =====");
                Console.WriteLine();
                Console.WriteLine("1. Check Account Details");
                Console.WriteLine("2. Withdraw");
                Console.WriteLine("3. Deposit");
                Console.WriteLine("4. Transfer");
                Console.WriteLine("5. View Last 5 Transactions");
                Console.WriteLine("6. Request Cheque Book");
                Console.WriteLine("7. Change Password");
                Console.WriteLine("8. Request Account Closure");
                Console.WriteLine("9. Exit");
                Console.WriteLine();
                Console.Write("Select an option: ");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ShowAccountsAsync(
                                serviceProvider,
                                customerId);
                            break;

                        case "2":
                            await WithdrawAsync(
                                serviceProvider,
                                customerId);
                            break;

                        case "3":
                            await DepositAsync(
                                serviceProvider,
                                customerId);
                            break;

                        case "4":
                            await TransferAsync(
                                serviceProvider,
                                customerId);
                            break;

                        case "5":
                            await ShowTransactionsAsync(
                                serviceProvider,
                                customerId);
                            break;

                        case "6":
                            await RequestCheckbookAsync(
                                serviceProvider,
                                customerId);
                            break;

                        case "7":
                            await ChangePasswordAsync(
                                serviceProvider,
                                username);
                            break;

                        case "8":
                            await RequestAccountClosureAsync(
                                serviceProvider,
                                username,
                                customerId);
                            break;

                        case "9":
                            return;

                        default:
                            ShowMessage("Invalid choice");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message);
                }
            }
        }

        private static async Task ShowAccountsAsync(
            ServiceProvider serviceProvider,
            int customerId)
        {
            using IServiceScope scope =
                serviceProvider.CreateScope();

            AccountService accountService =
                scope.ServiceProvider
                    .GetRequiredService<AccountService>();

            IReadOnlyList<Account> accounts =
                await accountService
                    .GetAccountsAsync(customerId);

            Console.Clear();
            Console.WriteLine("===== ACCOUNT DETAILS =====");
            Console.WriteLine();

            if (accounts.Count == 0)
            {
                ShowMessage(
                    "No bank accounts were found. Contact an administrator to open an account.");
                return;
            }

            foreach (Account account in accounts)
            {
                Console.WriteLine(
                    $"Account Number: {account.AccountNumber}");
                Console.WriteLine(
                    $"Type: {account.AccountType}");
                Console.WriteLine(
                    $"Balance: {account.Balance:C}");
                Console.WriteLine(
                    $"Status: {account.Status}");
                Console.WriteLine(
                    $"Opened: {account.OpenedDate:d}");

                if (account is CheckingAccount checking)
                {
                    Console.WriteLine(
                        $"Overdraft Limit: {checking.OverdraftLimit:C}");
                    Console.WriteLine(
                        $"Available Funds: {checking.AvailableFunds:C}");
                }

                if (account is SavingsAccount savings)
                {
                    Console.WriteLine(
                        $"Interest Rate: {savings.AnnualInterestRate:P}");
                    Console.WriteLine(
                        $"Minimum Balance: {savings.MinimumBalance:C}");
                }

                Console.WriteLine(new string('-', 40));
            }

            WaitForEnter();
        }

        private static async Task WithdrawAsync(
            ServiceProvider serviceProvider,
            int customerId)
        {
            string accountNumber =
                ReadRequired("Account Number: ");
            decimal amount =
                ReadDecimal("Withdrawal Amount: ");

            using IServiceScope scope =
                serviceProvider.CreateScope();

            AccountService accountService =
                scope.ServiceProvider
                    .GetRequiredService<AccountService>();

            await accountService.WithdrawAsync(
                customerId,
                accountNumber,
                amount);

            ShowMessage("Withdrawal successful.");
        }

        private static async Task DepositAsync(
            ServiceProvider serviceProvider,
            int customerId)
        {
            string accountNumber =
                ReadRequired("Account Number: ");
            decimal amount =
                ReadDecimal("Deposit Amount: ");

            using IServiceScope scope =
                serviceProvider.CreateScope();

            AccountService accountService =
                scope.ServiceProvider
                    .GetRequiredService<AccountService>();

            await accountService.DepositAsync(
                customerId,
                accountNumber,
                amount);

            ShowMessage("Deposit successful.");
        }

        private static async Task TransferAsync(
            ServiceProvider serviceProvider,
            int customerId)
        {
            string fromAccount =
                ReadRequired("From Account Number: ");
            string toAccount =
                ReadRequired("To Account Number: ");
            decimal amount =
                ReadDecimal("Transfer Amount: ");

            using IServiceScope scope =
                serviceProvider.CreateScope();

            TransferService transferService =
                scope.ServiceProvider
                    .GetRequiredService<TransferService>();

            await transferService.TransferAsync(
                customerId,
                fromAccount,
                toAccount,
                amount);

            ShowMessage("Transfer successful.");
        }

        private static async Task ShowTransactionsAsync(
            ServiceProvider serviceProvider,
            int customerId)
        {
            string accountNumber =
                ReadRequired("Account Number: ");

            using IServiceScope scope =
                serviceProvider.CreateScope();

            TransactionService transactionService =
                scope.ServiceProvider
                    .GetRequiredService<TransactionService>();

            IReadOnlyList<Transaction> transactions =
                await transactionService
                    .GetLastFiveTransactionsAsync(
                        customerId,
                        accountNumber);

            Console.Clear();
            Console.WriteLine("===== LAST 5 TRANSACTIONS =====");
            Console.WriteLine();

            if (transactions.Count == 0)
            {
                ShowMessage("No transactions were found.");
                return;
            }

            foreach (Transaction transaction in transactions)
            {
                Console.WriteLine(
                    $"Date: {transaction.TransactionDate:g}");
                Console.WriteLine(
                    $"Type: {transaction.TransactionType}");
                Console.WriteLine(
                    $"Amount: {transaction.Amount:C}");
                Console.WriteLine(
                    $"Balance After: {transaction.BalanceAfterTransaction:C}");

                if (transaction.RelatedAccountId.HasValue)
                {
                    Console.WriteLine(
                        $"Related Account ID: {transaction.RelatedAccountId}");
                }

                Console.WriteLine(new string('-', 40));
            }

            WaitForEnter();
        }

        private static async Task RequestCheckbookAsync(
            ServiceProvider serviceProvider,
            int customerId)
        {
            string accountNumber =
                ReadRequired("Account Number: ");

            using IServiceScope scope =
                serviceProvider.CreateScope();

            CheckbookService checkbookService =
                scope.ServiceProvider
                    .GetRequiredService<CheckbookService>();

            await checkbookService.RequestCheckbookAsync(
                customerId,
                accountNumber);

            ShowMessage("Cheque book request submitted.");
        }

        private static async Task RequestAccountClosureAsync(
            ServiceProvider serviceProvider,
            string username,
            int customerId)
        {
            using IServiceScope scope =
                serviceProvider.CreateScope();

            AccountClosureService accountClosureService =
                scope.ServiceProvider
                    .GetRequiredService<AccountClosureService>();

            AuthService authService =
                scope.ServiceProvider
                    .GetRequiredService<AuthService>();

            IReadOnlyList<AccountClosureOption> options =
                await accountClosureService
                    .GetClosureOptionsAsync(customerId);

            Console.Clear();
            Console.WriteLine("===== REQUEST ACCOUNT CLOSURE =====");
            Console.WriteLine();

            if (options.Count == 0)
            {
                ShowMessage("No open accounts were found.");
                return;
            }

            for (int index = 0;
                index < options.Count;
                index++)
            {
                AccountClosureOption option = options[index];

                Console.WriteLine(
                    $"{index + 1}. {option.AccountType} " +
                    $"{option.MaskedAccountNumber} — " +
                    $"{option.Balance:C} — " +
                    $"{GetClosureEligibilityText(option)}");
            }

            Console.WriteLine();

            List<AccountClosureOption> eligibleOptions =
                options
                    .Where(option => option.IsEligible)
                    .ToList();

            if (eligibleOptions.Count == 0)
            {
                ShowMessage(
                    "No accounts are currently eligible for closure.");
                return;
            }

            AccountClosureOption selectedOption;

            if (eligibleOptions.Count == 1)
            {
                selectedOption = eligibleOptions[0];

                Console.WriteLine(
                    "Selected: " +
                    $"{selectedOption.AccountType} " +
                    selectedOption.MaskedAccountNumber);
            }
            else
            {
                selectedOption =
                    ReadEligibleClosureOption(options);
            }

            Console.WriteLine();

            string password =
                ReadPassword("Re-enter your password: ");

            User? authenticatedUser =
                await authService.AuthenticateAsync(
                    username,
                    password);

            if (authenticatedUser == null)
            {
                ShowMessage("Invalid Credential");
                return;
            }

            bool confirmed = ReadConfirmation(
                "Confirm closure request for " +
                $"{selectedOption.AccountType} " +
                $"{selectedOption.MaskedAccountNumber}? (Y/N): ");

            if (!confirmed)
            {
                ShowMessage(
                    "Account closure request cancelled.");
                return;
            }

            AccountClosureRequest request =
                await accountClosureService
                    .RequestClosureAsync(
                        customerId,
                        selectedOption.AccountId);

            ShowMessage(
                "Account closure request submitted. " +
                $"Request ID: {request.RequestId}");
        }

        private static string GetClosureEligibilityText(
            AccountClosureOption option)
        {
            return option.IsEligible
                ? "Eligible"
                : option.IneligibilityReason ?? "Not eligible";
        }

        private static AccountClosureOption
            ReadEligibleClosureOption(
                IReadOnlyList<AccountClosureOption> options)
        {
            while (true)
            {
                int selection =
                    ReadInt("Select account: ");

                if (selection < 1 ||
                    selection > options.Count)
                {
                    Console.WriteLine("Invalid choice");
                    continue;
                }

                AccountClosureOption option =
                    options[selection - 1];

                if (!option.IsEligible)
                {
                    Console.WriteLine(
                        option.IneligibilityReason ??
                        "Account is not eligible for closure.");
                    continue;
                }

                return option;
            }
        }

        private static async Task ChangePasswordAsync(
            ServiceProvider serviceProvider,
            string username)
        {
            string currentPassword =
                ReadPassword("Current Password: ");
            string newPassword =
                ReadPassword("New Password: ");
            string confirmation =
                ReadPassword("Confirm New Password: ");

            if (newPassword != confirmation)
            {
                throw new InvalidOperationException(
                    "New passwords do not match.");
            }

            using IServiceScope scope =
                serviceProvider.CreateScope();

            AuthService authService =
                scope.ServiceProvider
                    .GetRequiredService<AuthService>();

            await authService.ChangePasswordAsync(
                username,
                currentPassword,
                newPassword);

            ShowMessage("Password changed successfully.");
        }

        private static async Task RunAdminLoginAsync(
            ServiceProvider serviceProvider)
        {
            Console.Clear();
            Console.WriteLine("===== ADMIN LOGIN =====");
            Console.WriteLine();

            string username =
                ReadRequired("Username: ");
            string password =
                ReadPassword("Password: ");

            int adminUserId;

            try
            {
                using IServiceScope scope =
                    serviceProvider.CreateScope();

                AuthService authService =
                    scope.ServiceProvider
                        .GetRequiredService<AuthService>();
                AuthorizationService authorizationService =
                    scope.ServiceProvider
                        .GetRequiredService<AuthorizationService>();

                User? user = await authService
                    .AuthenticateAsync(username, password);

                if (user == null ||
                    !await authorizationService
                        .IsAdminAsync(user.UserId))
                {
                    ShowMessage("Invalid Credential");
                    return;
                }

                adminUserId = user.UserId;
            }
            catch (Exception ex)
            {
                ShowMessage(
                    $"Admin login could not be completed: {ex.Message}");
                return;
            }

            await RunAdminMenuAsync(
                serviceProvider,
                adminUserId);
        }

        private static async Task RunAdminMenuAsync(
            ServiceProvider serviceProvider,
            int adminUserId)
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("===== ADMIN MENU =====");
                Console.WriteLine();
                Console.WriteLine("1. Create New Account");
                Console.WriteLine("2. Delete Account");
                Console.WriteLine("3. Edit Account Details");
                Console.WriteLine("4. Display Summary");
                Console.WriteLine("5. Reset Customer Password");
                Console.WriteLine("6. Approve Cheque Book Request");
                Console.WriteLine("7. Approve Account Closure Request");
                Console.WriteLine("8. Exit");
                Console.WriteLine();
                Console.Write("Select an option: ");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await CreateAccountAsync(
                                serviceProvider,
                                adminUserId);
                            break;

                        case "2":
                            await CloseAccountAsync(
                                serviceProvider,
                                adminUserId);
                            break;

                        case "3":
                            await EditAccountAsync(
                                serviceProvider,
                                adminUserId);
                            break;

                        case "4":
                            await ShowBankingSummaryAsync(
                                serviceProvider,
                                adminUserId);
                            break;

                        case "5":
                            await ResetCustomerPasswordAsync(
                                serviceProvider,
                                adminUserId);
                            break;

                        case "6":
                            await ApproveCheckbookRequestAsync(
                                serviceProvider,
                                adminUserId);
                            break;

                        case "7":
                            await ApproveAccountClosureRequestAsync(
                                serviceProvider,
                                adminUserId);
                            break;

                        case "8":
                            return;

                        default:
                            ShowMessage("Invalid choice");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message);
                }
            }
        }

        private static async Task CreateAccountAsync(
            ServiceProvider serviceProvider,
            int adminUserId)
        {
            int customerId =
                ReadInt("Customer ID: ");

            Console.WriteLine("1. Checking Account");
            Console.WriteLine("2. Savings Account");
            Console.Write("Select account type: ");

            string? accountType = Console.ReadLine();
            decimal openingBalance =
                ReadDecimal("Opening Balance: ");

            using IServiceScope scope =
                serviceProvider.CreateScope();

            AdminService adminService =
                scope.ServiceProvider
                    .GetRequiredService<AdminService>();

            Account account;

            switch (accountType)
            {
                case "1":
                    decimal overdraftLimit =
                        ReadDecimal("Overdraft Limit: ");

                    account = await adminService
                        .CreateCheckingAccountAsync(
                            adminUserId,
                            customerId,
                            openingBalance,
                            overdraftLimit);
                    break;

                case "2":
                    decimal annualInterestRate =
                        ReadDecimal(
                            "Annual Interest Rate (0.05 = 5%): ");
                    decimal minimumBalance =
                        ReadDecimal("Minimum Balance: ");

                    account = await adminService
                        .CreateSavingsAccountAsync(
                            adminUserId,
                            customerId,
                            openingBalance,
                            annualInterestRate,
                            minimumBalance);
                    break;

                default:
                    throw new InvalidOperationException(
                        "Invalid account type.");
            }

            ShowMessage(
                $"Account created successfully. Account Number: {account.AccountNumber}");
        }

        private static async Task CloseAccountAsync(
            ServiceProvider serviceProvider,
            int adminUserId)
        {
            string accountNumber =
                ReadRequired("Account Number: ");

            using IServiceScope scope =
                serviceProvider.CreateScope();

            AdminService adminService =
                scope.ServiceProvider
                    .GetRequiredService<AdminService>();

            await adminService.CloseAccountAsync(
                adminUserId,
                accountNumber);

            ShowMessage("Account closed successfully.");
        }

        private static async Task EditAccountAsync(
            ServiceProvider serviceProvider,
            int adminUserId)
        {
            string accountNumber =
                ReadRequired("Account Number: ");

            using IServiceScope scope =
                serviceProvider.CreateScope();

            AdminService adminService =
                scope.ServiceProvider
                    .GetRequiredService<AdminService>();

            Account account = await adminService.GetAccountAsync(
                adminUserId,
                accountNumber);

            if (account is CheckingAccount)
            {
                decimal overdraftLimit =
                    ReadDecimal("New Overdraft Limit: ");

                await adminService.UpdateCheckingAccountAsync(
                    adminUserId,
                    accountNumber,
                    overdraftLimit);
            }
            else if (account is SavingsAccount)
            {
                decimal annualInterestRate =
                    ReadDecimal(
                        "New Annual Interest Rate (0.05 = 5%): ");
                decimal minimumBalance =
                    ReadDecimal("New Minimum Balance: ");

                await adminService.UpdateSavingsAccountAsync(
                    adminUserId,
                    accountNumber,
                    annualInterestRate,
                    minimumBalance);
            }
            else
            {
                throw new InvalidOperationException(
                    "The account type cannot be edited.");
            }

            ShowMessage("Account details updated successfully.");
        }

        private static async Task ShowBankingSummaryAsync(
            ServiceProvider serviceProvider,
            int adminUserId)
        {
            using IServiceScope scope =
                serviceProvider.CreateScope();

            AdminService adminService =
                scope.ServiceProvider
                    .GetRequiredService<AdminService>();

            AdminBankingSummary summary =
                await adminService
                    .GetBankingSummaryAsync(adminUserId);

            Console.Clear();
            Console.WriteLine("===== BANKING SUMMARY =====");
            Console.WriteLine();
            Console.WriteLine(
                $"Total Accounts: {summary.TotalAccounts}");
            Console.WriteLine(
                $"Active Accounts: {summary.ActiveAccounts}");
            Console.WriteLine(
                $"Frozen Accounts: {summary.FrozenAccounts}");
            Console.WriteLine(
                $"Closed Accounts: {summary.ClosedAccounts}");
            Console.WriteLine(
                $"Total Balance: {summary.TotalBalance:C}");

            WaitForEnter();
        }

        private static async Task ResetCustomerPasswordAsync(
            ServiceProvider serviceProvider,
            int adminUserId)
        {
            string username =
                ReadRequired("Customer Username: ");
            string newPassword =
                ReadPassword("New Password: ");
            string confirmation =
                ReadPassword("Confirm New Password: ");

            if (newPassword != confirmation)
            {
                throw new InvalidOperationException(
                    "New passwords do not match.");
            }

            using IServiceScope scope =
                serviceProvider.CreateScope();

            AdminService adminService =
                scope.ServiceProvider
                    .GetRequiredService<AdminService>();

            await adminService.ResetCustomerPasswordAsync(
                adminUserId,
                username,
                newPassword);

            ShowMessage("Customer password reset successfully.");
        }

        private static async Task ApproveCheckbookRequestAsync(
            ServiceProvider serviceProvider,
            int adminUserId)
        {
            int requestId =
                ReadInt("Cheque Book Request ID: ");

            using IServiceScope scope =
                serviceProvider.CreateScope();

            AdminService adminService =
                scope.ServiceProvider
                    .GetRequiredService<AdminService>();

            await adminService.ApproveCheckbookRequestAsync(
                adminUserId,
                requestId);

            ShowMessage("Cheque book request approved.");
        }

        private static async Task
            ApproveAccountClosureRequestAsync(
                ServiceProvider serviceProvider,
                int adminUserId)
        {
            using IServiceScope scope =
                serviceProvider.CreateScope();

            AdminService adminService =
                scope.ServiceProvider
                    .GetRequiredService<AdminService>();

            IReadOnlyList<AccountClosureRequest> requests =
                await adminService
                    .GetPendingAccountClosureRequestsAsync(
                        adminUserId);

            if (requests.Count == 0)
            {
                ShowMessage(
                    "No pending account closure requests were found.");
                return;
            }

            Console.Clear();
            Console.WriteLine(
                "===== PENDING ACCOUNT CLOSURE REQUESTS =====");
            Console.WriteLine();

            foreach (AccountClosureRequest request in requests)
            {
                Console.WriteLine(
                    $"Request ID: {request.RequestId}");
                Console.WriteLine(
                    $"Customer ID: {request.CustomerId}");
                Console.WriteLine(
                    $"Account ID: {request.AccountId}");
                Console.WriteLine(
                    $"Requested: {request.RequestedDate:g}");
                Console.WriteLine(new string('-', 40));
            }

            int requestId =
                ReadInt("Account Closure Request ID: ");

            await adminService
                .ApproveAccountClosureRequestAsync(
                    adminUserId,
                    requestId);

            ShowMessage("Account closed successfully.");
        }

        private static string ReadRequired(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                string value =
                    Console.ReadLine()?.Trim() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }

                Console.WriteLine("Value cannot be empty.");
            }
        }

        private static string ReadValidated(
            string prompt,
            Func<string, string> validate)
        {
            while (true)
            {
                string value = ReadRequired(prompt);

                try
                {
                    return validate(value);
                }
                catch (Exception ex) when (
                    ex is ArgumentException ||
                    ex is System.ComponentModel.DataAnnotations
                        .ValidationException ||
                    ex is InvalidOperationException)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static async Task<string> ReadValidatedAsync(
            string prompt,
            Func<string, Task<string>> validate)
        {
            while (true)
            {
                string value = ReadRequired(prompt);

                try
                {
                    return await validate(value);
                }
                catch (Exception ex) when (
                    ex is ArgumentException ||
                    ex is System.ComponentModel.DataAnnotations
                        .ValidationException ||
                    ex is InvalidOperationException)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static string ReadValidatedPassword(
            string prompt,
            Func<string, string> validate)
        {
            while (true)
            {
                string value = ReadPassword(prompt);

                try
                {
                    return validate(value);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void ReadPasswordConfirmation(
            string password)
        {
            while (true)
            {
                string confirmation =
                    ReadPassword("Confirm Password: ");

                if (confirmation == password)
                {
                    return;
                }

                Console.WriteLine("Passwords do not match.");
            }
        }

        private static string ReadPassword(string prompt)
        {
            Console.Write(prompt);

            var password = new List<char>();

            while (true)
            {
                ConsoleKeyInfo key =
                    Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Count > 0)
                    {
                        password.RemoveAt(password.Count - 1);
                        Console.Write("\b \b");
                    }

                    continue;
                }

                if (!char.IsControl(key.KeyChar))
                {
                    password.Add(key.KeyChar);
                    Console.Write("*");
                }
            }

            return new string(password.ToArray());
        }

        private static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                if (DateTime.TryParse(
                    Console.ReadLine(),
                    out DateTime value))
                {
                    return value;
                }

                Console.WriteLine("Please enter a valid date.");
            }
        }

        private static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                if (decimal.TryParse(
                    Console.ReadLine(),
                    out decimal value))
                {
                    return value;
                }

                Console.WriteLine("Please enter a valid amount.");
            }
        }

        private static decimal ReadValidatedDecimal(
            string prompt,
            Func<decimal, decimal> validate)
        {
            while (true)
            {
                decimal value = ReadDecimal(prompt);

                try
                {
                    return validate(value);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static AccountType
            ReadRegistrationAccountType()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Choose Account Type");
                Console.WriteLine(
                    $"1. Checking (minimum opening deposit " +
                    $"{AccountOpeningPolicy.CheckingMinimumOpeningBalance:C})");
                Console.WriteLine(
                    $"2. Savings (minimum opening deposit " +
                    $"{AccountOpeningPolicy.SavingsMinimumOpeningBalance:C})");
                Console.Write("Select an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        return AccountType.Checking;

                    case "2":
                        return AccountType.Savings;

                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }
            }
        }

        private static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                if (int.TryParse(
                    Console.ReadLine(),
                    out int value))
                {
                    return value;
                }

                Console.WriteLine("Please enter a valid number.");
            }
        }

        private static bool ReadConfirmation(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                string value =
                    Console.ReadLine()?.Trim() ?? "";

                if (value.Equals(
                    "Y",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (value.Equals(
                    "N",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                Console.WriteLine(
                    "Please enter Y or N.");
            }
        }

        private static void ShowMessage(string message)
        {
            Console.WriteLine();
            Console.WriteLine(message);
            WaitForEnter();
        }

        private static void WaitForEnter()
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}
