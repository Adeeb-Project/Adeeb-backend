using System;

namespace adeeb.DTOs.Common;

public class ServiceResult<T>
{
    public bool Success => Type == ResultType.Success;

    public ResultType Type { get; set; }
    public string? ErrorMessage { get; set; }
    public T? Data { get; set; }

    // Static Helpers (Factory Methods) to create results easily
    public static ServiceResult<T> Ok(T data) => new() { Type = ResultType.Success, Data = data };
    public static ServiceResult<T> Created() => new() { Type = ResultType.Created };
    public static ServiceResult<T> NotFound(string message) => new() { Type = ResultType.NotFound, ErrorMessage = message };
    public static ServiceResult<T> BadRequest(string message) => new() { Type = ResultType.BadRequest, ErrorMessage = message };
    public static ServiceResult<T> Unauthorized(string message) => new() { Type = ResultType.Unauthorized, ErrorMessage = message };
    public static ServiceResult<T> Forbidden(string message) => new() { Type = ResultType.Forbidden, ErrorMessage = message };
    public static ServiceResult<T> Conflict(string message) => new() { Type = ResultType.Conflict, ErrorMessage = message };

    // Optional: Generic failure helper
    public static ServiceResult<T> Failure(ResultType type, string message) => new() { Type = type, ErrorMessage = message };
}