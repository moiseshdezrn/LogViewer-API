using LogViewer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogViewer.Infrastructure.EFConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("Users", "dbo");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .IsRequired();

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .IsRequired();

            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .IsRequired();

            entity.HasIndex(e => e.Username)
                .HasDatabaseName("IX_Users_Username")
                .IsUnique();

            entity.HasIndex(e => e.Email)
                .HasDatabaseName("IX_Users_Email")
                .IsUnique();

            entity.HasData(new User
            {
                Id = 1,
                Username = "Sample username",
                Email = "sample@test.com",
                Password = "$2a$12$diCUORXSxfHmtylUbv05YOHyJf.SNVKKKDkciiwGWiSmFH9//eqTa"
            });
        }
    }
}
