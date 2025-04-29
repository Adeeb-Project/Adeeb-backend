using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace adeeb.Models
{
    public class User
    {
        public User()
        {
            // Initialize with a new Company with required properties
            Company = new Company
            {
                Name = string.Empty,
                TotalNumberOfEmployees = 0,
                BundleType = BundleType.Basic
            };
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; } = UserRole.HRManager;  // Default value

        // Foreign key to Company
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
    }

    public enum UserRole
    {
        Admin,    // Developer
        HRManager // Can create surveys, manage employees
    }
}
