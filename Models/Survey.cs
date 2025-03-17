using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace adeeb.Models
{
    public class Survey
    {
        [Key]  // Ensures it's the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Enables auto-increment
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int CompanyId { get; set; }

        // Navigation Property to Questions
        public ICollection<Question> Questions { get; set; }
        public Company Company { get; set; }
    }

}
