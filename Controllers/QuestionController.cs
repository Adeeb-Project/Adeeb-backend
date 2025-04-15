using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AdeebBackend.DTOs;
using AdeebBackend.Services;

namespace AdeebBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IChatGPTService _chatGPTService;

        public QuestionController(IChatGPTService chatGPTService)
        {
            _chatGPTService = chatGPTService;
        }

        // POST: api/question/improve
        [HttpPost("improve")]
        public async Task<IActionResult> ImproveQuestion([FromBody] QuestionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Question))
                return BadRequest("Question cannot be empty.");

            string improved = await _chatGPTService.ImproveQuestionAsync(request.Question);
            return Ok(new { improvedQuestion = improved });
        }
    }
}
