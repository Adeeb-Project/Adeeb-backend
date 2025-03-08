using System;
using System.Text;
using System.Text.Json;
using Amazon.S3.Model;

namespace AdeebBackend.Services;

public class TwilioEmailService
{

    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;

    public TwilioEmailService(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        var requestUrl = "https://api.sendgrid.com/v3/mail/send";
        var apiKey = _configuration["TwilioSettings:EmailAPIKey"];
        var fromEmail = _configuration["TwilioSettings:FromEmail"];

        using (var client = _clientFactory.CreateClient())
        {
            var emailPayload = new
            {
                personalizations = new[]
           {
                new { to = new[] { new { email = toEmail } }, subject = subject }
            },
                from = new { email = fromEmail },
                content = new[]
           {
                new { type = "text/plain", value = body }
            }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(emailPayload), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Headers = { { "Authorization", $"Bearer {apiKey}" } },
                Content = jsonContent
            };

            var response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }


    }



}
