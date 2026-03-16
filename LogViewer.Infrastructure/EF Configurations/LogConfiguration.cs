using LogViewer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogViewer.Infrastructure.EFConfigurations
{
    public class LogConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> entity)
        {
            entity.ToTable("Logs", "dbo");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            entity.Property(e => e.Level)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.Message)
                .IsRequired();

            entity.Property(e => e.Source)
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(e => e.ExceptionMessage);

            entity.Property(e => e.StackTrace);

            entity.Property(e => e.UserId)
                .HasMaxLength(128);

            entity.Property(e => e.MachineName)
                .HasMaxLength(128)
                .IsRequired();

            entity.Property(e => e.RequestPath)
                .HasMaxLength(512);

            entity.Property(e => e.RequestMethod)
                .HasMaxLength(10);

            entity.Property(e => e.CorrelationId)
                .HasColumnType("uniqueidentifier");

            entity.Property(e => e.ThreadId);

            entity.Property(e => e.ApplicationName)
                .HasMaxLength(128)
                .IsRequired();

            entity.HasIndex(e => e.Timestamp)
                .HasDatabaseName("IX_Logs_Timestamp")
                .IsDescending();

            entity.HasIndex(e => e.Level)
                .HasDatabaseName("IX_Logs_Level");

            entity.HasIndex(e => e.Source)
                .HasDatabaseName("IX_Logs_Source");
        }
    }
}
