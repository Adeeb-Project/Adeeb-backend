using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using adeeb.Data;
using adeeb.Models;
using Microsoft.EntityFrameworkCore;

namespace adeeb.Services
{
    public class SurveyAnalysisService
    {
        private readonly AppDbContext _context;

        public SurveyAnalysisService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SurveyAnalysisResult> AnalyzeSurveyResponses(int surveyId)
        {
            var responses = await _context.SurveyResponses
                .Where(r => r.SurveyId == surveyId)
                .ToListAsync();

            var result = new SurveyAnalysisResult();

            // Simple analysis for demonstration
            result.KeyThemes.Add("Work Environment");
            result.KeyThemes.Add("Team Collaboration");
            result.KeyThemes.Add("Professional Development");

            result.SentimentAnalysis = "Overall positive sentiment with some areas for improvement";

            result.ActionableSuggestions.Add("Implement regular team-building activities");
            result.ActionableSuggestions.Add("Create clear career progression paths");
            result.ActionableSuggestions.Add("Improve inter-departmental communication");

            result.AreasForImprovement.Add("Workload management");
            result.AreasForImprovement.Add("Training opportunities");
            result.AreasForImprovement.Add("Cross-department collaboration");

            return result;
        }
    }
} 