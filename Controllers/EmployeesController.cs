using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using adeeb.Models;
using adeeb.Data; //ya shbab its AppDbContext not ApplicationDbContext
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AdeebBackend.DTOs;

namespace adeeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/employees
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LeftEmployeedDataForCompanyDto>>> GetEmployees()
        {
            var companyId = int.Parse(User.FindFirst("companyId")?.Value);

            var employees = await _context.Employees
                .Where(e => e.CompanyId == companyId)
                .ToListAsync();

            var result = new List<LeftEmployeedDataForCompanyDto>();

            foreach (var emp in employees)
            {
                var entry = await _context.EmployeeSurveyLinks
                    .FirstOrDefaultAsync(s => s.EmployeeId == emp.Id);

                result.Add(new LeftEmployeedDataForCompanyDto
                {
                    Id = emp.Id,
                    FullName = emp.FullName,
                    Email = emp.Email,
                    JoinDate = emp.JoinDate,
                    Department = emp.Department,
                    Position = emp.Position,
                    PhoneNumber = emp.PhoneNumber,
                    SurveyStatus = entry == null ? EmployeeSurvyeStatus.SurveyNotAssigned :
                        entry.IsCompleted ? EmployeeSurvyeStatus.SurveyCompleted : EmployeeSurvyeStatus.SurveySent
                });
            }

            return Ok(result);
        }


        // GET: api/employees/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.Include(e => e.SurveyResponses).FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // POST: api/employees
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Employee>> PostEmployee(AddNewEmployeeDto employee)
        {
            var companyId = int.Parse(User.FindFirst("companyId")?.Value);

            /*  var existing = await _context.Users.AnyAsync(u => u.Email == employee.Email);
             if (existing)
             {
                 return BadRequest("Email already exists.");
             } */

            _context.Employees.Add(new Employee
            {
                FullName = employee.FullName,
                Email = employee.Email,
                JoinDate = employee.JoinDate,
                Department = employee.Department,
                Position = employee.Position,
                PhoneNumber = employee.PhoneNumber,
                CompanyId = companyId
            });
            await _context.SaveChangesAsync();

            return Created();
        }

        // PUT: api/employees/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/employees/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
