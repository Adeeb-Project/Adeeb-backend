using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AdeebBackend.Models;
using AdeebBackend.DTOs;

namespace AdeebBackend.Services
{
    public class ChatGPTService : IChatGPTService
    {
        private readonly HttpClient _httpClient;

        // Ideally, store your API key securely using configuration or environment variables.
        private readonly string _apiKey = "";
        // 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
        public ChatGPTService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> SummarizeEmployeesAsync(List<Employee> employees)
        {
            // Compile employee information into a prompt.
            var employeeInfo = new StringBuilder();
            foreach (var emp in employees)
            {
                employeeInfo.AppendLine(
                    $"ID: {emp.ID}, Name: {emp.NAME} {emp.LASTNAME}, Position: {emp.POSITION}, " +
                    $"Gender: {emp.GENDER}, Age: {emp.AGE}, Leave Date: {emp.LEAVEDATE?.ToString("yyyy-MM-dd") ?? "N/A"}, " +
                    $"Years Stayed: {emp.YEARSTAYED}, Email: {emp.EMAIL}");
            }

            string prompt = $"Please provide a detailed summary of the following employees:\n{employeeInfo}";

            return await SendChatGPTRequestAsync(prompt);
        }

        public async Task<string> ImproveQuestionAsync(string question)
        {
            string prompt = $"Please improve the following question for clarity and detail:\n{question}";
            return await SendChatGPTRequestAsync(prompt);
        }

        private async Task<string> SendChatGPTRequestAsync(string prompt)
        {
            var requestBody = new ChatGPTRequest
            {
                Model = "gpt-4",
                Messages = new List<ChatGPTMessage>
                {
                    new ChatGPTMessage { Role = "system", Content = "You are a helpful assistant." },
                    new ChatGPTMessage { Role = "user", Content = prompt }
                }
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            var response = await _httpClient.SendAsync(requestMessage);
            
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var chatResponse = JsonSerializer.Deserialize<ChatGPTResponse>(responseString);
                return chatResponse?.Choices?[0].Message?.Content?.Trim() ?? "No summary returned.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"ChatGPT API error: {errorContent}");
            }
        }
    }
}
