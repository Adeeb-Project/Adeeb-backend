using System;
using System.Text.Json.Serialization;

namespace adeeb.Models;

public class Company
{

    public int Id { get; set; }
    public string Name { get; set; }
    public string LogoUrl { get; set; }
    public int TotalNumberOfEmployees { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BundleType BundleType { get; set; }


    // Navigation Property to Employee
    public ICollection<Employee> Employees { get; set; }
    // Navigation Property to User
    public ICollection<User> Users { get; set; }

}

public enum BundleType
{
    Basic,    // Allows 2 users
    Premium   // Allows 5 users + additional features
}