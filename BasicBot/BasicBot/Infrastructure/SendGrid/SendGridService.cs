using BasicBot.Common.Constants;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace BasicBot.Infrastructure.SendGrid
{
    public class SendGridService : ISendGridService
    {
        IConfiguration _configuration;

        public SendGridService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Execute(
            string toEmail,
            string toName,
            string subject,
            string plainTextContent,
            string htmlContent
        )
        {
            var apiKey = _configuration["SendGridAPIKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(Constants.BOT_EMAIL, Constants.BOT_NAME);
            var to = new EmailAddress(toEmail, toName);

            var email = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(email);

            if (response.StatusCode.ToString().ToLower() == "unauthorized")
                return false;

            return true;
        }
    }
}
