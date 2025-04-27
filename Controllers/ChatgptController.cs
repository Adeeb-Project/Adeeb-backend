using AdeebBackend.DTOs;
using AdeebBackend.Services;
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

        [HttpPost("RefineSurveyQuestion")]
        public async Task<IActionResult> RefineSurveyQuestion(ChatgptQuestionRefineRequestDto requestDto)
        {
            if (string.IsNullOrWhiteSpace(requestDto.Question))
            {
                return BadRequest("Question cannot be empty.");
            }
            var result = await _chatGptService.RefineSurveyQuestion("What is the capital of France?");
            return Ok(result);
        }
    }
}
