using System;
using System.Net;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace EmailHandlerApi
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toName, string toEmail, string subject, string message);
    }
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private IOptions<EmailConfig> _emailConfig;

        public EmailService(SmtpClient smtpClient, IOptions<EmailConfig> emailConfig)
        {
            this._smtpClient = smtpClient;
            this._emailConfig = emailConfig;
        }

        public async Task SendEmailAsync(string toName, string toEmail, string subject, string message)
        {
            try
            {
                var emailMessage = new MimeMessage();
                var bodyBuilder = new BodyBuilder();
                

                emailMessage.From.Add(new MailboxAddress(_emailConfig.Value.FromName, _emailConfig.Value.FromAddress));
                emailMessage.To.Add(new MailboxAddress(toName, toEmail));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(TextFormat.Html) { Text = message };
                bodyBuilder.HtmlBody = "<h1>test</h1>";

                 _smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await _smtpClient.ConnectAsync(_smtpClient.LocalDomain, Convert.ToInt32(_emailConfig.Value.MailServerPort), SecureSocketOptions.Auto).ConfigureAwait(false);
                await _smtpClient.AuthenticateAsync(new NetworkCredential(_emailConfig.Value.UserID, _emailConfig.Value.UserPassword));
                await _smtpClient.SendAsync(emailMessage).ConfigureAwait(false);
                await _smtpClient.DisconnectAsync(true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}