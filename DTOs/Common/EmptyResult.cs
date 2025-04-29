using System;

namespace adeeb.DTOs.Common;

public sealed class EmptyResult
{
    private EmptyResult() { }
    public static EmptyResult Instance { get; } = new EmptyResult();
} 