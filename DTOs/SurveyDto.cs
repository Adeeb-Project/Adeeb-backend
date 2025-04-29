using System;
using System.ComponentModel.DataAnnotations;

namespace adeeb.DTOs;

public class SurveyDto
{
    public int? SurveyId { get; set; }
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Description { get; set; }
    public DateTime? ExpiryDate { get; set; }
    [Required]
    public required List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
}
