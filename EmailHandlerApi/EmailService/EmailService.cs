using System;
using System.Net;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
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

        public IConfiguration _config { get; }

        public EmailService(SmtpClient smtpClient, IConfiguration configuration)
        {
            this._smtpClient = smtpClient;
            this._config = configuration;
        }

        public async Task SendEmailAsync(string toName, string toEmail, string subject, string message)
        {
            try
            {
                var emailMessage = new MimeMessage();
                var bodyBuilder = new BodyBuilder();

                var fromName = _config.GetValue<string>("EmailConfiguration:FromName");
                var fromEmail = _config.GetValue<string>("EmailConfiguration:FromEmail");

                emailMessage.From.Add(new MailboxAddress(fromName, fromEmail));
                emailMessage.To.Add(new MailboxAddress(toName, toEmail));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(TextFormat.Html) { Text = message };
                bodyBuilder.HtmlBody = "<h1>test</h1>";

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