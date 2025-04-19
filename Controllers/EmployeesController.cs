using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using adeeb.Models;
using adeeb.Data; // using AppDbContext not ApplicationDbContext
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adeeb.Services; // Ensure this matches your Services folder namespace

namespace adeeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IChatGPTService _chatGPTService;

        // Update the constructor to inject IChatGPTService along with AppDbContext.
        public EmployeesController(AppDbContext context, IChatGPTService chatGPTService)
        {
            _context = context;
            _chatGPTService = chatGPTService;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        // GET: api/employees/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees
                                .Include(e => e.SurveyResponses)
                                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
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

        // NEW: GET: api/employees/summary
        // This endpoint compiles all employees' data and sends it to ChatGPT for summarization.
        [HttpGet("summary")]
        public async Task<IActionResult> GetEmployeesSummary()
        {
            var employees = await _context.Employees.ToListAsync();
            if (employees == null || employees.Count == 0)
            {
                return NotFound("No employees found to summarize.");
            }

            string summary = await _chatGPTService.SummarizeEmployeesAsync(employees);
            return Ok(new { summary });
        }
    }
}
