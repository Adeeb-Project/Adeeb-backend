using System;

namespace AdeebBackend.DTOs;

public class SurveySubmissionDto
{
    public int? SurveyId { get; set; }
    public List<QuestionSubmissionDto> QuestionsANswers { get; set; } = new List<QuestionSubmissionDto>();
}
