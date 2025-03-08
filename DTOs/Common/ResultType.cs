using System;

namespace AdeebBackend.DTOs.Common;

public enum ResultType
{
    Success = 200,
    Created = 201,
    BadRequest = 400,
    NotFound = 404,
    Unauthorized = 401,
    Forbidden = 403,
    Conflict = 409
}