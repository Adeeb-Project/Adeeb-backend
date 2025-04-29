using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace adeeb.Models
{
    public class Employee
    {
        public Employee()
        {
            EmployeeSurveyLinks = new List<EmployeeSurveyLink>();
            JoinDate = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public required string FullName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        public DateTime JoinDate { get; set; }

        [Required]
        public required string Department { get; set; }

        [Required]
        public required string Position { get; set; }

        [Required]
        [Phone]
        public required string PhoneNumber { get; set; }

        // Foreign key to Company
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

        // Employee role (Regular employee only)
        public bool CanTakeSurveys => true;

        // Navigation Property to SurveyResponse
        public ICollection<SurveyResponse> SurveyResponses { get; set; }

        public List<EmployeeSurveyLink> EmployeeSurveyLinks { get; set; } = new();
    }
}
