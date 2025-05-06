using System;
using adeeb.Data;
using adeeb.Models;
using AdeebBackend.DTOs;
using AdeebBackend.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace AdeebBackend.Services;

public class CompaniesService
{
    private readonly AppDbContext _context;
    private readonly S3Service _s3Service;
    private readonly JwtService _jwtService;

    public CompaniesService(AppDbContext context, S3Service s3Service, JwtService jwtService)
    {
        _context = context;
        _s3Service = s3Service;
        _jwtService = jwtService;
    }

    public async Task<ServiceResult<RegisteredCompanyResponseDto>> RegisterCompany(CompanyRegistrationRequestDto companyRequest)
    {

        var name = companyRequest.Name;
        var totalNumberOfEmployees = companyRequest.TotalNumberOfEmployees;
        var bundle = companyRequest.Bundle;
        string url = companyRequest.LogoImage == null ? "" : await _s3Service.UploadCompanyLogoAsync(companyRequest.LogoImage);

        var company = new Company
        {
            Name = name,
            TotalNumberOfEmployees = totalNumberOfEmployees,
            BundleType = bundle,
            LogoUrl = url
        };

        bool existsCompany = await _context.Companies.AnyAsync(c => c.Name == company.Name);
        if (existsCompany)
        {
            return ServiceResult<RegisteredCompanyResponseDto>.Conflict("The provided company name is already registered.");
        }

        bool existsEmail = await _context.Users.AnyAsync(u => u.Email == companyRequest.Email);
        if (existsEmail)
        {
            return ServiceResult<RegisteredCompanyResponseDto>.Conflict("The provided email is already registered.");
        }

        bool existsUsername = await _context.Users.AnyAsync(u => u.Name == companyRequest.NameOfUser);
        if (existsUsername)
        {
            return ServiceResult<RegisteredCompanyResponseDto>.Conflict("The provided username is already registered.");
        }

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

        var roles = new List<string> { "HRManager" };


        var token = _jwtService.GenerateToken(user.Id, user.CompanyId, user.Name, roles);


        return ServiceResult<RegisteredCompanyResponseDto>.Ok(new RegisteredCompanyResponseDto
        {
            CompanyId = company.Id,
            Token = token,
            CompanyName = company.Name,
            LogoUrl = company.LogoUrl,
            UserName = user.Name,
            Email = user.Email

        });
    }

    public async Task<ServiceResult<CompanyChartResponseDto>> GetCompanyChartAsync(
    int companyId,
    string period,
    string graphType)
    {
        // 1. Historic total
        var company = await _context.Companies.FindAsync(companyId)
                      ?? throw new InvalidOperationException("Company not found");
        int totalHistoric = company.TotalNumberOfEmployees;

        // 2. Build full labels & both series
        var (labels, fullTurnover, fullRetention)
            = await BuildTimeSeriesAsync(companyId, totalHistoric, period);

        // 3. Compute current snapshot (all-time) for donut
        int leftAllTime = await _context.Employees
            .Where(e => e.CompanyId == companyId && e.LeftDate != default)
            .CountAsync();
        double currentTurnover = totalHistoric > 0
            ? Math.Round((double)leftAllTime / totalHistoric * 100, 2)
            : 0;
        double currentRetention = Math.Round(100 - currentTurnover, 2);

        // 4. Prepare DTO
        var dto = new CompanyChartResponseDto
        {
            Labels = (graphType == "donut")
                ? new List<string> { "Current" }
                : labels,

            Data = graphType switch
            {
                "combined" or "bar" => new CompanySeries
                {
                    Turnover = fullTurnover,
                    Retention = fullRetention
                },
                "turnover" => new CompanySeries { Turnover = fullTurnover },
                "retention" => new CompanySeries { Retention = fullRetention },
                _ => null
            },

            CurrentStatus = graphType == "donut"
                ? new CompanySeries
                {
                    Turnover = new List<double> { currentTurnover },
                    Retention = new List<double> { currentRetention }
                }
                : null,

            LastUpdated = DateTime.UtcNow
        };

        return ServiceResult<CompanyChartResponseDto>.Ok(dto);
    }

    // Helper to build your quarterly/yearly/allTime arrays
    private async Task<(List<string>, List<double>, List<double>)> BuildTimeSeriesAsync(
        int companyId, int totalHistoric, string period)
    {
        var labels = new List<string>();
        var turnover = new List<double>();
        var retention = new List<double>();
        DateTime now = DateTime.UtcNow;

        if (period == "quarterly")
        {
            int year = now.Year;
            for (int q = 1; q <= 4; q++)
            {
                var start = new DateTime(year, (q - 1) * 3 + 1, 1);
                var end = start.AddMonths(3);
                labels.Add($"Q{q} {year}");

                int leftCount = await _context.Employees
                    .Where(e => e.CompanyId == companyId
                             && e.LeftDate >= start
                             && e.LeftDate < end)
                    .CountAsync();

                double t = totalHistoric > 0
                    ? Math.Round((double)leftCount / totalHistoric * 100, 2)
                    : 0;
                turnover.Add(t);
                retention.Add(Math.Round(100 - t, 2));
            }
        }
        else if (period == "yearly")
        {
            int currentYear = now.Year;
            for (int y = currentYear - 4; y <= currentYear; y++)
            {
                var start = new DateTime(y, 1, 1);
                var end = start.AddYears(1);
                labels.Add(y.ToString());

                int leftCount = await _context.Employees
                    .Where(e => e.CompanyId == companyId
                             && e.LeftDate >= start
                             && e.LeftDate < end)
                    .CountAsync();

                double t = totalHistoric > 0
                    ? Math.Round((double)leftCount / totalHistoric * 100, 2)
                    : 0;
                turnover.Add(t);
                retention.Add(Math.Round(100 - t, 2));
            }
        }
        else // allTime
        {
            labels.Add("All Time");
            int leftCount = await _context.Employees
                .Where(e => e.CompanyId == companyId
                         && e.LeftDate != default)
                .CountAsync();
            double t = totalHistoric > 0
                ? Math.Round((double)leftCount / totalHistoric * 100, 2)
                : 0;
            turnover.Add(t);
            retention.Add(Math.Round(100 - t, 2));
        }

        return (labels, turnover, retention);
    }


}
