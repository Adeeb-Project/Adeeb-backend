using adeeb.Data;
using adeeb.Models;
using adeeb.DTOs;
using adeeb.Extensions;
using adeeb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace adeeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly CompaniesService _companiesService;
        public CompaniesController(CompaniesService companiesService)
        {
            _companiesService = companiesService;
        }


        // POST: api/companies
        [HttpPost("register")]
        public async Task<ActionResult> RegisterCompany(CompanyRegistrationRequestDto companyRequest)
        {
            var result = await _companiesService.RegisterCompany(companyRequest);
            return result.ToActionResult();
        }
    }
}