using System;

namespace AdeebBackend.DTOs;

public class SurveyDto
{
    public int SurveyId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<QuestionDto> Questions { get; set; }
}
