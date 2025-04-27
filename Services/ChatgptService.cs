using System;
using OpenAI.Chat;

namespace AdeebBackend.Services;



public class ChatGptService
{
    private readonly ChatClient _client;

    public ChatGptService(string apiKey)
    {
        _client = new ChatClient(
            model: "gpt-4o",
            apiKey: apiKey
        );
    }

    public async Task<string> RefineSurveyQuestion(string prompt)
    {

        ChatCompletion completion = await _client.CompleteChatAsync(
            $$""" 
            You are an HR employee who wants the best for his company. 
            Refine the following exit survey question to be more specific and clear if needed.
            Make it more professional and concise. Only return the refined question.
            Do not add any extra information or context.
            Just return the refined question.
            Question: {{prompt}}
            """
        );

        return completion.Content[0].Text;
    }
}

