using System;
using adeeb.Data;
using adeeb.Models;
using AdeebBackend.DTOs;
using AdeebBackend.DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdeebBackend.Services;

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

    public async Task<ServiceResult<IEnumerable<SurveyDto>>> GetSurveys(int companyId)
    {
        var surveys = await _context.Surveys.Include(e => e.Questions).Where(t => t.CompanyId == companyId).Select(s => new SurveyDto
        {
            SurveyId = s.Id,
            Title = s.Title,
            Description = s.Description,
            Questions = s.Questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                QuestionType = q.QuestionType
            }).ToList()
        }).ToListAsync();
        return ServiceResult<IEnumerable<SurveyDto>>.Ok(surveys);
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
        var domainName = _configuration["AppSettings:DomainName"];
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

        // Send email to the employee
        _twilioEmailService.SendEmailAsync(
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
        // Retrieve the active assignment for the employee along with its Survey and nested Questions
        var assignment = await _context.EmployeeSurveyLinks
            .Include(e => e.Survey)
                .ThenInclude(s => s.Questions)
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && !e.IsCompleted);

        if (assignment == null)
        {
            return ServiceResult<SurveyDto>.NotFound("No active survey assignment found for the employee");
        }

        var survey = assignment.Survey;

        // Map the Survey entity to a DTO
        var surveyDto = new SurveyDto
        {
            SurveyId = survey.Id,
            Title = survey.Title,
            Description = survey.Description,
            Questions = survey.Questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                QuestionType = q.QuestionType
            }).ToList()
        };

        return ServiceResult<SurveyDto>.Ok(surveyDto);

    }

}
