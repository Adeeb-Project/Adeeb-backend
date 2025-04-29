using System.ComponentModel.DataAnnotations;

namespace adeeb.DTOs;

public class AssignSurveyForEmployeeRequestDto
{
    [Required]
    public required int EmployeeId { get; set; }

    [Required]
    public required int SurveyId { get; set; }
}
