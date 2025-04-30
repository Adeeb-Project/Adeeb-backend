using AdeebBackend.DTOs;
using AdeebBackend.Extensions;
using AdeebBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdeebBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatgptController : ControllerBase
    {
        private readonly ChatGptService _chatGptService;

        public ChatgptController(ChatGptService chatGptService)
        {
            _chatGptService = chatGptService;
        }

        // POST: api/chatgpt/RefineSurveyQuestion
        [HttpPost("RefineSurveyQuestion")]
        [Authorize]
        public async Task<ActionResult<string>> RefineSurveyQuestion(ChatgptQuestionRefineRequestDto requestDto)
        {

            var result = await _chatGptService.RefineSurveyQuestion(requestDto.Question);
            return result.ToActionResult();
        }

        // GET: api/chatgpt/SurveyResponsesAnalysis
        [HttpGet("SurveyResponsesAnalysis")]
        [Authorize]
        public async Task<ActionResult<bool>> SurveyResponsesAnalysis([FromQuery] int surveyId)
        {
            var companyId = int.Parse(User.FindFirst("companyId")?.Value);
            var result = await _chatGptService.SurveyResponsesAnalysis(surveyId, companyId);
            return result.ToActionResult();
        }
    }
}
