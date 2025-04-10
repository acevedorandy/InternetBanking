using InternetBanking.Infraestructure.Core;
using InternetBanking.Infraestructure.Dto;
using InternetBanking.Infraestructure.Interfaces;
using InternetBanking.Infraestructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace InternetBanking.Infraestructure.Services
{
    public class EmailService : IEmailService
    {
        public MailSettings MailSettings { get; }

        public EmailService(IOptions<MailSettings> options)

            => MailSettings = options.Value;

        public async Task<NotificationResponse> SendEmailAsync(EmailRequest emailRequest)
        {
            NotificationResponse result = new NotificationResponse();

            try
            {
                var email = CreateEmailMessage(emailRequest);

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(MailSettings.SmtpHost, MailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(MailSettings.SmtpUser, MailSettings.SmtpPass);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                result.Messages = $"Ha ocurrido un error enviando el mail.{ex.Message}";
            }
            return result;
        }

        private MimeMessage CreateEmailMessage(EmailRequest emailRequest)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(emailRequest.From ?? MailSettings.EmailFrom),
                Subject = emailRequest.Subject
            };

            email.To.Add(MailboxAddress.Parse(emailRequest.To));
            email.Body = new BodyBuilder { HtmlBody = emailRequest.Body }.ToMessageBody();

            return email;
        }
    }
}
