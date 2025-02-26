using System.ComponentModel.DataAnnotations;

namespace adeeb.Models
{
    public class SurveyQuestionResponse
    {
        public int Id { get; set; }

        [Required]
        public int SurveyResponseId { get; set; } // Foreign Key

        [Required]
        public int QuestionId { get; set; } // Foreign Key

        [Required]
        public string Response { get; set; }

        // Navigation Property to SurveyResponse
        public SurveyResponse SurveyResponse { get; set; }
    }
}