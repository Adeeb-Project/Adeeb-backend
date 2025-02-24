using adeeb.Data;
using adeeb.Models;
using AdeebBackend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdeebBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveysController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public SurveysController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/surveys
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Survey>>> GetSurveys()
        {
            var surveys = await _context.Surveys.Include(e => e.Questions).Select(s => new SurveyDto
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
            return Ok(surveys);
        }

        // POST: api/surveys/assign
        [HttpPost("assign")]
        public async Task<ActionResult> AssignSurveyForEmployee(AssignSurveyForEmployeeRequestDto requestDto)
        {
            //just checking first before anything if the surveyId and employeeId actuaclly exist
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == requestDto.EmployeeId);
            var surveyExists = await _context.Surveys.AnyAsync(s => s.Id == requestDto.SurveyId);
            if (!employeeExists || !surveyExists)
            {
                return BadRequest("Invalid EmployeeId or SurveyId");
            }

            //check if the survey has already been assigned to the employee
            var exists = await _context.EmployeeSurveyLinks
    .AnyAsync(es => es.EmployeeId == requestDto.EmployeeId && es.SurveyId == requestDto.SurveyId);

            if (exists)
            {
                return BadRequest("This survey has already been assigned to this employee.");
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

            return Created();
        }

        // GET: api/surveys/{employeeId}
        [HttpGet("{employeeId}")]
        public async Task<ActionResult<SurveyDto>> GetSurveyDetailsAndQuestions(int employeeId)
        {
            // Retrieve the active assignment for the employee along with its Survey and nested Questions
            var assignment = await _context.EmployeeSurveyLinks
                .Include(e => e.Survey)
                    .ThenInclude(s => s.Questions)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && !e.IsCompleted);

            if (assignment == null)
            {
                return NotFound("No active survey assignment found for the employee");
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

            return Ok(surveyDto);

        }
    }
}
