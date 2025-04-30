
using System;
using System.Linq;
using AdeebBackend.Models;
using adeeb.Models;
using adeeb.Data;

namespace AdeebBackend.Data
{
    public class DummyDbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any()) return;

            var company = new Company
            {
                Id = 1,
                Name = "Test Company",
                LogoUrl = "http://example.com/logo.png",
                TotalNumberOfEmployees = 10,
                BundleType = BundleType.Basic
            };
            context.Companies.Add(company);
            context.SaveChanges();

            var users = new User[]
            {
                new User
                {
                    Id = 1,
                    Name = "Admin User",
                    Email = "admin@example.com",
                    Password = "hashedpassword",
                    CompanyId = company.Id,
                    Role = UserRole.Admin
                },
                new User
                {
                    Id = 2,
                    Name = "HR Manager",
                    Email = "hr@example.com",
                    Password = "hashedpassword",
                    CompanyId = company.Id,
                    Role = UserRole.HRManager
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            var employees = Enumerable.Range(1, 10).Select(i => new Employee
            {
                Id = i,
                CompanyId = company.Id,
                FullName = $"Employee {i}",
                Email = $"employee{i}@example.com",
                JoinDate = DateTime.UtcNow.AddMonths(-i),
                Department = i % 2 == 0 ? "Engineering" : "Marketing",
                Position = i % 2 == 0 ? "Software Engineer" : "Marketing Specialist",
                PhoneNumber = $"+96650000000{i}"
            }).ToArray();

            context.Employees.AddRange(employees);
            context.SaveChanges();

            var survey = new Survey
            {
                Id = 1,
                CompanyId = company.Id,
                Title = "Employee Exit Survey",
                Description = "We want your honest feedback to improve.",
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30)
            };
            context.Surveys.Add(survey);
            context.SaveChanges();

            var questions = new Question[]
            {
                new Question { Id = 1, SurveyId = survey.Id, Text = "How would you rate your overall experience with the company?", QuestionType = QuestionType.RatingQuestion },
                new Question { Id = 2, SurveyId = survey.Id, Text = "What was your primary reason for leaving the company?", QuestionType = QuestionType.TextQuestion },
                new Question { Id = 3, SurveyId = survey.Id, Text = "How satisfied were you with your compensation and benefits?", QuestionType = QuestionType.RatingQuestion },
                new Question { Id = 4, SurveyId = survey.Id, Text = "Would you recommend our company as a good place to work?", QuestionType = QuestionType.TextQuestion },
                new Question { Id = 5, SurveyId = survey.Id, Text = "How would you rate your relationship with your direct supervisor?", QuestionType = QuestionType.RatingQuestion },
                new Question { Id = 6, SurveyId = survey.Id, Text = "Were you provided with sufficient opportunities for growth?", QuestionType = QuestionType.TextQuestion },
                new Question { Id = 7, SurveyId = survey.Id, Text = "How satisfied were you with the training and development programs?", QuestionType = QuestionType.RatingQuestion },
                new Question { Id = 8, SurveyId = survey.Id, Text = "Any suggestions for improving the work environment?", QuestionType = QuestionType.TextQuestion },
                new Question { Id = 9, SurveyId = survey.Id, Text = "How would you rate the overall communication within the company?", QuestionType = QuestionType.RatingQuestion },
                new Question { Id = 10, SurveyId = survey.Id, Text = "What could we have done to retain you as an employee?", QuestionType = QuestionType.TextQuestion }
            };

            context.Questions.AddRange(questions);
            context.SaveChanges();

            var rnd = new Random();
            var textAnswers = new[]
            {
                "I was looking for better career growth.",
                "The compensation didn't meet my expectations.",
                "Work-life balance was not ideal.",
                "Yes, I would recommend it.",
                "Not really, the environment could be more supportive.",
                "Improve internal communication and feedback loops.",
                "Provide clearer career progression paths.",
                "Consider offering more flexible work options.",
                "Invest in team-building activities.",
                "Focus more on employee well-being."
            };

            foreach (var employee in employees)
            {
                var surveyResponse = new SurveyResponse
                {
                    SurveyId = survey.Id,
                    EmployeeId = employee.Id,
                    Response = "Submitted",
                    SubmittedAt = DateTime.UtcNow
                };
                context.SurveyResponses.Add(surveyResponse);
                context.SaveChanges();

                var questionResponses = questions.Select((q, idx) => new QuestionResponse
                {
                    SurveyResponseId = surveyResponse.Id,
                    QuestionId = q.Id,
                    Answer = q.QuestionType == QuestionType.RatingQuestion
                        ? rnd.Next(1, 6).ToString()
                        : textAnswers[(employee.Id + idx) % textAnswers.Length]
                }).ToArray();

                context.QuestionResponses.AddRange(questionResponses);
                context.SaveChanges();
            }
        }
    }
}
