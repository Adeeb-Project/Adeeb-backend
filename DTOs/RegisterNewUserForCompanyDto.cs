using System;

namespace AdeebBackend.DTOs;

public class RegisterNewUserForCompanyDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int CompanyId { get; set; }

}
