namespace adeeb.Models
{
    public class Survey
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiryDate { get; set; }

        // Navigation Property to SurveyResponses
        public ICollection<SurveyResponse> SurveyResponses { get; set; }
    }
}
