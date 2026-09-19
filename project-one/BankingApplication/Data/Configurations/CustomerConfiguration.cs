using BankingApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.Data.Configurations
{
    public class CustomerConfiguration
        : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.CustomerId);

            builder.HasIndex(c => c.UserId)
                .IsUnique();

            builder
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.PhoneNumber)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(c => c.Email)
                .HasMaxLength(254)
                .IsRequired();

            builder.Property(c => c.DateOfBirth)
                .HasColumnType("date");

            builder.Ignore(c => c.MaskedSocialSecurityNumber);

            builder.Property<string>("_socialSecurityNumberLastFour")
                .HasColumnName("SocialSecurityNumberLastFour")
                .HasMaxLength(4)
                .IsRequired();

            builder.OwnsOne(
                c => c.Address,
                address =>
                {
                    address.Property(a => a.Street)
                        .HasColumnName("Street")
                        .HasMaxLength(100)
                        .IsRequired();

                    address.Property(a => a.City)
                        .HasColumnName("City")
                        .HasMaxLength(50)
                        .IsRequired();

                    address.Property(a => a.State)
                        .HasColumnName("State")
                        .HasMaxLength(2)
                        .IsRequired();

                    address.Property(a => a.Country)
                        .HasColumnName("Country")
                        .HasMaxLength(50)
                        .IsRequired();

                    address.Property(a => a.ZipCode)
                        .HasColumnName("ZipCode")
                        .HasMaxLength(10)
                        .IsRequired();
                });

            builder.Navigation(c => c.Address)
                .IsRequired();
        }
    }
}
