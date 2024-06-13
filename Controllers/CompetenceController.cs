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
using RPFBE.Model.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompetenceController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<HomeController> logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ICodeUnitWebService codeUnitWebService;
        private readonly IMailService mailService;
        private readonly IOptions<WebserviceCreds> config;

        public CompetenceController(
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

        //Get Staff Competence List
        [Authorize]
        [HttpGet]
        [Route("getstaffcompetencelist")]

        public async Task<IActionResult> GetStaffCompetenceList()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                List<CompetenceFrameworkModel> competenceFrameworks = new List<CompetenceFrameworkModel>();
                var res = await codeUnitWebService.Client().GetCompetenceListStaffAsync(user.EmployeeId);
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);

                if(resSerial != null)
                {
                    foreach (var item in resSerial)
                    {
                        CompetenceFrameworkModel cfm = new CompetenceFrameworkModel
                        {
                            Cno = item.Cno,
                            Staffno = item.Staffno,
                            Staffname = item.Staffname,
                            Supervisorno = item.Supervisorno,
                            Supervisorname = item.Supervisorname,
                            Startdate = item.Startdate,
                            Enddate = item.Enddate,
                            Behavescoreemp = item.Behavescoreemp,
                            Behavescoresup = item.Behavescoresup,
                            Behavescoreavg = item.Behavescoreavg,
                            Techscoreemp = item.Techscoreemp,
                            Techscoresup = item.Techscoresup,
                            Techscoreavg = item.Techscoreavg,
                            Averagescore = item.Averagescore,
                            Percentagescore = item.Percentagescore,
                            Status = item.Status,
                        };
                        competenceFrameworks.Add(cfm);
                    }
                    return Ok(new { competenceFrameworks });
                }
                else
                {
                    return Ok(new { competenceFrameworks });
                }


            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Staff Competence Failed: " + x.Message });
            }
        }

        //Get Staff Compentence General
        [Authorize]
        [HttpGet]
        [Route("getstaffcompetencegeneral/{cno}")]
        public async Task<IActionResult> GetStaffCompetenceGeneral(string cno)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                List<CompetenceFrameworkModel> competenceGeneral = new List<CompetenceFrameworkModel>();
                var res = await codeUnitWebService.Client().GetCompetenceGeneralStaffAsync(cno);
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);

                if (resSerial != null)
                {
                    foreach (var item in resSerial)
                    {
                        CompetenceFrameworkModel cfm = new CompetenceFrameworkModel
                        {
                            Cno = item.Cno,
                            Staffno = item.Staffno,
                            Staffname = item.Staffname,
                            Supervisorno = item.Supervisorno,
                            Supervisorname = item.Supervisorname,
                            Startdate = item.Startdate,
                            Enddate = item.Enddate,
                            Behavescoreemp = item.Behavescoreemp,
                            Behavescoresup = item.Behavescoresup,
                            Behavescoreavg = item.Behavescoreavg,
                            Techscoreemp = item.Techscoreemp,
                            Techscoresup = item.Techscoresup,
                            Techscoreavg = item.Techscoreavg,
                            Averagescore = item.Averagescore,
                            Percentagescore = item.Percentagescore,
                            Status = item.Status,
                        };
                        competenceGeneral.Add(cfm);
                    }
                    return Ok(new { competenceGeneral });
                }
                else
                {
                    return Ok(new { competenceGeneral });
                }


            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Staff Competence General Failed: " + x.Message });
            }
        }

        //Get Staff Competence Line
        [Authorize]
        [HttpGet]
        [Route("getstaffcompetencelines/{cno}")]
        public async Task<IActionResult> GetStaffCompetenceLines(string cno)
        {
            try
            {
                List<CompetenceLineModel> competenceLines = new List<CompetenceLineModel>();
                var res = await codeUnitWebService.Client().GetCompetenceLineStaffAsync(cno);
                dynamic resS = JsonConvert.DeserializeObject(res.return_value);

                if(resS != null)
                {
                    foreach (var item in resS)
                    {
                        CompetenceLineModel clm = new CompetenceLineModel
                        {
                            Cno = item.Cno,
                            Lineno = item.Lineno,
                            Type = item.Type,
                            Competence = item.Competence,
                            Description = item.Description,
                            Employeeassesment = item.Employeeassesment,
                            Employeecomment = item.Employeecomment,
                            Supervisorassesment = item.Supervisorassesment,
                            Supervisorcomment = item.Supervisorcomment,
                        };
                        competenceLines.Add(clm);
                    }
                    return Ok(new { competenceLines });
                }
                else
                {
                    return Ok(new { competenceLines });
                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Staff Competence Lines Failed: " + x.Message });
            }
        }

        //Modify Staff Compentence Line
        [Authorize]
        [HttpPost]
        [Route("modifystaffcompetenceline")]
        public async Task<IActionResult> ModifyStaffCompetenceLine([FromBody] CompetenceLineModel competenceLineModel)
        {
            try
            {
                var res = await codeUnitWebService.Client().ModifyCompetencyLineAsync(competenceLineModel.Cno,Int32.Parse(competenceLineModel.Lineno), competenceLineModel.Employeeassesment, competenceLineModel.Employeecomment, "0"); //Stadd
                return Ok(new { res.return_value });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify  Competence Line Failed: " + x.Message });
            }
        }

        //Staff Calculate score
        [Authorize]
        [HttpGet]
        [Route("staffcalculatescore/{cno}")]
        public async Task<IActionResult> StaffCalculateScore(string cno)
        {
            try
            {
                var res = await codeUnitWebService.Client().CalculateScoresStaffAsync(cno);
                return Ok(res.return_value);
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Competence Calculation Failed: " + x.Message });
            }
        }

        //Move Competence Record to Supervisor
        [Authorize]
        [HttpGet]
        [Route("staffpushtosupervisor/{cno}")]
        public async Task<IActionResult> StaffPushToSup(string cno)
        {
            try
            {
                var res = await codeUnitWebService.Client().PushToSupervisorAsync(cno);
                return Ok(res.return_value);
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Push to Supervisor failed: " + x.Message });
            }
        }
        //Get Competency Report
        [Authorize]
        [HttpGet]
        [Route("getcompentencyreport/{cno}")]
        public async Task<IActionResult> GetCompentencyReport(string cno)
        {
            try
            {
                var path = await codeUnitWebService.Client().GetCompetenceReportAsync(cno);
                var file = path.return_value;

                // Response...
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

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Report failed: " + x.Message });
            }
        }
        //Supervisor Competence Record
        [Authorize]
        [HttpGet]
        [Route("getsupercompetencelist")]
        public async Task<IActionResult> GetSuperCompetenceList()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                List<CompetenceFrameworkModel> competenceFrameworks = new List<CompetenceFrameworkModel>();
                var res = await codeUnitWebService.Client().GetCompetenceListManagerAsync(user.EmployeeId);
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);

                if (resSerial != null)
                {
                    foreach (var item in resSerial)
                    {
                        CompetenceFrameworkModel cfm = new CompetenceFrameworkModel
                        {
                            Cno = item.Cno,
                            Staffno = item.Staffno,
                            Staffname = item.Staffname,
                            Supervisorno = item.Supervisorno,
                            Supervisorname = item.Supervisorname,
                            Startdate = item.Startdate,
                            Enddate = item.Enddate,
                            Behavescoreemp = item.Behavescoreemp,
                            Behavescoresup = item.Behavescoresup,
                            Behavescoreavg = item.Behavescoreavg,
                            Techscoreemp = item.Techscoreemp,
                            Techscoresup = item.Techscoresup,
                            Techscoreavg = item.Techscoreavg,
                            Averagescore = item.Averagescore,
                            Percentagescore = item.Percentagescore,
                            Status = item.Status,
                        };
                        competenceFrameworks.Add(cfm);
                    }
                    return Ok(new { competenceFrameworks });
                }
                else
                {
                    return Ok(new { competenceFrameworks });
                }


            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Supervisor Competence Failed: " + x.Message });
            }
        }
        
        //Modify Supervisor Compentence Line
        [Authorize]
        [HttpPost]
        [Route("modifysupervisorcompetenceline")]
        public async Task<IActionResult> ModifySupervisorCompetenceLine([FromBody] CompetenceLineModel competenceLineModel)
        {
            try
            {
                var res = await codeUnitWebService.Client().ModifyCompetencyLineAsync(competenceLineModel.Cno,Int32.Parse(competenceLineModel.Lineno), competenceLineModel.Supervisorassesment, competenceLineModel.Supervisorcomment, "1"); //Supervisr
                return Ok(new { res.return_value });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Supervisor Competence Line Modification Failed: " + x.Message });
            }
        }
        //Supervisor Calculate score
        [Authorize]
        [HttpPost]
        [Route("supervisorcalculatescore")]
        public async Task<IActionResult> SupervisorCalculateScore([FromBody] CompetenceLineModel competenceLineModel)
        {
            try
            {
                var res = await codeUnitWebService.Client().CalculateScoresManagerAsync(competenceLineModel.Cno, competenceLineModel.GeneralSupervisorcomment);
                return Ok(res.return_value);
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Competence Calculation Failed: " + x.Message });
            }
        }
    }
}
