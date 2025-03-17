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
            // Ensure database is created
            context.Database.EnsureCreated();

            // If Users table already has data, exit
            if (context.Users.Any()) return;

            // Create a test company
            var company = new Company
            {
                Id = 1,
                Name = "Test Company",
                LogoUrl = "http://example.com/logo.png",
                TotalNumberOfEmployees = 3,
                BundleType = BundleType.Basic
            };
            context.Companies.Add(company);
            context.SaveChanges();

            // Create Users (Admin + HR Manager)
            var users = new User[]
            {
                new User
                {
                    Id = 1,
                    Name = "Admin User",
                    Email = "admin@example.com",
                    Password = "hashedpassword", // Replace with hashed password in production
                    CompanyId = company.Id,
                    Role = UserRole.Admin
                },
                new User
                {
                    Id = 2,
                    Name = "HR Manager",
                    Email = "hr@example.com",
                    Password = "hashedpassword", // Replace with hashed password in production
                    CompanyId = company.Id,
                    Role = UserRole.HRManager
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            // Create Employees (Survey Participants)
            var employees = new Employee[]
            {
                new Employee
                {
                    Id = 1,
                    CompanyId = company.Id,
                    FullName = "John Doe",
                    Email = "john.doe@example.com",
                    JoinDate = DateTime.UtcNow,
                    Department = "Sales",
                    Position = "Sales Executive",
                    PhoneNumber = "+966500000001"
                },
                new Employee
                {
                    Id = 2,
                    CompanyId = company.Id,
                    FullName = "Jane Smith",
                    Email = "jane.smith@example.com",
                    JoinDate = DateTime.UtcNow,
                    Department = "Marketing",
                    Position = "Marketing Manager",
                    PhoneNumber = "+966500000002"
                }
            };

            context.Employees.AddRange(employees);
            context.SaveChanges();

            // Create a sample survey
            var survey = new Survey
            {
                Id = 1,
                CompanyId = company.Id,
                Title = "Employee Satisfaction Survey",
                Description = "We want to know your opinions",
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30)
            };
            context.Surveys.Add(survey);
            context.SaveChanges();

            // Create survey questions
            var questions = new Question[]
            {
                new Question
                {
                    Id = 1,
                    SurveyId = survey.Id,
                    Text = "How satisfied are you with your job?",
                    QuestionType = QuestionType.RatingQuestion
                },
                new Question
                {
                    Id = 2,
                    SurveyId = survey.Id,
                    Text = "What can be improved in your work environment?",
                    QuestionType = QuestionType.TextQuestion
                }
            };

            context.Questions.AddRange(questions);
            context.SaveChanges();

            // Assign the survey to employees
            var employeeSurveyLinks = new EmployeeSurveyLink[]
            {
                new EmployeeSurveyLink
                {
                    Id = 1,
                    EmployeeId = 1,
                    SurveyId = survey.Id,
                    UniqueLink = "http://localhost:3000/survey/1",
                    SentAt = DateTime.UtcNow,
                    IsCompleted = false
                },
                new EmployeeSurveyLink
                {
                    Id = 2,
                    EmployeeId = 2,
                    SurveyId = survey.Id,
                    UniqueLink = "http://localhost:3000/survey/2",
                    SentAt = DateTime.UtcNow,
                    IsCompleted = false
                }
            };

            context.EmployeeSurveyLinks.AddRange(employeeSurveyLinks);
            context.SaveChanges();
        }
    }
}
