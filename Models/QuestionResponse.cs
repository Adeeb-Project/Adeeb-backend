using System;
using adeeb.Models;

namespace AdeebBackend.Models;



public class QuestionResponse
{
    public int Id { get; set; }

    public string Answer { get; set; }

    public int QuestionId { get; set; }
    public Question Question { get; set; }

    public int SurveyResponseId { get; set; }
    public SurveyResponse SurveyResponse { get; set; }
}
