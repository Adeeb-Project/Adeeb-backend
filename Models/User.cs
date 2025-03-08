
namespace adeeb.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }

        // Foreign key to Company
        public int CompanyId { get; set; }

        // Navigation property
        public Company Company { get; set; }
    }
}
