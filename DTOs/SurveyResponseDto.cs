using System.ComponentModel.DataAnnotations;

namespace adeeb.DTOs;

public class SurveyResponseDto
{
    [Required]
    public required int QuestionId { get; set; }

    [Required]
    public required string Response { get; set; }

    [Required]
    public required int EmployeeId { get; set; }
} 