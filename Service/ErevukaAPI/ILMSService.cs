using Microsoft.Extensions.Configuration;
using RPFBE.Service.ErevukaAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPFBE.Service.ErevukaAPI
{
    public interface ILMSService
    {
        IConfiguration Configuration { get; }

        Task<List<Course>> GetCourses(string email);
    }
}