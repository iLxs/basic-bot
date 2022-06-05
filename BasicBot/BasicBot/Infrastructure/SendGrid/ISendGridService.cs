using System.Threading.Tasks;

namespace BasicBot.Infrastructure.SendGrid
{
    public interface ISendGridService
    {
        Task<bool> Execute(
            string toEmail,
            string toName,
            string subject,
            string plainTextContent,
            string htmlContent
        );
    }
}
