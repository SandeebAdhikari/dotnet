using BankingApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.Username)
                .HasMaxLength(30)
                .IsRequired();

            builder.HasIndex(u => u.Username)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .HasMaxLength(256)
                .IsRequired();
        }
    }
}
