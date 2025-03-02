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
    [Range(30, int.MaxValue)]
    public int TotalNumberOfEmployees { get; set; }
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BundleType Bundle { get; set; }

    // This property will hold the uploaded image file
    [Required]
    public IFormFile LogoImage { get; set; }

    //these are for user registration
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [StringLength(maximumLength: 30, MinimumLength = 8)]
    public string Password { get; set; }
    [Required]
    [MinLength(4)]
    public string NameOfUser { get; set; }
}