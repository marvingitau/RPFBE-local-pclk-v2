using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RPFBE.Service.ErevukaAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace RPFBE.Service.ErevukaAPI
{
    public class LMSService : ILMSService
    {
        private readonly HttpClient client;

        public LMSService(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            client = clientFactory.CreateClient("ErevukaLMSApi");
            Configuration = configuration;
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");
        }

        public IConfiguration Configuration { get; }

        public async Task<List<Course>> GetCourses(string email)
        {
            var url = string.Format("/api/lms/{0}/{1}/courses", email, Configuration["LMS:CompanyCode"]);
            List<Course> result = new List<Course>();

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var stringResponse = await response.Content.ReadAsStringAsync();

                dynamic resSerial = JsonConvert.DeserializeObject(stringResponse);

                //result = JsonSerializer.Deserialize<List<HolidayModel>>(stringResponse,
                //    new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                foreach (var item in resSerial)
                {
                    Course cos = new Course
                    {
                        Value = item.course_id,
                        Label = item.course_name
                    };
                    result.Add(cos);
                }

            }
            else
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }

            return result;
        }

    }
}
