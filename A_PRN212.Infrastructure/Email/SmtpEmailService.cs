using System.Net;
using System.Net.Mail;
using A_PRN212.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace A_PRN212.Infrastructure.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly GmailOptions _options;

        public SmtpEmailService()
        {
            _options = LoadOptions();
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress(_options.Email, "Library Management System");
            var toAddress = new MailAddress(toEmail);

            using var smtp = new SmtpClient
            {
                Host = _options.EffectiveHost,
                Port = _options.Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, _options.Password)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };

            smtp.Send(message);
        }

        private static GmailOptions LoadOptions()
        {
            string basePath = File.Exists(Path.Combine(AppContext.BaseDirectory, "appsettings.json"))
                ? AppContext.BaseDirectory
                : Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var options = new GmailOptions();
            config.GetSection("GmailOptions").Bind(options);
            return options;
        }
    }
}
