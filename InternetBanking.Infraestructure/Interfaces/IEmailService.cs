using InternetBanking.Infraestructure.Core;
using InternetBanking.Infraestructure.Dto;
using InternetBanking.Infraestructure.Settings;

namespace InternetBanking.Infraestructure.Interfaces
{
    public interface IEmailService
    {
        public MailSettings MailSettings { get; }
        Task<NotificationResponse> SendEmailAsync(EmailRequest emailRequest);
    }
}
