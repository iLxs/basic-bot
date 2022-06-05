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
        public DbSet<QualificationModel> Qualifications { get; set; }
        public DbSet<MedicalAppointmentModel> MedicalAppointments { get; set; }

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

            modelBuilder.Entity<QualificationModel>()
                .ToContainer("Qualification")
                .HasPartitionKey("idUser")
                .HasNoDiscriminator()
                .HasKey("id");

            modelBuilder.Entity<MedicalAppointmentModel>()
                .ToContainer("MedicalAppointment")
                .HasPartitionKey("idUser")
                .HasNoDiscriminator()
                .HasKey("id");
        }
    }
}
