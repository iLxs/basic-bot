using BasicBot.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BasicBot.Persistence
{
    public interface IDatabaseService
    {
        Task<bool> SaveAsync();
        DbSet<UserModel> Users { get; set; }
        DbSet<QualificationModel> Qualifications { get; set; }
        DbSet<MedicalAppointmentModel> MedicalAppointments { get; set; }
    }
}
