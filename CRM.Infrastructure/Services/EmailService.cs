using CRM.Application.Interfaces.Email;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace CRM.Infrastructure.Services
{
    public sealed class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpSetting = _configuration.GetSection("SmtpSettings");

            var from = smtpSetting["From"];
            var host = smtpSetting["Host"];
            var port = int.Parse(smtpSetting["Port"] ?? "25");
            var username = smtpSetting["Username"];
            var password = smtpSetting["Password"];
            var enableSsl = bool.Parse(smtpSetting["EnableSsl"] ?? "false");

            using (var client = new SmtpClient(host, port))
            {
                client.Credentials = new System.Net.NetworkCredential(username, password);
                client.EnableSsl = true;
                client.Timeout = 10000;

                var mailMessage = new MailMessage(from, to, subject, body)
                {
                    IsBodyHtml = true
                };

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
