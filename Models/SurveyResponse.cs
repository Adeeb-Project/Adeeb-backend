using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace adeeb.Models
{
    public class SurveyResponse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public required string Response { get; set; }

        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }

        public int SurveyId { get; set; }
        [ForeignKey("SurveyId")]
        public Survey? Survey { get; set; }

        public int EmployeeId { get; set; } // You can reference an Employee model if needed
        public DateTime SubmittedAt { get; set; }
    }
}
