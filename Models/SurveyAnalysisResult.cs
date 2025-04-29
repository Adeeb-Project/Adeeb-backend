namespace adeeb.Models;

public class SurveyAnalysisResult
{
    public List<string> KeyThemes { get; set; } = new List<string>();
    public string SentimentAnalysis { get; set; } = string.Empty;
    public List<string> ActionableSuggestions { get; set; } = new List<string>();
    public List<string> AreasForImprovement { get; set; } = new List<string>();
} 