using adeeb.Data;
using adeeb.Models;
using AdeebBackend.DTOs;
using AdeebBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdeebBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly S3Service _s3Service;
        private readonly JwtService _jwtService;

        public CompaniesController(AppDbContext context, S3Service s3Service, JwtService jwtService)
        {
            _context = context;
            _s3Service = s3Service;
            _jwtService = jwtService;
        }

        // POST: api/companies
        [HttpPost("register")]
        public async Task<ActionResult<Company>> RegisterCompany(CompanyRegistrationRequestDto companyRequest)
        {

            var name = companyRequest.Name;
            var totalNumberOfEmployees = companyRequest.TotalNumberOfEmployees;
            var bundle = companyRequest.Bundle;
            string url = await _s3Service.UploadCompanyLogoAsync(companyRequest.LogoImage);

            var company = new Company
            {
                Name = name,
                TotalNumberOfEmployees = totalNumberOfEmployees,
                BundleType = bundle,
                LogoUrl = url
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            //here we should create a user for the company
            var user = new User
            {
                Name = companyRequest.NameOfUser,
                Email = companyRequest.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(companyRequest.Password),
                CompanyId = company.Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user.Id, company.Id);


            return Ok(new RegisteredCompanyResponseDto
            {
                CompanyId = company.Id,
                Token = token,
                CompanyName = company.Name,
                LogoUrl = company.LogoUrl,
                UserName = user.Name,
                Email = user.Email

            });

        }
    }
}