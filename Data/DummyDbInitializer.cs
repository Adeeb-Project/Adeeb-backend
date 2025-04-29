using System;
using System.Collections.Generic;
using System.Linq;
using adeeb.Models;
using Microsoft.EntityFrameworkCore;

namespace adeeb.Data
{
    public static class DummyDbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Check if the database has been seeded
            if (context.Companies.Any())
            {
                return; // Database has been seeded
            }

            // Create a company
            var company = new Company
            {
                Name = "EboM",
                TotalNumberOfEmployees = 100,
                BundleType = BundleType.Basic,
                LogoUrl = "https://example.com/logo.png"
            };

            context.Companies.Add(company);
            context.SaveChanges();

            // Create a user for the company
            var user = new User
            {
                Name = "HR Manager",
                Email = "hr@ebom.com",
                Password = BCrypt.Net.BCrypt.HashPassword("12345678"),
                CompanyId = company.Id
            };

            context.CustomUsers.Add(user);
            context.SaveChanges();

            // Create employees
            var employee1 = new Employee
            {
                FullName = "John Doe",
                Email = "john@ebom.com",
                Department = "Engineering",
                Position = "Software Engineer",
                PhoneNumber = "1234567890",
                CompanyId = company.Id,
                JoinDate = DateTime.UtcNow
            };

            var employee2 = new Employee
            {
                FullName = "Jane Smith",
                Email = "jane@ebom.com",
                Department = "Marketing",
                Position = "Marketing Manager",
                PhoneNumber = "0987654321",
                CompanyId = company.Id,
                JoinDate = DateTime.UtcNow
            };

            context.Employees.AddRange(employee1, employee2);
            context.SaveChanges();

            // Create a survey
            var survey = new Survey
            {
                Title = "Employee Satisfaction Survey",
                Description = "Annual survey to measure employee satisfaction",
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                CompanyId = company.Id
            };

            context.Surveys.Add(survey);
            context.SaveChanges();

            // Create questions
            var question1 = new Question
            {
                Text = "How satisfied are you with your work environment?",
                Type = QuestionType.Rating,
                SurveyId = survey.Id
            };

            var question2 = new Question
            {
                Text = "What aspects of the company culture do you appreciate most?",
                Type = QuestionType.Text,
                SurveyId = survey.Id
            };

            context.Questions.AddRange(question1, question2);
            context.SaveChanges();

            // Create survey assignments
            var assignment1 = new EmployeeSurveyLink
            {
                UniqueLink = Guid.NewGuid().ToString(),
                IsCompleted = false,
                EmployeeId = employee1.Id,
                SurveyId = survey.Id
            };

            var assignment2 = new EmployeeSurveyLink
            {
                UniqueLink = Guid.NewGuid().ToString(),
                IsCompleted = false,
                EmployeeId = employee2.Id,
                SurveyId = survey.Id
            };

            context.EmployeeSurveyLinks.AddRange(assignment1, assignment2);
            context.SaveChanges();
        }
    }
}
