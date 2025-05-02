using System;

namespace AdeebBackend.DTOs;

public class SurveyAnalysisResponseDto
{
    public int SurveyId { get; set; }
    public List<SurveyAnalysisSingleResponseDto> Questions { get; set; }

}

public class SurveyAnalysisSingleResponseDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public string Analysis { get; set; }
}
