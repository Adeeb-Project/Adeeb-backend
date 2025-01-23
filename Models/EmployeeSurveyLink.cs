namespace adeeb.Models
{
    public class EmployeeSurveyLink
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; } // You can reference an Employee model here as well
        public int SurveyId { get; set; } // Foreign Key
        public string UniqueLink { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsCompleted { get; set; } // Indicates whether the survey is completed by the employee

        // Navigation Properties
        public Survey Survey { get; set; }
    }
}
