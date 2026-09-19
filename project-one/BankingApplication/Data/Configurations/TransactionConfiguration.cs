using BankingApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.Data.Configurations
{
    public class TransactionConfiguration
        : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.TransactionId);

            builder.Property(t => t.Amount)
                .HasPrecision(18, 2);

            builder.Property(t => t.BalanceAfterTransaction)
                .HasPrecision(18, 2);

            builder
                .HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.RelatedAccountId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
