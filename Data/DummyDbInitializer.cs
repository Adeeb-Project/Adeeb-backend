using System;
using adeeb.Data;
using adeeb.Models;

namespace AdeebBackend.Data;

public class DummyDbInitializer
{

    public static void Seed(AppDbContext context)
    {
        // Ensure the database is created
        context.Database.EnsureCreated();

        // Check if data already exists to avoid duplicate seeding
        if (context.Employees.Any())
        {
            return; // DB has been seeded
        }

        // Create dummy employees
        var employees = new Employee[]
        {
            new Employee
            {
                Id = 1,
                FullName = "John Doe",
                Email = "john.doe@example.com",
                JoinDate = new DateTime(2020, 1, 1),
                Department = "Sales",
                Position = "Sales Executive"
            },
            new Employee
            {
                Id = 2,
                FullName = "Jane Smith",
                Email = "jane.smith@example.com",
                JoinDate = new DateTime(2019, 5, 15),
                Department = "Marketing",
                Position = "Marketing Manager"
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
            }
        };
        context.EmployeeSurveyLinks.AddRange(employeeSurveyLinks);
        context.SaveChanges();
    }

}
