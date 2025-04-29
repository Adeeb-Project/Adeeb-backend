using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using adeeb.Models;

namespace adeeb.DTOs
{
    public class CompanyRegistrationRequestDto
    {
        [Required]
        public required string Name { get; set; }

        public string? LogoUrl { get; set; }
        public IFormFile? LogoImage { get; set; }
        public int TotalNumberOfEmployees { get; set; }
        public BundleType Bundle { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(8)]
        public required string Password { get; set; }

        [Required]
        public required string NameOfUser { get; set; }
    }
} 