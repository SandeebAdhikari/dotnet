using BankingApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.RoleId);

            builder.Property(r => r.RoleName)
                .HasMaxLength(30)
                .IsRequired();

            builder.HasIndex(r => r.RoleName)
                .IsUnique();

            builder.HasData(
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
        }
    }
}
