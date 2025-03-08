using adeeb.Data;
using adeeb.Models;
using AdeebBackend.DTOs;
using AdeebBackend.Extensions;
using AdeebBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdeebBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveysController : ControllerBase
    {

        private readonly SurveysService _surveyService;

        public SurveysController(SurveysService surveyService)
        {

            _surveyService = surveyService;
        }

        // GET: api/surveys
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Survey>>> GetSurveys()
        {

            var companyId = int.Parse(User.FindFirst("companyId")?.Value);
            var result = await _surveyService.GetSurveys(companyId);
            return result.ToActionResult();
        }

        // POST: api/surveys/assign
        [HttpPost("assign")]
        [Authorize]
        public async Task<ActionResult> AssignSurveyForEmployee(AssignSurveyForEmployeeRequestDto requestDto)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value);
            var companyId = int.Parse(User.FindFirst("companyId")?.Value);
            var result = await _surveyService.AssignSurveyForEmployee(requestDto, userId, companyId);
            return result.ToActionResult();
        }

        // GET: api/surveys/{employeeId}
        [HttpGet("{employeeId}")]
        public async Task<ActionResult<SurveyDto>> GetSurveyDetailsAndQuestions(int employeeId)
        {
            var result = await _surveyService.GetSurveyDetailsAndQuestions(employeeId);
            return result.ToActionResult();
        }
    }
}
