using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdeebBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {

        //this is for aws app runner health check
        //GET api/health
        [HttpGet]
        public IActionResult getDefaultHealthCheck()
        {
            return Ok("Health check is working");
        }
    }
}
