using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;

namespace adeeb.Services;

public class TwilioEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;
    private readonly string _fromEmail;

    public TwilioEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _apiKey = configuration["SendGrid:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "SendGrid:ApiKey is not configured");
        _fromEmail = configuration["SendGrid:FromEmail"] ?? throw new ArgumentNullException(nameof(configuration), "SendGrid:FromEmail is not configured");
    }

    public async Task SendEmailAsync(string toEmail, string subject, string content)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(_fromEmail);
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
        await client.SendEmailAsync(msg);
    }
}
