namespace adeeb.Models
{
    public class SurveyResponse
    {
        public int Id { get; set; }
        public int SurveyId { get; set; } // Foreign Key
        public int EmployeeId { get; set; } // You can reference an Employee model if needed
        public string Response { get; set; }
        public DateTime SubmittedAt { get; set; }

        // Navigation Property to Survey
        public Survey Survey { get; set; }
    }
}
