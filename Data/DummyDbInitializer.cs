using System;
using System.Linq;
using adeeb.Data;
using adeeb.Models;

namespace AdeebBackend.Data
{
    public class DummyDbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            // Ensure the database is created
            context.Database.EnsureCreated();

            // Check if the database is already seeded
            if (context.Employees.Any())
            {
                return; // Database already seeded
            }

            // Create a dummy company
            var company = new Company
            {
                Id = 1, // Set explicitly if needed; otherwise, let EF generate the Id
                Name = "Test Company",
                LogoUrl = "http://example.com/logo.png",
                TotalNumberOfEmployees = 3, // Updated count
                BundleType = BundleType.Basic
            };
            context.Companies.Add(company);
            context.SaveChanges();

            // Create dummy employees
            var employees = new Employee[]
            {
                new Employee
                {
                    Id = 1,
                    CompanyId = company.Id,  // Reference the company
                    FullName = "John Doe",
                    Email = "john.doe@example.com",
                    JoinDate = new DateTime(2020, 1, 1),
                    Department = "Sales",
                    Position = "Sales Executive",
                    PhoneNumber = "+966500000001"
                },
                new Employee
                {
                    Id = 2,
                    CompanyId = company.Id,  // Reference the company
                    FullName = "Jane Smith",
                    Email = "jane.smith@example.com",
                    JoinDate = new DateTime(2019, 5, 15),
                    Department = "Marketing",
                    Position = "Marketing Manager",
                    PhoneNumber = "+966500000002"
                },
                new Employee
                {
                    Id = 3,  // 🔥 New employee added
                    CompanyId = company.Id,
                    FullName = "Mohammad Makki",
                    Email = "mmmakki9@gmail.com",
                    JoinDate = DateTime.UtcNow, // Assume today is the join date
                    Department = "Engineering",
                    Position = "Software Engineer",
                    PhoneNumber = "+966554337339"
                }
            };

            context.Employees.AddRange(employees);
            context.SaveChanges();

            // Create a dummy survey
            var surveys = new Survey[]
            {
                new Survey
                {
                    Id = 1,
                    CompanyId = company.Id,
                    Title = "Employee Satisfaction Survey",
                    Description = "We want to know your opinions",
                    CreatedAt = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(30)
                }
            };
            context.Surveys.AddRange(surveys);
            context.SaveChanges();

            // Create questions for the survey
            var questions = new Question[]
            {
                new Question
                {
                    Id = 1,
                    SurveyId = 1,
                    Text = "How satisfied are you with your job?",
                    QuestionType = QuestionType.RatingQuestion
                },
                new Question
                {
                    Id = 2,
                    SurveyId = 1,
                    Text = "What can be improved in your work environment?",
                    QuestionType = QuestionType.TextQuestion
                }
            };
            context.Questions.AddRange(questions);
            context.SaveChanges();

            // Create employee survey assignments
            var employeeSurveyLinks = new EmployeeSurveyLink[]
            {
                new EmployeeSurveyLink
                {
                    Id = 1,
                    EmployeeId = 1,
                    SurveyId = 1,
                    UniqueLink = "http://localhost:3000/survey/1",
                    SentAt = DateTime.UtcNow,
                    IsCompleted = false
                },
                new EmployeeSurveyLink
                {
                    Id = 2,
                    EmployeeId = 2,
                    SurveyId = 1,
                    UniqueLink = "http://localhost:3000/survey/2",
                    SentAt = DateTime.UtcNow,
                    IsCompleted = false
                },
                new EmployeeSurveyLink
                {
                    Id = 3,  // 🔥 Assign survey to Mohammad Makki
                    EmployeeId = 3,
                    SurveyId = 1,
                    UniqueLink = "http://localhost:3000/survey/3",
                    SentAt = DateTime.UtcNow,
                    IsCompleted = false
                }
            };

            context.EmployeeSurveyLinks.AddRange(employeeSurveyLinks);
            context.SaveChanges();
        }
    }
}
