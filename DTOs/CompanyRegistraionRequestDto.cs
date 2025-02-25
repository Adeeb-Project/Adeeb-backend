namespace AdeebBackend.DTOs;

using adeeb.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class CompanyRegistrationRequestDto
{
    [Required]
    [StringLength(maximumLength: 30, MinimumLength = 1)]
    public string Name { get; set; }
    [Required]
    [MinLength(30)]
    public int TotalNumberOfEmployees { get; set; }
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BundleType Bundle { get; set; }

    // This property will hold the uploaded image file
    [Required]
    public IFormFile LogoImage { get; set; }
}