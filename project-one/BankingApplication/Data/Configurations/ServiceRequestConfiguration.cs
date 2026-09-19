using BankingApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.Data.Configurations
{
    public class ServiceRequestConfiguration
        : IEntityTypeConfiguration<ServiceRequest>
    {
        public void Configure(
            EntityTypeBuilder<ServiceRequest> builder)
        {
            builder.ToTable("ServiceRequests");

            builder.HasKey(r => r.RequestId);

            builder
                .HasOne<Customer>()
                .WithMany()
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasDiscriminator<string>("RequestKind")
                .HasValue<CheckbookRequest>("Checkbook")
                .HasValue<DebitCardReplacementRequest>(
                    "DebitCardReplacement")
                .HasValue<AccountStatementRequest>(
                    "AccountStatement")
                .HasValue<AddressChangeRequest>("AddressChange");

            builder.Property<string>("RequestKind")
                .HasMaxLength(30);
        }
    }

    public class AccountServiceRequestConfiguration
        : IEntityTypeConfiguration<AccountServiceRequest>
    {
        public void Configure(
            EntityTypeBuilder<AccountServiceRequest> builder)
        {
            builder
                .HasOne<Account>()
                .WithMany()
                .HasForeignKey(r => r.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class AccountStatementRequestConfiguration
        : IEntityTypeConfiguration<AccountStatementRequest>
    {
        public void Configure(
            EntityTypeBuilder<AccountStatementRequest> builder)
        {
            builder.Property(r => r.StartDate)
                .HasColumnType("date");

            builder.Property(r => r.EndDate)
                .HasColumnType("date");
        }
    }

    public class AddressChangeRequestConfiguration
        : IEntityTypeConfiguration<AddressChangeRequest>
    {
        public void Configure(
            EntityTypeBuilder<AddressChangeRequest> builder)
        {
            builder.OwnsOne(
                r => r.NewAddress,
                address =>
                {
                    address.Property(a => a.Street)
                        .HasColumnName("NewAddressStreet")
                        .HasMaxLength(100)
                        .IsRequired();

                    address.Property(a => a.City)
                        .HasColumnName("NewAddressCity")
                        .HasMaxLength(50)
                        .IsRequired();

                    address.Property(a => a.State)
                        .HasColumnName("NewAddressState")
                        .HasMaxLength(2)
                        .IsRequired();

                    address.Property(a => a.Country)
                        .HasColumnName("NewAddressCountry")
                        .HasMaxLength(50)
                        .IsRequired();

                    address.Property(a => a.ZipCode)
                        .HasColumnName("NewAddressZipCode")
                        .HasMaxLength(10)
                        .IsRequired();
                });

            builder.Navigation(r => r.NewAddress)
                .IsRequired();
        }
    }
}
