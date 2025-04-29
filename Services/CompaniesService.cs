using System;
using adeeb.Data;
using adeeb.Models;
using adeeb.DTOs;
using adeeb.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace adeeb.Services;

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

        bool existsEmail = await _context.CustomUsers.AnyAsync(u => u.Email == companyRequest.Email);
        if (existsEmail)
        {
            return ServiceResult<RegisteredCompanyResponseDto>.Conflict("The provided email is already registered.");
        }

        bool existsUsername = await _context.CustomUsers.AnyAsync(u => u.Name == companyRequest.NameOfUser);
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



        _context.CustomUsers.Add(user);
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

}
