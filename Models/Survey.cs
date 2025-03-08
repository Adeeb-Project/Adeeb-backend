namespace adeeb.Models
{
    public class Survey
    {
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
