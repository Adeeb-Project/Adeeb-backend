using System.Collections.Generic;
using System.Threading.Tasks;
using AdeebBackend.Models;

namespace AdeebBackend.Services
{
    public interface IChatGPTService
    {
        Task<string> SummarizeEmployeesAsync(List<Employee> employees);
        Task<string> ImproveQuestionAsync(string question);
    }
}
