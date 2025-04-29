using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using adeeb.Data;
using adeeb.DTOs;
using adeeb.DTOs.Common;
using adeeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace adeeb.Services;

public class SurveysService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly TwilioEmailService _twilioEmailService;

    public SurveysService(AppDbContext context, IConfiguration configuration, TwilioEmailService twilioEmailService)
    {
        _context = context;
        _configuration = configuration;
        _twilioEmailService = twilioEmailService;
    }

    public async Task<ServiceResult<List<QuestionDto>>> GetTestQuestions()
    {
        var questions = new List<QuestionDto>
        {
            new QuestionDto
            {
                Text = "How satisfied are you with your work environment?",
                Type = "Rating"
            },
            new QuestionDto
            {
                Text = "What aspects of the company culture do you appreciate most?",
                Type = "Text"
            },
            new QuestionDto
            {
                Text = "How would you rate your work-life balance?",
                Type = "Rating"
            },
            new QuestionDto
            {
                Text = "Do you feel supported in your professional development?",
                Type = "YesNo"
            }
        };

        return ServiceResult<List<QuestionDto>>.Ok(questions);
    }

    public async Task<ServiceResult<List<QuestionDto>>> GetSurveyQuestions(int surveyId)
    {
        var questions = await _context.Questions
            .Where(q => q.SurveyId == surveyId)
            .Select(q => new QuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                Type = q.Type.ToString(),
                Options = q.Options != null ? new List<string>(q.Options.Split(new[] { ',' }, StringSplitOptions.None)) : null
            })
            .ToListAsync();

        if (!questions.Any())
        {
            return ServiceResult<List<QuestionDto>>.NotFound("No questions found for this survey.");
        }

        return ServiceResult<List<QuestionDto>>.Ok(questions);
    }

    public async Task<ServiceResult<List<SurveyDto>>> GetSurveys(int companyId)
    {
        var surveys = await _context.Surveys
            .Where(s => s.CompanyId == companyId)
            .Include(s => s.Questions)
            .Select(s => new SurveyDto
            {
                SurveyId = s.Id,
                Title = s.Title,
                Description = s.Description,
                ExpiryDate = s.ExpiryDate,
                Questions = s.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Type = q.Type.ToString(),
                    Options = q.Options != null ? new List<string>(q.Options.Split(new[] { ',' }, StringSplitOptions.None)) : null
                }).ToList()
            })
            .ToListAsync();

        return ServiceResult<List<SurveyDto>>.Ok(surveys);
    }

    public async Task<ServiceResult<SurveyDto>> GetSurvey(int surveyId)
    {
        var survey = await _context.Surveys
            .Include(s => s.Questions)
            .FirstOrDefaultAsync(s => s.Id == surveyId);

        if (survey == null)
        {
            return ServiceResult<SurveyDto>.NotFound("Survey not found.");
        }

        var surveyDto = new SurveyDto
        {
            SurveyId = survey.Id,
            Title = survey.Title,
            Description = survey.Description,
            ExpiryDate = survey.ExpiryDate,
            Questions = survey.Questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                Type = q.Type.ToString(),
                Options = q.Options != null ? new List<string>(q.Options.Split(new[] { ',' }, StringSplitOptions.None)) : null
            }).ToList()
        };

        return ServiceResult<SurveyDto>.Ok(surveyDto);
    }

    public async Task<ServiceResult<SurveyDto>> CreateSurvey(int companyId, SurveyDto surveyDto)
    {
        var survey = new Survey
        {
            Title = surveyDto.Title,
            Description = surveyDto.Description,
            ExpiryDate = surveyDto.ExpiryDate,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Surveys.Add(survey);
        await _context.SaveChangesAsync();

        foreach (var questionDto in surveyDto.Questions)
        {
            var question = new Question
            {
                Text = questionDto.Text,
                Type = Enum.Parse<QuestionType>(questionDto.Type),
                Options = questionDto.Options != null ? string.Join(",", questionDto.Options) : null,
                SurveyId = survey.Id
            };

            _context.Questions.Add(question);
        }

        await _context.SaveChangesAsync();

        surveyDto.SurveyId = survey.Id;
        return ServiceResult<SurveyDto>.Ok(surveyDto);
    }

    public async Task<ServiceResult<SurveyDto>> UpdateSurvey(int surveyId, SurveyDto surveyDto)
    {
        var survey = await _context.Surveys
            .Include(s => s.Questions)
            .FirstOrDefaultAsync(s => s.Id == surveyId);

        if (survey == null)
        {
            return ServiceResult<SurveyDto>.NotFound("Survey not found.");
        }

        survey.Title = surveyDto.Title;
        survey.Description = surveyDto.Description;
        survey.ExpiryDate = surveyDto.ExpiryDate;

        // Remove existing questions
        _context.Questions.RemoveRange(survey.Questions);

        // Add new questions
        foreach (var questionDto in surveyDto.Questions)
        {
            var question = new Question
            {
                Text = questionDto.Text,
                Type = Enum.Parse<QuestionType>(questionDto.Type),
                Options = questionDto.Options != null ? string.Join(",", questionDto.Options) : null,
                SurveyId = survey.Id
            };

            _context.Questions.Add(question);
        }

        await _context.SaveChangesAsync();

        return ServiceResult<SurveyDto>.Ok(surveyDto);
    }

    public async Task<ServiceResult<QuestionDto>> AddQuestionToSurvey(int surveyId, QuestionDto questionDto)
    {
        var survey = await _context.Surveys.Include(s => s.Questions).FirstOrDefaultAsync(s => s.Id == surveyId);
        if (survey == null)
        {
            return ServiceResult<QuestionDto>.NotFound("Survey not found.");
        }

        var newQuestion = new Question
        {
            SurveyId = surveyId,
            Text = questionDto.Text,
            Type = Enum.Parse<QuestionType>(questionDto.Type),
            Options = questionDto.Options != null ? string.Join(",", questionDto.Options) : null
        };

        _context.Questions.Add(newQuestion);
        await _context.SaveChangesAsync();

        return ServiceResult<QuestionDto>.Ok(new QuestionDto
        {
            Id = newQuestion.Id,
            Text = newQuestion.Text,
            Type = newQuestion.Type.ToString(),
            Options = newQuestion.Options != null ? new List<string>(newQuestion.Options.Split(new[] { ',' }, StringSplitOptions.None)) : null
        });
    }

    public async Task<ServiceResult<QuestionDto>> EditSurveyQuestion(int questionId, QuestionDto questionDto)
    {
        var question = await _context.Questions.FindAsync(questionId);
        if (question == null)
        {
            return ServiceResult<QuestionDto>.NotFound("Question not found.");
        }

        question.Text = questionDto.Text;
        question.Type = Enum.Parse<QuestionType>(questionDto.Type);
        question.Options = questionDto.Options != null ? string.Join(",", questionDto.Options) : null;

        _context.Questions.Update(question);
        await _context.SaveChangesAsync();

        return ServiceResult<QuestionDto>.Ok(new QuestionDto
        {
            Id = question.Id,
            Text = question.Text,
            Type = question.Type.ToString(),
            Options = question.Options != null ? new List<string>(question.Options.Split(new[] { ',' }, StringSplitOptions.None)) : null
        });
    }

    public async Task<ServiceResult<EmptyResult>> AssignSurveyForEmployee(AssignSurveyForEmployeeRequestDto requestDto, int userId, int companyId)
    {
        //just checking first before anything if the surveyId and employeeId actuaclly exist
        var employeeExists = await _context.Employees.AnyAsync(e => e.Id == requestDto.EmployeeId);
        var surveyExists = await _context.Surveys.AnyAsync(s => s.Id == requestDto.SurveyId);
        var companyExists = await _context.Companies.AnyAsync(c => c.Id == companyId);
        if (!employeeExists || !surveyExists || !companyExists)
        {
            return ServiceResult<EmptyResult>.BadRequest("Invalid EmployeeId or SurveyId or CompanyId.");
        }

        var employee = await _context.Employees.FindAsync(requestDto.EmployeeId);
        if (employee == null)
        {
            return ServiceResult<EmptyResult>.BadRequest("Employee not found.");
        }

        //check if the survey has already been assigned to the employee
        var exists = await _context.EmployeeSurveyLinks
            .AnyAsync(es => es.EmployeeId == requestDto.EmployeeId && es.SurveyId == requestDto.SurveyId);

        if (exists)
        {
            return ServiceResult<EmptyResult>.BadRequest("This survey has already been assigned to this employee.");
        }

        //check if employee is from the same company as the user assigning the survey
        if (employee.CompanyId != companyId)
        {
            return ServiceResult<EmptyResult>.BadRequest("Employee is not from the same company as the user. Cannot assign survey to employees in other companies.");
        }

        //this should later be changed into our website domain name in appsettings.json!!!
        //this is the link the employee will get sent on his email
        var domainName = _configuration["AppSettings:DomainName"] ?? "http://localhost:3000";
        var uniqueLink = $"{domainName}/survey/{requestDto.EmployeeId}";

        var assignment = new EmployeeSurveyLink
        {
            EmployeeId = requestDto.EmployeeId,
            SurveyId = requestDto.SurveyId,
            UniqueLink = uniqueLink,
            SentAt = DateTime.UtcNow,
            IsCompleted = false
        };

        _context.EmployeeSurveyLinks.Add(assignment);
        await _context.SaveChangesAsync();

        var company = await _context.Companies.FindAsync(employee.CompanyId);
        if (company == null)
        {
            return ServiceResult<EmptyResult>.BadRequest("Company not found.");
        }

        // Send email to the employee
        await _twilioEmailService.SendEmailAsync(
            employee.Email,
            "Survey Assignment | استبيان الخروج",
            $@"
Dear {employee.FullName},

We hope this email finds you well. As part of our commitment to continuous improvement, we invite you to participate in an **anonymous exit survey** about your experience at **{company.Name}**.

Your feedback is **completely confidential** and will help us enhance the work environment and improve employee experiences in Saudi Arabia.

We truly value your insights, and your participation will contribute to positive changes in the company.

Please click the link below to complete the survey:
👉 {uniqueLink}

Thank you for your time and valuable input.

Best regards,  
{company.Name}

---
  
عزيزي/عزيزتي {employee.FullName}،

نأمل أن تصلك هذه الرسالة وأنت بخير. كجزء من التزامنا بالتحسين المستمر، ندعوك للمشاركة في **استبيان خروج مجهول الهوية** حول تجربتك في **{company.Name}**.

مشاركتك **سرية تمامًا**، وستساعدنا في تحسين بيئة العمل وتعزيز تجربة الموظفين في المملكة العربية السعودية.

نحن نقدر رأيك، وستساهم ملاحظاتك في إحداث تغييرات إيجابية في الشركة.

يرجى النقر على الرابط أدناه لإكمال الاستبيان:  
👉 {uniqueLink}

شكرًا لوقتك ومساهمتك القيّمة.

تحياتنا،  
{company.Name}
"
        );

        return ServiceResult<EmptyResult>.Created();
    }

    public async Task<ServiceResult<SurveyDto>> GetSurveyDetailsAndQuestions(int employeeId)
    {
        var assignment = await _context.EmployeeSurveyLinks
            .Include(e => e.Survey)
            .ThenInclude(s => s.Questions)
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && !e.IsCompleted);

        if (assignment?.Survey == null)
        {
            return ServiceResult<SurveyDto>.NotFound("No active survey assignment found.");
        }

        var survey = assignment.Survey;
        var surveyDto = new SurveyDto
        {
            SurveyId = survey.Id,
            Title = survey.Title,
            Description = survey.Description,
            Questions = survey.Questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                Type = q.Type.ToString(),
                Options = q.Options != null ? new List<string>(q.Options.Split(new[] { ',' }, StringSplitOptions.None)) : null
            }).ToList()
        };

        return ServiceResult<SurveyDto>.Ok(surveyDto);
    }
}
