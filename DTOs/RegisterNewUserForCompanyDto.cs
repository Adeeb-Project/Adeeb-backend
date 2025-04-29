using System;
using System.ComponentModel.DataAnnotations;

namespace adeeb.DTOs;

public class RegisterNewUserForCompanyDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    public int CompanyId { get; set; }
}
