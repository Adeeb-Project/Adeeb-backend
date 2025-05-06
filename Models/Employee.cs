namespace adeeb.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime LeftDate { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string PhoneNumber { get; set; }

        // Foreign key to Company
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        // Employee role (Regular employee only)
        public bool CanTakeSurveys => true;

        // Navigation Property to SurveyResponse
        public ICollection<SurveyResponse> SurveyResponses { get; set; }
    }
}
