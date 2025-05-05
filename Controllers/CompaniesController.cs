using adeeb.Data;
using adeeb.Models;
using AdeebBackend.DTOs;
using AdeebBackend.Extensions;
using AdeebBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdeebBackend.Controllers
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

        // GET: api/companies/rates
        [HttpGet("rates")]
        [Authorize]
        public async Task<ActionResult> GetCompanyRates()
        {
            var companyId = int.Parse(User.FindFirst("companyId")?.Value);
            var result = await _companiesService.GetCompanyRates(companyId);
            return result.ToActionResult();
        }
    }


}