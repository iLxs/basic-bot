using BasicBot.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BasicBot.Persistence
{
    public class DatabaseService : DbContext, IDatabaseService
    {
        public DatabaseService()
        {
            Database.EnsureCreatedAsync();
        }

        public DatabaseService(DbContextOptions options) : base(options)
        {
            Database.EnsureCreatedAsync();
        }

        public DbSet<UserModel> Users { get; set; }

        public async Task<bool> SaveAsync()
        {
            return (await SaveChangesAsync() > 0);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .ToContainer("User")
                .HasPartitionKey("channel")
                .HasNoDiscriminator()
                .HasKey("id");
        }
    }
}
