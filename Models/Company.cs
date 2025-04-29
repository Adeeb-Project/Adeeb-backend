using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace adeeb.Models;

public class Company
{
    public Company()
    {
        Employees = new List<Employee>();
        Users = new List<User>();
        Surveys = new List<Survey>();
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    public string? LogoUrl { get; set; }
    public int TotalNumberOfEmployees { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BundleType BundleType { get; set; }

    // Navigation Property to Employee
    public ICollection<Employee> Employees { get; set; }
    // Navigation Property to User
    public ICollection<User> Users { get; set; }
    public ICollection<Survey> Surveys { get; set; }
}

public enum BundleType
{
    Basic,    // Allows 2 users
    Premium   // Allows 5 users + additional features
}