using System.ComponentModel.DataAnnotations;
using adeeb.Models;

namespace adeeb.DTOs;

public class QuestionDto
{
    public int? Id { get; set; }

    [Required]
    public required string Text { get; set; }

    [Required]
    public required string Type { get; set; }

    public List<string>? Options { get; set; }
}
