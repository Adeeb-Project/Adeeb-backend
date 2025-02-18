using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using adeeb.Models;

namespace adeeb.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Survey> Surveys { get; set; }
        public DbSet<SurveyResponse> SurveyResponses { get; set; }
        public DbSet<EmployeeSurveyLink> EmployeeSurveyLinks { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public new DbSet<User> Users { get; set; }
    }
}
