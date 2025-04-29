using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace adeeb.Models
{
    public class Survey
    {
        public Survey()
        {
            Questions = new List<Question>();
            EmployeeSurveyLinks = new List<EmployeeSurveyLink>();
            CreatedAt = DateTime.UtcNow;
        }

        [Key]  // Ensures it's the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Enables auto-increment
        public int Id { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

        // Navigation Property to Questions
        public List<Question> Questions { get; set; } = new();
        public List<EmployeeSurveyLink> EmployeeSurveyLinks { get; set; } = new();
    }
}
