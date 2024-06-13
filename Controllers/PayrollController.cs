using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RPFBE.Auth;
using RPFBE.Model;
using RPFBE.Model.PayrollModels;
using RPFBE.Model.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayrollController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<HomeController> logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ICodeUnitWebService codeUnitWebService;
        private readonly IMailService mailService;
        private readonly IOptions<WebserviceCreds> config;

        public PayrollController(
             UserManager<ApplicationUser> userManager,
                ApplicationDbContext dbContext,
                ILogger<HomeController> logger,
                IWebHostEnvironment webHostEnvironment,
                ICodeUnitWebService codeUnitWebService,
                IMailService mailService,
                IOptions<WebserviceCreds> config
            )
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.logger = logger;
            this.webHostEnvironment = webHostEnvironment;
            this.codeUnitWebService = codeUnitWebService;
            this.mailService = mailService;
            this.config = config;
        }
        //Get Payroll Period
        [Authorize]
        [HttpGet]
        [Route("getpayrollperiods")]
        public async Task<IActionResult> GetPayrollPeriods()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                List<PayrollPeriod> periods = new List<PayrollPeriod>();
                var res = await codeUnitWebService.PayrollMGT().GetPayrollPeriodsAsync(user.EmployeeId);
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);
                if(res.return_value != "")
                {
                    foreach (var item in resSerial)
                    {
                        PayrollPeriod pp = new PayrollPeriod
                        {
                            Value = item.Code,
                            Label = item.Description
                        };
                        periods.Add(pp);
                    }
                }
                
                return Ok(new { periods });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Payroll Periods Failed: " + x.Message });
            }
        }

        //Get Payroll Years
        [Authorize]
        [HttpGet]
        [Route("getpayrollyears")]
        public async Task<IActionResult> GetPayrollYears()
        {
            try
            {
                List<PayrollPeriod> years = new List<PayrollPeriod>();
                var res = await codeUnitWebService.PayrollMGT().GetPayrollYearsAsync();
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);
                foreach (var item in resSerial)
                {
                    PayrollPeriod pp = new PayrollPeriod
                    {
                        Value = item.Code,
                        Label = item.Description
                    };
                    years.Add(pp);
                }
                return Ok(new { years });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Payroll Years Failed: " + x.Message });
            }
        }

        //Generate Payslip
        [Authorize]
        [HttpGet]
        [Route("generatepayslip/{PayrollPeriod}")]
        public async Task<IActionResult> GeneratePayslip(string PayrollPeriod)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var resPath = await codeUnitWebService.PayrollMGT().GeneratePayslipAsync(user.EmployeeId, PayrollPeriod);
                var file = resPath.return_value;

                //byte[] b = System.IO.File.ReadAllBytes(file);
                //return  Convert.ToBase64String(b);

                //// Response...
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file,
                    Inline = true // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                return File(System.IO.File.ReadAllBytes(file), "application/pdf");
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Generate Payslip Failed: " + x.Message });
            }
        }

        //Generate P9
        [Authorize]
        [HttpGet]
        [Route("generatepnine/{year}")]
        public async Task<IActionResult> GeneratePnine(int year)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var resPath = await codeUnitWebService.PayrollMGT().GenerateP9Async(user.EmployeeId, year);
                var file = resPath.return_value;

                //byte[] b = System.IO.File.ReadAllBytes(file);
                //return  Convert.ToBase64String(b);

                //// Response...
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file,
                    Inline = true // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                return File(System.IO.File.ReadAllBytes(file), "application/pdf");
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Generate P9 Failed: " + x.Message });
            }
        }
    }
}
