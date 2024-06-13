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
using RPFBE.Model.LMS;
using RPFBE.Service.ErevukaAPI;
using RPFBE.Service.ErevukaAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LMSController : Controller
    {
        private readonly ILogger<LMSController> logger;
        private readonly ILMSService lms;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ICodeUnitWebService codeUnitWebService;
        private readonly IOptions<WebserviceCreds> config;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;

        public LMSController(
            ILogger<LMSController> logger,
            ILMSService lms,
            IWebHostEnvironment webHostEnvironment,
            ICodeUnitWebService codeUnitWebService,
            IOptions<WebserviceCreds> config,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext
        )
        {
            this.logger = logger;
            this.lms = lms;
            this.webHostEnvironment = webHostEnvironment;
            this.codeUnitWebService = codeUnitWebService;
            this.config = config;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        //Get Available Courses
        [Authorize]
        [Route("courses")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                _ = new List<Course>();
                List<Course> courses = await lms.GetCourses(user.Email);

                return Ok(new { courses });
            }
            catch (Exception x)
            {
                logger.LogError($"User:NA,Verb:POST,Action:Get Courses Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Courses Failed: " + x.Message });

            }
           
        }

        /**
         * Selected Courses CRUD
         * 
         * 
         */

        //Get Posted Selected Courses
        [Authorize]
        [HttpGet]
        [Route("selectedcourse/{eid}/{aid}")]
        public async Task<IActionResult> RetrieveSelectedCourse(string eid,string aid)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;
                List<SeletectedCoursesDTO> selectedCourses = new List<SeletectedCoursesDTO>();

                var result = await codeUnitWebService.Client().GetSelectedCoursesAsync(eid, aid);
                dynamic resSeria = JsonConvert.DeserializeObject(result.return_value);

                if(resSeria != null)
                {
                    foreach (var item in resSeria)
                    {
                        SeletectedCoursesDTO dTO = new SeletectedCoursesDTO
                        {
                            LineNo = item.LineNo,
                            Course = new Selectable { Value = item.CourseId, Label = item.CourseName },
                            EmployeeId = item.EmployeeId,
                            EmployeeName = item.EmployeeName,
                            EmployeeEmail = item.EmployeeEmail,
                            AppraisalNo = item.AppraisalNo,
                            CourseScore = item.CourseScore,
                            Origin = item.Origin,
                        };
                        selectedCourses.Add(dTO);
                    };


                    var customeCourse = selectedCourses.Where(x => x.Origin == "1");
                    var selectedCourse = selectedCourses.Where(x => x.Origin == "0");


                    logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Get Selected Courses Success");
                    return Ok(new { selectedCourse, customeCourse });
                }
                else
                {
                var customeCourse = selectedCourses;
                var selectedCourse = selectedCourses;
                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Get Selected Courses Success");
                return Ok(new { selectedCourse, customeCourse });
                }


            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{user.EmployeeId},Verb:GET,Path:Get Selected Courses Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Selected Courses failed: " + x.Message });

            }
        }



        // Post Selected Course
        [Authorize]
        [HttpPost]
        [Route("createselectedcourse")]
        public async Task<IActionResult> PostSelectedCourse([FromBody] SelectedCourse selectedCourse)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;
                var empId = "";
                if (selectedCourse.EmployeeId != null)
                {
                    empId = selectedCourse.EmployeeId;
                }
                else
                {
                    empId = user.EmployeeId;
                }
                var resCoz = await codeUnitWebService.Client().CreateSelectedCourseAsync(selectedCourse.CourseId, selectedCourse.CourseName, empId, selectedCourse.AppraisalNo);

                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Create Selected Course Success");
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Create Selected Course Success", ExtMessage = resCoz.return_value });

            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{user.EmployeeId},Verb:POST,Path:Create Selected Course Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Create Selected Course failed: " + x.Message });

            }
        }

        //Modify Selected Course
        [Authorize]
        [HttpPost]
        [Route("updateselectedcourse")]
        public async Task<IActionResult> ModifySelectedCourse([FromBody] SelectedCourse selectedCourse)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;

                var resAchieve = await codeUnitWebService.Client().UpdateSelectedCourseAsync(selectedCourse.LineNo, selectedCourse.CourseId, selectedCourse.CourseName);
                if (resAchieve.return_value=="true")
                {
                    logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Modify Selected Course:{selectedCourse.LineNo} Success");
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Modiy Selected Course Success" });
                }
                else
                {
                    logger.LogWarning($"User:{user.EmployeeId},Verb:POST,Path:Modify Selected Course Failed");
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify Specific Focus failed" });

                }
            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{user.EmployeeId},Verb:POST,Path:Modify Selected Course Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify Selected Course Failed: " + x.Message });

            }
        }

        //Delete Selected Course 
        [Authorize]
        [HttpDelete]
        [Route("deleteselectedcourse/{lineno}/{cname}")]
        public async Task<IActionResult> DestroySelectedCourse(int lineno,string cname)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;

                var resAchieve = await codeUnitWebService.Client().DeleteSelectedCourseAsync(lineno);
                if (resAchieve.return_value =="true")
                {
                    logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Deleted Selected Course:{cname} Success");
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Delete Specific Focus, Success" });
                }
                else
                {
                    logger.LogWarning($"User:{user.EmployeeId},Verb:POST,Path:Delete Selected Course:{cname} Failed");
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Delete Specific Focus failed" });
                }
            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{user.EmployeeId},Verb:POST,Path:Delete Selected Course:{cname} Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Delete Selected Course:{selectedCourse.courseName} Failed: " + x.Message });

            }
        }


    }
}
