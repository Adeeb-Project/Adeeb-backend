using System;
using adeeb.Data;
using AdeebBackend.DTOs;
using AdeebBackend.DTOs.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;

namespace AdeebBackend.Services;



public class ChatGptService
{
    private readonly ChatClient _client;
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public ChatGptService(IConfiguration configuration, AppDbContext context, ChatClient client)

    {
        _configuration = configuration;
        var apiKey = _configuration["OpenAI:ApiKey"];
        _context = context;
        _client = client;
    }

    public async Task<ServiceResult<string>> RefineSurveyQuestion(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return ServiceResult<string>.BadRequest("Question cannot be empty.");
        }

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

        return ServiceResult<string>.Ok(completion.Content[0].Text);
    }

    public async Task<ServiceResult<SurveyAnalysisResponseDto>> SurveyResponsesAnalysis(int surveyId, int companyId)
    {
        var exists = await _context.Surveys.AnyAsync(s => s.Id == surveyId);
        if (!exists)
        {
            return ServiceResult<SurveyAnalysisResponseDto>.NotFound("Survey not found.");
        }

        var authorizedCompany = await _context.Surveys.AnyAsync(s => s.Id == surveyId && s.CompanyId == companyId);
        if (!authorizedCompany)
        {
            return ServiceResult<SurveyAnalysisResponseDto>
            .Unauthorized("You are not authorized to access this survey it does not belong to your company.");
        }

        var questions = await _context.Questions
            .Where(s => s.SurveyId == surveyId).ToListAsync();

        var questionResponses = await _context.QuestionResponses
            .Where(q => q.Question.SurveyId == surveyId)
            .ToListAsync();

        if (questionResponses.Count == 0)
        {
            return ServiceResult<SurveyAnalysisResponseDto>.NotFound("No responses found for this survey.");
        }

        var surveyAnalysisResponseDto = new SurveyAnalysisResponseDto
        {
            SurveyId = surveyId,
            Questions = new List<SurveyAnalysisSingleResponseDto>()
        };

        questions.ForEach(itemQuestion =>
        {
            var promptQuestionTitle = itemQuestion.Text;
            var questionResponsesList = questionResponses
                .Where(q => q.QuestionId == itemQuestion.Id)
                .Select(q => q.Answer).ToList();

            var prompt = $$"""
                You are an HR employee who wants the best for his company. 
                Analyze the following exit survey question and its responses.
                Question: {{promptQuestionTitle}}
                Responses: {{string.Join(", ", questionResponsesList)}}
                Provide for each question the summary & analysis of it responses.
                Ensure that you make text responses anonymous and do not include any personal information.
                Change the tone of the answer if you think it is not professional.
                Change the tone and signature of the answer if you feel like the person who wrote the answer
                can be detected.
                Do not add any extra information or context.
                Just return the question title and one summary & analysis of it responses gathered up as one asnwer from your side.
                If you can make your answer shorter do it, but do not make it too short, we care about the details of what is happening behind the company and if there is a real issue. But still we need it to short so that the reader does not get bored.
                Do not bring up each response separately.
                If the responses were numbers, do not analyze them separately.
                If the reponses were numbers, write the average, the min, and the max. and a short summary analysis.
                Do not write the question title in the answer and do not write any sort of template or boiler plate, just return your summary analysis answer as I would display it directly for customers.
                Group the responses and analyze them under the question title.
                Make it in paragraph style or add spacing between the paragraphs. We want it to be readable for an average human.
                """
            ;

            ChatCompletion completion = _client.CompleteChatAsync(prompt).Result;

            var questionAnalysis = new SurveyAnalysisSingleResponseDto
            {
                QuestionId = itemQuestion.Id,
                QuestionText = promptQuestionTitle,
                Analysis = completion.Content[0].Text
            };

            surveyAnalysisResponseDto.Questions.Add(questionAnalysis);



        });





        return ServiceResult<SurveyAnalysisResponseDto>.Ok(surveyAnalysisResponseDto);
    }
}

