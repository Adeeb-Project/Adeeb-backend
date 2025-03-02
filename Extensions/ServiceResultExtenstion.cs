using System;
using AdeebBackend.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace AdeebBackend.Extensions;

public static class ServiceResultExtensions
{
    public static ActionResult ToActionResult<T>(this ServiceResult<T> result)
    {
        return result.Type switch
        {
            ResultType.Success => new OkObjectResult(result.Data),
            ResultType.BadRequest => new BadRequestObjectResult(result.ErrorMessage),
            ResultType.NotFound => new NotFoundObjectResult(result.ErrorMessage),
            ResultType.Unauthorized => new UnauthorizedObjectResult(result.ErrorMessage),
            ResultType.Forbidden => new ObjectResult(result.ErrorMessage) { StatusCode = StatusCodes.Status403Forbidden },
            ResultType.Conflict => new ConflictObjectResult(result.ErrorMessage),
            _ => new ObjectResult("An unexpected error occurred.") { StatusCode = 500 }
        };
    }
}