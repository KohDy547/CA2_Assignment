using CA2_Assignment.Configurations;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace CA2_Assignment.Services
{
    public class EmailService : IEmailSender
    {
        private readonly App_SharedSettings _App_SharedSettings;

        public EmailService(IOptions<App_SharedSettings> App_SharedSettings)
        {
            _App_SharedSettings = App_SharedSettings.Value;
        }

        public Task SendEmailAsync(string inputEmail, string inputSubject, string inputHtmlMessage)
        {
            SendGridClient client = new SendGridClient(_App_SharedSettings.AuthMsg_SendGridKey);
            SendGridMessage message = new SendGridMessage()
            {
                From = new EmailAddress(
                    _App_SharedSettings.AuthMsg_ServerEmail,
                    _App_SharedSettings.AuthMsg_ServerEmailName),
                Subject = inputSubject,
                PlainTextContent = inputHtmlMessage,
                HtmlContent = inputHtmlMessage
            };
            message.AddTo(new EmailAddress(inputEmail));
            message.SetClickTracking(false, false);

            return client.SendEmailAsync(message);
        }
    }
}
