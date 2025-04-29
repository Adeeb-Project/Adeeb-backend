using Microsoft.AspNetCore.Mvc;
using adeeb.DTOs.Common;

namespace adeeb.Extensions
{
    public static class ServiceResultExtensions
    {
        public static ActionResult ToActionResult<T>(this ServiceResult<T> result)
        {
            return result.Type switch
            {
                ResultType.Success => new OkObjectResult(result.Data),
                ResultType.Created => new CreatedResult("", result.Data),
                ResultType.BadRequest => new BadRequestObjectResult(result.ErrorMessage),
                ResultType.NotFound => new NotFoundObjectResult(result.ErrorMessage),
                ResultType.Unauthorized => new UnauthorizedObjectResult(result.ErrorMessage),
                ResultType.Forbidden => new ForbidResult(),
                ResultType.Conflict => new ConflictObjectResult(result.ErrorMessage),
                _ => new StatusCodeResult(500)
            };
        }
    }
} 