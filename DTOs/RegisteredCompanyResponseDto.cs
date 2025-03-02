using System;

namespace AdeebBackend.DTOs;

public class RegisteredCompanyResponseDto
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; }
    public string LogoUrl { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
}
