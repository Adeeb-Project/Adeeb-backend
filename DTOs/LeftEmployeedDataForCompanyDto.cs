using System;
using System.Text.Json.Serialization;

namespace AdeebBackend.DTOs;

public class LeftEmployeedDataForCompanyDto
{

    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime JoinDate { get; set; }
    public string Department { get; set; }
    public string Position { get; set; }
    public string PhoneNumber { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EmployeeSurvyeStatus SurveyStatus { get; set; }

}

public enum EmployeeSurvyeStatus
{
    SurveyNotAssigned,
    SurveySent,
    SurveyCompleted,
}
