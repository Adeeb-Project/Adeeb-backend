namespace adeeb.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime JoinDate { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }

        // Foreign key to Company
        public int CompanyId { get; set; }

        // Navigation property
        public Company Company { get; set; }

        // Navigation Property to SurveyResponse
        public ICollection<SurveyResponse> SurveyResponses { get; set; }
    }
}
