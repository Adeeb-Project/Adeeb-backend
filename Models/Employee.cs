using System.ComponentModel.DataAnnotations;

namespace adeeb.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime JoinDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Department { get; set; }

        [Required]
        [StringLength(50)]
        public string Position { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public DateTime? ExitDate { get; set; }

        // Navigation Property to SurveyResponse
        public ICollection<SurveyResponse> SurveyResponses { get; set; } = new List<SurveyResponse>();
    }
}
