using System;

namespace AdeebBackend.DTOs;

public class AssignSurveyForEmployeeRequestDto
{

    public int SurveyId { get; set; }
    public List<int> EmployeeId { get; set; }

}
