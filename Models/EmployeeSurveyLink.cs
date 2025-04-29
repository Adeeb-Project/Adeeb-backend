using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace adeeb.Models
{
    public class EmployeeSurveyLink
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public required string UniqueLink { get; set; }

        public DateTime SentAt { get; set; }
        public bool IsCompleted { get; set; }

        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        public int SurveyId { get; set; }
        [ForeignKey("SurveyId")]
        public Survey? Survey { get; set; }
    }
}
