using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
Makki/provide-rates

using adeeb.Models;
using AdeebBackend.Data;
using AdeebBackend.Models;
using adeeb.Data;
using Microsoft.EntityFrameworkCore;

namespace AdeebBackend.Data
{
    public class DummyDbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.Migrate();
            if (context.Companies.Any()) return;

            // 1) Company & Survey & Questions
            var company = new Company
            {
                Name = "Test Company",
                TotalNumberOfEmployees = 1000,
                BundleType = BundleType.Premium
            };
            context.Companies.Add(company);
            context.SaveChanges();

            var survey = new Survey
            {
                CompanyId = company.Id,
                Title = "Employee Exit Survey",
                Description = "Your candid feedback helps us get better.",
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(1)
            };
            context.Surveys.Add(survey);
            context.SaveChanges();

            var questions = new[] {
        new Question { SurveyId = survey.Id, Text = "1. Overall Job Satisfaction (1-5)",               QuestionType = QuestionType.RatingQuestion },
        new Question { SurveyId = survey.Id, Text = "2. Role Clarity (1-5)",                           QuestionType = QuestionType.RatingQuestion },
        new Question { SurveyId = survey.Id, Text = "3. Job Expectations?",                            QuestionType = QuestionType.MultipleChoiceQuestion },
        new Question { SurveyId = survey.Id, Text = "4. What did you enjoy most about this role?",     QuestionType = QuestionType.TextQuestion },
        new Question { SurveyId = survey.Id, Text = "5. Manager Support?",                              QuestionType = QuestionType.MultipleChoiceQuestion },
        new Question { SurveyId = survey.Id, Text = "6. Relationship with Manager (1-5)",              QuestionType = QuestionType.RatingQuestion },
        new Question { SurveyId = survey.Id, Text = "7. Workload Manageability?",                      QuestionType = QuestionType.MultipleChoiceQuestion },
        new Question { SurveyId = survey.Id, Text = "8. Mental Well-Being (1-5)",                       QuestionType = QuestionType.RatingQuestion },
        new Question { SurveyId = survey.Id, Text = "9. Growth Opportunities Satisfaction (1-5)",    QuestionType = QuestionType.RatingQuestion },
        new Question { SurveyId = survey.Id, Text = "10. Suggestions for improvement",                 QuestionType = QuestionType.TextQuestion }
      };
            context.Questions.AddRange(questions);
            context.SaveChanges();


            // 2) Read all lines from Data/employees.csv in your project root
            var projectRoot = Directory.GetCurrentDirectory();
            var csvPath = Path.Combine(projectRoot, "Data", "employees.csv");
            if (!File.Exists(csvPath))
                throw new FileNotFoundException($"CSV not found at {csvPath}");

            var lines = File.ReadAllLines(csvPath)
                            .Skip(1)
                            .Where(l => !string.IsNullOrWhiteSpace(l))
                            .ToArray();


            var rnd = new Random();
            var employees = new List<Employee>();

            foreach (var line in lines)
            {
                var cols = line.Split(',');
                var fullName = cols[0].Trim();
                var email = cols[1].Trim();
                // if phone missing or masked, generate a random one
                var phone = rnd.Next(500_000_000, 999_999_999).ToString();
                phone = "+966" + phone;

                // parse left date, else random 1–8 years ago
                DateTime leftDate;
                if (!DateTime.TryParse(cols[3], out leftDate))
                    leftDate = DateTime.UtcNow.AddDays(-rnd.Next(30, 365 * 8));

                var dept = cols.Length > 4 ? cols[4].Trim() : "General";
                var pos = cols.Length > 5 ? cols[5].Trim() : "Staff";

                employees.Add(new Employee
                {
                    CompanyId = company.Id,
                    FullName = fullName,
                    Email = email,
                    PhoneNumber = phone,
                    LeftDate = leftDate,
                    Department = dept,
                    Position = pos
                });
            }

            context.Employees.AddRange(employees);
            context.SaveChanges();

            // 3) Prepare richer text-answers
            var enjoyedMost = new[] {
        "I really appreciated the collaborative atmosphere; teammates were always willing to help.",
        "Flexible work hours allowed me to balance personal and professional life seamlessly.",
        "The comprehensive training program helped me get up to speed very quickly.",
        "Opportunities for cross-functional projects kept my work varied and interesting.",
        "I loved the monthly knowledge-sharing sessions with senior leadership."
      };
            var suggestions = new[] {
        "I would recommend more one-on-one coaching from managers to develop career goals.",
        "Better documentation on internal processes would reduce onboarding friction.",
        "It would be helpful to have more frequent feedback loops—quarterly reviews felt too sparse.",
        "More social events or team offsites could strengthen cross-team relationships.",
        "Consider offering a mentorship program pairing juniors with seniors."
      };
            var mcqAnswers = new Dictionary<string, string[]>
            {
                ["3. Job Expectations?"] = new[] { "Exceeded my expectations", "Met my expectations", "Partially met my expectations", "Did not meet my expectations" },
                ["5. Manager Support?"] = new[] { "Yes, definitely", "Somewhat/occasionally", "No, not really" },
                ["7. Workload Manageability?"] = new[] { "Yes, most of the time", "Somewhat manageable at times", "No, it was often unmanageable" }
            };

            // 4) Seed ~20 random survey responses
            var sample = employees.OrderBy(_ => rnd.Next()).Take(20).ToList();
            foreach (var emp in sample)
            {
                var sr = new SurveyResponse
                {
                    SurveyId = survey.Id,
                    EmployeeId = emp.Id,
                    Response = "Submitted",
                    SubmittedAt = DateTime.UtcNow.AddMinutes(-rnd.Next(1, 10_000))
                };
                context.SurveyResponses.Add(sr);
                context.SaveChanges();


                foreach (var q in questions)
                {
                    string ans;
                    switch (q.QuestionType)
                    {
                        case QuestionType.RatingQuestion:
                            ans = rnd.Next(1, 6).ToString();
                            break;
                        case QuestionType.MultipleChoiceQuestion:
                            ans = mcqAnswers[q.Text][rnd.Next(mcqAnswers[q.Text].Length)];
                            break;
                        default: // text
                            ans = q.Text.StartsWith("4.")
                                ? enjoyedMost[rnd.Next(enjoyedMost.Length)]
                                : suggestions[rnd.Next(suggestions.Length)];
                            break;
                    }
                    context.QuestionResponses.Add(new QuestionResponse
                    {
                        SurveyResponseId = sr.Id,
                        QuestionId = q.Id,
                        Answer = ans
                    });
                }

                context.SaveChanges();
            }
        }
    }
}