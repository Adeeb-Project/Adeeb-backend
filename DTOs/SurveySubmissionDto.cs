using System;

namespace AdeebBackend.DTOs;

public class SurveySubmissionDto
{
    public int? SurveyId { get; set; }
    public List<QuestionSubmissionDto> QuestionsAnswers { get; set; } = new List<QuestionSubmissionDto>();
}
