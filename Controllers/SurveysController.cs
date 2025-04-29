using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using adeeb.Services;
using adeeb.Models;
using adeeb.Data;
using adeeb.DTOs;
using adeeb.DTOs.Common;
using adeeb.Extensions;

namespace adeeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveysController : ControllerBase
    {
        private readonly SurveysService _surveyService;
        private readonly AppDbContext _context;
        private readonly SurveyAnalysisService _analysisService;

        public SurveysController(SurveysService surveyService, AppDbContext context, SurveyAnalysisService analysisService)
        {
            _surveyService = surveyService;
            _context = context;
            _analysisService = analysisService;
        }

        // GET: api/surveys
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SurveyDto>>> GetSurveys()
        {
            var companyIdClaim = User.FindFirst("companyId");
            if (companyIdClaim == null)
            {
                return Unauthorized("Company ID not found in token.");
            }

            if (int.TryParse(companyIdClaim.Value, out var companyId))
            {
                var result = await _surveyService.GetSurveys(companyId);
                return result.ToActionResult();
            }

            return BadRequest("Invalid company ID format in token.");
        }

        // POST: api/surveys/assign
        [HttpPost("assign")]
        [Authorize]
        public async Task<ActionResult> AssignSurveyForEmployee(AssignSurveyForEmployeeRequestDto requestDto)
        {
            var userIdClaim = User.FindFirst("userId");
            var companyIdClaim = User.FindFirst("companyId");
            if (userIdClaim == null || companyIdClaim == null)
            {
                return Unauthorized("User ID or Company ID not found in token.");
            }

            if (int.TryParse(userIdClaim.Value, out var userId) && int.TryParse(companyIdClaim.Value, out var companyId))
            {
                var result = await _surveyService.AssignSurveyForEmployee(requestDto, userId, companyId);
                return result.ToActionResult();
            }

            return BadRequest("Invalid user ID or company ID format in token.");
        }

        // GET: api/surveys/{employeeId}
        [HttpGet("{employeeId:int}")]
        public async Task<ActionResult<SurveyDto>> GetSurveyDetailsAndQuestions(int employeeId)
        {
            var result = await _surveyService.GetSurveyDetailsAndQuestions(employeeId);
            return result.ToActionResult();
        }

        // POST: api/surveys/{surveyId}/questions
        [HttpPost("{surveyId}/questions")]
        [Authorize]
        public async Task<ActionResult> AddQuestionToSurvey(int surveyId, QuestionDto questionDto)
        {
            var result = await _surveyService.AddQuestionToSurvey(surveyId, questionDto);
            return result.ToActionResult();
        }

        // PUT: api/surveys/questions/{questionId}
        [HttpPut("questions/{questionId}")]
        [Authorize]
        public async Task<ActionResult> EditSurveyQuestion(int questionId, QuestionDto questionDto)
        {
            var result = await _surveyService.EditSurveyQuestion(questionId, questionDto);
            return result.ToActionResult();
        }

        // POST: api/surveys
        [HttpPost("new")]
        [Authorize]
        public async Task<ActionResult<SurveyDto>> CreateSurvey(SurveyDto surveyDto)
        {
            var companyIdClaim = User.FindFirst("companyId");
            if (companyIdClaim == null)
            {
                return Unauthorized("Company ID not found in token.");
            }

            if (int.TryParse(companyIdClaim.Value, out var companyId))
            {
                var result = await _surveyService.CreateSurvey(companyId, surveyDto);
                return result.ToActionResult();
            }

            return BadRequest("Invalid company ID format in token.");
        }

        // GET: api/surveys/{surveyId}/responses
        [HttpGet("{surveyId}/responses")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SurveyResponse>>> GetSurveyResponses(int surveyId)
        {
            var companyIdClaim = User.FindFirst("companyId");
            if (companyIdClaim == null)
            {
                return Unauthorized("Company ID not found in token.");
            }

            if (!int.TryParse(companyIdClaim.Value, out var companyId))
            {
                return BadRequest("Invalid company ID format in token.");
            }

            // Verify that the survey belongs to the company
            var survey = await _context.Surveys.FirstOrDefaultAsync(s => s.Id == surveyId && s.CompanyId == companyId);
            if (survey == null)
            {
                return NotFound("Survey not found or you don't have access to it.");
            }

            var responses = await _context.SurveyResponses
                .Include(r => r.Survey)
                .Where(r => r.SurveyId == surveyId)
                .ToListAsync();

            return Ok(responses);
        }

        // GET: api/surveys/{surveyId}/analyze
        [HttpGet("{surveyId}/analyze")]
        [Authorize]
        public async Task<ActionResult<SurveyAnalysisResult>> AnalyzeSurveyResponses(int surveyId)
        {
            var companyIdClaim = User.FindFirst("companyId");
            if (companyIdClaim == null)
            {
                return Unauthorized("Company ID not found in token.");
            }

            if (!int.TryParse(companyIdClaim.Value, out var companyId))
            {
                return BadRequest("Invalid company ID format in token.");
            }

            // Verify that the survey belongs to the company
            var survey = await _context.Surveys.FirstOrDefaultAsync(s => s.Id == surveyId && s.CompanyId == companyId);
            if (survey == null)
            {
                return NotFound("Survey not found or you don't have access to it.");
            }

            var result = await _analysisService.AnalyzeSurveyResponses(surveyId);
            return Ok(result);
        }

        // GET: api/surveys/test-analysis
        [HttpGet("test-analysis")]
        [Authorize]
        public async Task<ActionResult<SurveyAnalysisResult>> TestAnalysis()
        {
            var companyIdClaim = User.FindFirst("companyId");
            if (companyIdClaim == null)
            {
                return Unauthorized("Company ID not found in token.");
            }

            if (!int.TryParse(companyIdClaim.Value, out var companyId))
            {
                return BadRequest("Invalid company ID format in token.");
            }

            // Create a test survey
            var testSurvey = new Survey
            {
                Title = "Employee Satisfaction Survey",
                Description = "Test survey for NLP analysis demonstration",
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Surveys.Add(testSurvey);
            await _context.SaveChangesAsync();

            // Add test responses
            var testResponses = new List<SurveyResponse>
            {
                new SurveyResponse
                {
                    SurveyId = testSurvey.Id,
                    EmployeeId = 3,
                    Response = "I really enjoy working here. The team is supportive and the work environment is great. However, I think we could improve our communication between departments.",
                    SubmittedAt = DateTime.UtcNow
                },
                new SurveyResponse
                {
                    SurveyId = testSurvey.Id,
                    EmployeeId = 3,
                    Response = "The workload is sometimes overwhelming, but the management is understanding. I appreciate the flexible working hours and the opportunities for professional development.",
                    SubmittedAt = DateTime.UtcNow
                },
                new SurveyResponse
                {
                    SurveyId = testSurvey.Id,
                    EmployeeId = 3,
                    Response = "I'm concerned about the lack of clear career progression paths. The benefits are good, but I feel stuck in my current role. More training opportunities would be helpful.",
                    SubmittedAt = DateTime.UtcNow
                },
                new SurveyResponse
                {
                    SurveyId = testSurvey.Id,
                    EmployeeId = 3,
                    Response = "The company culture is excellent, and I feel valued. The recent team-building activities were very effective. I would like to see more cross-department collaboration.",
                    SubmittedAt = DateTime.UtcNow
                }
            };

            _context.SurveyResponses.AddRange(testResponses);
            await _context.SaveChangesAsync();

            // Analyze the responses
            var result = await _analysisService.AnalyzeSurveyResponses(testSurvey.Id);

            return Ok(new
            {
                SurveyId = testSurvey.Id,
                Analysis = result
            });
        }

        // POST: api/surveys/{surveyId}/submit
        [HttpPost("{surveyId}/submit")]
        public async Task<ActionResult> SubmitSurveyResponse(int surveyId, SurveyResponseDto responseDto)
        {
            var survey = await _context.Surveys.FindAsync(surveyId);
            if (survey == null)
            {
                return NotFound("Survey not found.");
            }

            var employee = await _context.Employees.FindAsync(responseDto.EmployeeId);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            var surveyResponse = new SurveyResponse
            {
                SurveyId = surveyId,
                EmployeeId = responseDto.EmployeeId,
                Response = responseDto.Response,
                SubmittedAt = DateTime.UtcNow
            };

            _context.SurveyResponses.Add(surveyResponse);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Survey response submitted successfully." });
        }
    }
}
