using System.ComponentModel.DataAnnotations;

namespace adeeb.Models
{
    public class SurveyResponse
    {
        public int Id { get; set; }

        [Required]
        public int SurveyId { get; set; } // Foreign Key

        [Required]
        public int EmployeeId { get; set; } // Foreign Key

        [Required]
        public DateTime SubmittedAt { get; set; }

        // Navigation Property to Survey
        public Survey Survey { get; set; }

        // Navigation Property to Employee
        public Employee Employee { get; set; }

        // Collection of Question Responses
        public ICollection<SurveyQuestionResponse> QuestionResponses { get; set; }
    }
}
