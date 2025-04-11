using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using adeeb.Models;
using adeeb.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdeebBackend.Services;
using AdeebBackend.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace adeeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public UsersController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterNewUserForCompanyDto user)
        {
            if (string.IsNullOrWhiteSpace(user.Name) ||
                string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest("Name, Email, and Password are required.");
            }

            bool emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (emailExists)
            {
                return Conflict("The provided email is already registered.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            var company = await _context.Companies.FindAsync(user.CompanyId);
            if (company == null)
            {
                return BadRequest("Invalid CompanyId. Company does not exist.");
            }

            var userToAdd = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                CompanyId = user.CompanyId,

            };

            _context.Users.Add(userToAdd);
            await _context.SaveChangesAsync();

            return Ok();
        }
        // GET: api/users/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]

        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password);
            if (!isPasswordValid)
            {
                return Unauthorized("Invalid password.");
            }

            // Assuming you have a way to get the roles of the user
            var roles = new List<string> { "Admin" }; // Replace with actual roles of the user

            var token = _jwtService.GenerateToken(user.Id, user.CompanyId, user.Name, roles);

            return Ok(new
            {
                UserId = user.Id,
                CompanyId = user.CompanyId,
                UserName = user.Name,
                Token = token
            });
        }

    }
}
