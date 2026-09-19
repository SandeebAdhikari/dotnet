using BankingApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.Data.Configurations
{
    public class AccountConfiguration
        : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(a => a.AccountId);

            builder.Property(a => a.AccountNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(a => a.AccountNumber)
                .IsUnique();

            builder.Property(a => a.Balance)
                .HasPrecision(18, 2);

            builder.Ignore(a => a.AccountType);

            builder
                .HasOne<Customer>()
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasDiscriminator<string>("AccountKind")
                .HasValue<CheckingAccount>("Checking")
                .HasValue<SavingsAccount>("Savings");

            builder.Property<string>("AccountKind")
                .HasMaxLength(20);
        }
    }

    public class CheckingAccountConfiguration
        : IEntityTypeConfiguration<CheckingAccount>
    {
        public void Configure(
            EntityTypeBuilder<CheckingAccount> builder)
        {
            builder.Property(c => c.OverdraftLimit)
                .HasPrecision(18, 2);

            builder.Ignore(c => c.AvailableFunds);
        }
    }

    public class SavingsAccountConfiguration
        : IEntityTypeConfiguration<SavingsAccount>
    {
        public void Configure(
            EntityTypeBuilder<SavingsAccount> builder)
        {
            builder.Property(s => s.AnnualInterestRate)
                .HasPrecision(9, 6);

            builder.Property(s => s.MinimumBalance)
                .HasPrecision(18, 2);
        }
    }
}
