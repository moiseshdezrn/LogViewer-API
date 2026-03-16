using Microsoft.EntityFrameworkCore;
using LogViewer.Core.Entities;

namespace LogViewer.Infrastructure.DatabaseContext
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {
        }

        public LogDbContext()
        {
        }

        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogDbContext).Assembly);
        }
    }
}
