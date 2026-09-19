using BankingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Data
{
    public class BankingDbContext : DbContext
    {
        public BankingDbContext(
            DbContextOptions<BankingDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Customer> Customers => Set<Customer>();

        public DbSet<Role> Roles => Set<Role>();

        public DbSet<UserRole> UserRoles => Set<UserRole>();

        public DbSet<Account> Accounts => Set<Account>();

        public DbSet<Transaction> Transactions => Set<Transaction>();

        public DbSet<ServiceRequest> ServiceRequests
            => Set<ServiceRequest>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUsers(modelBuilder);

            ConfigureRoles(modelBuilder);

            ConfigureCustomers(modelBuilder);

            ConfigureAccounts(modelBuilder);

            ConfigureTransactions(modelBuilder);

            ConfigureServiceRequests(modelBuilder);
        }

        private static void ConfigureUsers(
            ModelBuilder modelBuilder)
        {
            var user =
                modelBuilder.Entity<User>();

            user.ToTable("Users");

            user.HasKey(u => u.UserId);

            user.HasIndex(u => u.Username)
                .IsUnique();
        }

        private static void ConfigureRoles(
            ModelBuilder modelBuilder)
        {
            var role =
                modelBuilder.Entity<Role>();

            role.ToTable("Roles");

            role.HasKey(r => r.RoleId);

            role.HasIndex(r => r.RoleName)
                .IsUnique();

            role.HasData(
                new
                {
                    RoleId = 1,
                    RoleName = "Customer"
                },
                new
                {
                    RoleId = 2,
                    RoleName = "Admin"
                });

            var userRole =
                modelBuilder.Entity<UserRole>();

            userRole.ToTable("UserRoles");

            userRole.HasKey(
                ur => new
                {
                    ur.UserId,
                    ur.RoleId
                });

            userRole
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            userRole
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureCustomers(
            ModelBuilder modelBuilder)
        {
            var customer =
                modelBuilder.Entity<Customer>();

            customer.ToTable("Customers");

            customer.HasKey(c => c.CustomerId);

            customer
                .HasIndex(c => c.UserId)
                .IsUnique();

            customer
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Customer>(
                    c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            customer
                .Property(c => c.PhoneNumber)
                .HasMaxLength(10)
                .IsRequired();

            customer
                .Property(c => c.DateOfBirth)
                .HasColumnType("date");

            customer
                .Ignore(c =>
                    c.MaskedSocialSecurityNumber);

            customer
                .Property<string>(
                    "SocialSecurityNumberLastFour")
                .HasField(
                    "_socialSecurityNumberLastFour")
                .HasMaxLength(4)
                .IsRequired();

            customer.OwnsOne(
                c => c.Address,
                address =>
                {
                    address
                        .Property(a => a.Street)
                        .HasColumnName("Street")
                        .HasMaxLength(100)
                        .IsRequired();

                    address
                        .Property(a => a.City)
                        .HasColumnName("City")
                        .HasMaxLength(50)
                        .IsRequired();

                    address
                        .Property(a => a.State)
                        .HasColumnName("State")
                        .HasMaxLength(2)
                        .IsRequired();

                    address
                        .Property(a => a.Country)
                        .HasColumnName("Country")
                        .HasMaxLength(50)
                        .IsRequired();

                    address
                        .Property(a => a.ZipCode)
                        .HasColumnName("ZipCode")
                        .HasMaxLength(10)
                        .IsRequired();
                });

            customer
                .Navigation(c => c.Address)
                .IsRequired();
        }

        private static void ConfigureAccounts(
            ModelBuilder modelBuilder)
        {
            var account =
                modelBuilder.Entity<Account>();

            account.ToTable("Accounts");

            account.HasKey(a => a.AccountId);

            account
                .HasIndex(a => a.AccountNumber)
                .IsUnique();

            account
                .Property(a => a.Balance)
                .HasPrecision(18, 2);

            account
                .Ignore(a => a.AccountType);

            account
                .HasOne<Customer>()
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            account
                .HasDiscriminator<string>(
                    "AccountKind")
                .HasValue<CheckingAccount>(
                    "Checking")
                .HasValue<SavingsAccount>(
                    "Savings");

            var checking =
                modelBuilder.Entity<CheckingAccount>();

            checking
                .Property(c => c.OverdraftLimit)
                .HasPrecision(18, 2);

            checking
                .Ignore(c => c.AvailableFunds);

            var savings =
                modelBuilder.Entity<SavingsAccount>();

            savings
                .Property(s => s.AnnualInterestRate)
                .HasPrecision(9, 6);

            savings
                .Property(s => s.MinimumBalance)
                .HasPrecision(18, 2);
        }

        private static void ConfigureTransactions(
            ModelBuilder modelBuilder)
        {
            var transaction =
                modelBuilder.Entity<Transaction>();

            transaction.ToTable("Transactions");

            transaction.HasKey(
                t => t.TransactionId);

            transaction
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            transaction
                .Property(
                    t => t.BalanceAfterTransaction)
                .HasPrecision(18, 2);

            transaction
                .HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            transaction
                .HasOne<Account>()
                .WithMany()
                .HasForeignKey(
                    t => t.RelatedAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureServiceRequests(
            ModelBuilder modelBuilder)
        {
            var request =
                modelBuilder.Entity<ServiceRequest>();

            request.ToTable("ServiceRequests");

            request.HasKey(
                r => r.RequestId);

            request
                .HasOne<Customer>()
                .WithMany()
                .HasForeignKey(
                    r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            request
                .HasDiscriminator<string>(
                    "RequestKind")
                .HasValue<CheckbookRequest>(
                    "Checkbook")
                .HasValue<DebitCardReplacementRequest>(
                    "DebitCardReplacement")
                .HasValue<AccountStatementRequest>(
                    "AccountStatement")
                .HasValue<AddressChangeRequest>(
                    "AddressChange");

            modelBuilder
                .Entity<CheckbookRequest>()
                .HasOne<Account>()
                .WithMany()
                .HasForeignKey(
                    r => r.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<DebitCardReplacementRequest>()
                .HasOne<Account>()
                .WithMany()
                .HasForeignKey(
                    r => r.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            var statementRequest =
                modelBuilder
                    .Entity<AccountStatementRequest>();

            statementRequest
                .HasOne<Account>()
                .WithMany()
                .HasForeignKey(
                    r => r.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            statementRequest
                .Property(r => r.StartDate)
                .HasColumnType("date");

            statementRequest
                .Property(r => r.EndDate)
                .HasColumnType("date");

            var addressChange =
                modelBuilder
                    .Entity<AddressChangeRequest>();

            addressChange.OwnsOne(
                r => r.NewAddress,
                address =>
                {
                    address
                        .Property(a => a.Street)
                        .HasColumnName(
                            "NewAddressStreet")
                        .HasMaxLength(100)
                        .IsRequired();

                    address
                        .Property(a => a.City)
                        .HasColumnName(
                            "NewAddressCity")
                        .HasMaxLength(50)
                        .IsRequired();

                    address
                        .Property(a => a.State)
                        .HasColumnName(
                            "NewAddressState")
                        .HasMaxLength(2)
                        .IsRequired();

                    address
                        .Property(a => a.Country)
                        .HasColumnName(
                            "NewAddressCountry")
                        .HasMaxLength(50)
                        .IsRequired();

                    address
                        .Property(a => a.ZipCode)
                        .HasColumnName(
                            "NewAddressZipCode")
                        .HasMaxLength(10)
                        .IsRequired();
                });

            addressChange
                .Navigation(r => r.NewAddress)
                .IsRequired();
        }
    }
}