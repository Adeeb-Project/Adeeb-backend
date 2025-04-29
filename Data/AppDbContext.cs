using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using adeeb.Models;
using Microsoft.EntityFrameworkCore.Metadata;

namespace adeeb.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<SurveyResponse> SurveyResponses { get; set; }
        public DbSet<EmployeeSurveyLink> EmployeeSurveyLinks { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<User> CustomUsers { get; set; }  // Renamed to CustomUsers to avoid conflict

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                      .ValueGeneratedOnAdd()
                      .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
            });

            // Other model configurations...
        }
    }
}
