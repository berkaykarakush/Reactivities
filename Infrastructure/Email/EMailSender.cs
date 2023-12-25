using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Email
{
    public class EMailSender
    {
        private IConfiguration _configuration;

        public EMailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string userEmail, string emailSubject, string msg)
        {
            // Create a SendGrid client using the API key from configuration
            var client = new SendGridClient(_configuration["Sendgrid:ApiKey"]);
            // Create a SendGrid message with the specified parameters
            var message = new SendGridMessage
            {

                From = new EmailAddress(_configuration["Sendgrid:Email"], _configuration["Sendgrid:User"]),// Set the sender's email address and name
                Subject = emailSubject, // Set the email subject
                PlainTextContent = msg, // Set the plain text content of the email
                HtmlContent = msg  // Set the HTML content of the email
            };
            // Add the recipient's email address to the message
            message.AddTo(new EmailAddress(userEmail));
            // Disable click tracking for both plain text and HTML content
            message.SetClickTracking(false, false);
            // Send the email using the SendGrid client
            await client.SendEmailAsync(message);
        }
    }
}