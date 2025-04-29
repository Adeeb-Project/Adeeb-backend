using System.ComponentModel.DataAnnotations;
using adeeb.Models;

namespace adeeb.DTOs;

public class RegisteredCompanyResponseDto
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
