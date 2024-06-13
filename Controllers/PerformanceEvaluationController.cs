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
using RPFBE.Model.Performance;
using RPFBE.Model.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerformanceEvaluationController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<HomeController> logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ICodeUnitWebService codeUnitWebService;
        private readonly IMailService mailService;
        private readonly IOptions<WebserviceCreds> config;

        public PerformanceEvaluationController(
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

        //Employee Appraisals
        [Authorize]
        [HttpGet]
        [Route("getemployeeappraisals")]
        public async Task<IActionResult> GetEmployeeAppraisal()
        {
            try
            {
                List<EmployeeAppraisalList> employeeAppraisalLists = new List<EmployeeAppraisalList>();
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var apprRes = await codeUnitWebService.HRWS().GetEmployeePerformanceAppraisalEmployeeNoAsync(user.EmployeeId);
                dynamic apprSerial = JsonConvert.DeserializeObject(apprRes.return_value);
                foreach (var item in apprSerial.EmployeeAppraisals)
                {
                    EmployeeAppraisalList eal = new EmployeeAppraisalList
                    {
                        No = item.No,
                        EmployeeNo = item.EmployeeNo,
                        KPICode = item.KPICode,
                        EmployeeName = item.EmployeeName,
                        EmployeeDesgnation = item.EmployeeDesgnation,
                        JobTitle = item.JobTitle,
                        ManagerNo = item.ManagerNo,
                        ManagerName = item.ManagerName,
                        ManagerDesignation = item.ManagerDesignation,
                        AppraisalPeriod = item.AppraisalPeriod,
                        AppraisalStartPeriod = item.AppraisalStartPeriod,
                        AppraisalEndPeriod = item.AppraisalEndPeriod,
                        AppraisalLevel = item.AppraisalLevel,
                    };
                    employeeAppraisalLists.Add(eal);
                }
                return Ok(new { employeeAppraisalLists });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Job Performance KPIs failed: " + x.Message });

            }
        }


        //Get Employee Appraisal Performance Standard List
        // All the KPI stds
        [Authorize]
        [HttpGet]
        [Route("getappraisalperformancestd/{AppraisalNo}")]
        public async Task<IActionResult> GetAppraisalPerformancestd(string AppraisalNo)
        {
            try
            {
                List<EmployeeAppraisalStandard> employeeAppraisalStandards = new List<EmployeeAppraisalStandard>();
                var resL = await codeUnitWebService.HRWS().GetEmployeePerformanceAppraisalIndicatorsAsync(AppraisalNo);
                dynamic esSer = JsonConvert.DeserializeObject(resL.return_value);

                foreach (var item in esSer)
                {
                    EmployeeAppraisalStandard eas = new EmployeeAppraisalStandard
                    {
                        CriteriaCode = item.CriteriaCode,
                        TargetCode = item.TargetCode,
                        KPIDescription = item.KPIDescription,
                        HeaderNo = item.HeaderNo,
                        StandardCode = item.StandardCode,
                        IndicatorCode = item.IndicatorCode,
                        StandardDescription = item.StandardDescription,
                        StandardWeighting = item.StandardWeighting,
                        Timelines = item.Timelines,
                        ActivityDescription = item.ActivityDescription,
                        TargetedScore = item.TargetedScore,
                        AchievedScoreEmployee = item.AchievedScoreEmployee,
                        EmployeeComments = item.EmployeeComments,
                    };
                    employeeAppraisalStandards.Add(eas);
                }
                return Ok(new { employeeAppraisalStandards });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Employee Appraisal Perfomance Std failed: " + x.Message });

            }
        }

        //Get Supervisor Employee Appraisal Performance Standard List
        // All the KPI stds
        [Authorize]
        [HttpGet]
        [Route("getsupervisorappraisalperformancestd/{AppraisalNo}")]
        public async Task<IActionResult> GetSupervisorAppraisalPerformancestd(string AppraisalNo)
        {
            try
            {
                List<SupervisorAppraisalStandard> employeeAppraisalStandards = new List<SupervisorAppraisalStandard>();
                var resL = await codeUnitWebService.HRWS().GetSupervisorPerformanceAppraisalIndicatorsAsync(AppraisalNo);
                dynamic esSer = JsonConvert.DeserializeObject(resL.return_value);

                foreach (var item in esSer)
                {
                    SupervisorAppraisalStandard eas = new SupervisorAppraisalStandard
                    {
                        CriteriaCode = item.CriteriaCode,
                        TargetCode = item.TargetCode,
                        KPIDescription = item.KPIDescription,
                        HeaderNo = item.HeaderNo,
                        StandardCode = item.StandardCode,
                        IndicatorCode = item.IndicatorCode,
                        StandardDescription = item.StandardDescription,
                        StandardWeighting = item.StandardWeighting,
                        Timelines = item.Timelines,
                        ActivityDescription = item.ActivityDescription,
                        TargetedScore = item.TargetedScore,
                        AchievedScoreSupervisor = item.AchievedScoreSupervisor,
                        SupervisorComments = item.SupervisorComments,
                    };
                    employeeAppraisalStandards.Add(eas);
                }
                return Ok(new { employeeAppraisalStandards });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Employee Appraisal Perfomance Std failed: " + x.Message });

            }
        }


        //Get Moderated Supervisor Employee Appraisal Performance Standard List
        // All the KPI stds
        [Authorize]
        [HttpGet]
        [Route("getmoderatedsupervisorappraisalperformancestd/{AppraisalNo}")]
        public async Task<IActionResult> GetModeratedSupervisorAppraisalPerformancestd(string AppraisalNo)
        {
            try
            {
                List<SupervisorAppraisalStandard> employeeAppraisalStandards = new List<SupervisorAppraisalStandard>();
                var resL = await codeUnitWebService.HRWS().GetModeratedPerformanceAppraisalIndicatorsAsync(AppraisalNo);
                dynamic esSer = JsonConvert.DeserializeObject(resL.return_value);

                foreach (var item in esSer)
                {
                    SupervisorAppraisalStandard eas = new SupervisorAppraisalStandard
                    {
                        CriteriaCode = item.CriteriaCode,
                        TargetCode = item.TargetCode,
                        KPIDescription = item.KPIDescription,
                        HeaderNo = item.HeaderNo,
                        StandardCode = item.StandardCode,
                        IndicatorCode = item.IndicatorCode,
                        StandardDescription = item.StandardDescription,
                        StandardWeighting = item.StandardWeighting,
                        Timelines = item.Timelines,
                        ActivityDescription = item.ActivityDescription,
                        TargetedScore = item.TargetedScore,
                        AchievedScoreEmployee = item.AchievedScoreEmployee,
                        AchievedScoreSupervisor = item.AchievedScoreSupervisor,
                        OverallAchievedScore = item.OverallAchievedScore,
                        EmployeeComments = item.EmployeeComments,
                        SupervisorComments = item.SupervisorComments,
                    };
                    employeeAppraisalStandards.Add(eas);
                }
                return Ok(new { employeeAppraisalStandards });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Employee Appraisal Perfomance Std failed: " + x.Message });

            }
        }





        //Get Employee Appraisal Performance Standard List
        // Filtere the KPI stds

        [Authorize]
        [HttpGet]
        [Route("getappraisalperformancestdperkpi/{KPICode}/{HeaderNo}")]
        public async Task<IActionResult> GetAppraisalPerformancestdPerKPI(int KPICode, string HeaderNo)
        {
            try
            {
                List<EmployeeAppraisalStandard> employeeAppraisalStandards = new List<EmployeeAppraisalStandard>();
                var resL = await codeUnitWebService.HRWS().GetEmployeePerformanceAppraisalIndicatorsPerKPIAsync(KPICode, HeaderNo);
                dynamic esSer = JsonConvert.DeserializeObject(resL.return_value);

                foreach (var item in esSer)
                {
                    EmployeeAppraisalStandard eas = new EmployeeAppraisalStandard
                    {
                        CriteriaCode = item.CriteriaCode,
                        TargetCode = item.TargetCode,
                        KPIDescription = item.KPIDescription,
                        HeaderNo = item.HeaderNo,
                        StandardCode = item.StandardCode,
                        IndicatorCode = item.IndicatorCode,
                        StandardDescription = item.StandardDescription,
                        StandardWeighting = item.StandardWeighting,
                        Timelines = item.Timelines,
                        ActivityDescription = item.ActivityDescription,
                        TargetedScore = item.TargetedScore,
                        AchievedScoreEmployee = item.AchievedScoreEmployee,
                        EmployeeComments = item.EmployeeComments,
                    };
                    employeeAppraisalStandards.Add(eas);
                }
                return Ok(new { employeeAppraisalStandards });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Employee Appraisal Perfomance Std failed: " + x.Message });

            }
        }

  

        /*
         * 
         **/
        //Update on Employee Appraisal
        [Authorize]
        [HttpPost]
        [Route("updateemployeeappraisal")]
        public async Task<IActionResult> UpdateEmployeeAppraisal([FromBody] ModifyEmployeeAppraisal modifyEmployee)
        {
            try
            {
                var updareRes = await codeUnitWebService.HRWS().ModifyEmployeePerformanceAppraisalIndicatorAsync(modifyEmployee.TargetCode, modifyEmployee.IndicatorCode, modifyEmployee.KPICode, modifyEmployee.HeaderNo, modifyEmployee.AchievedScore, modifyEmployee.EmployeeComments);
                if (updareRes.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Modify Employee Appraisal Target, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify Employee Appraisal Target failed" });

                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Update Employee Appraisal failed: " + x.Message });

            }
        }

        /*
        * 
        **/
        //Update on Supervisor Employee Appraisal
        [Authorize]
        [HttpPost]
        [Route("updatesupervisoremployeeappraisal")]
        public async Task<IActionResult> UpdateSupervisorEmployeeAppraisal([FromBody] ModifyEmployeeAppraisal modifyEmployee)
        {
            try
            {
                var updareRes = await codeUnitWebService.HRWS().ModifySupervisorPerformanceAppraisalIndicatorAsync(modifyEmployee.TargetCode, modifyEmployee.IndicatorCode, modifyEmployee.KPICode, modifyEmployee.HeaderNo, modifyEmployee.AchievedScore, modifyEmployee.EmployeeComments);
                if (updareRes.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Modify Employee Appraisal Target, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify Employee Appraisal Target failed" });

                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Update Employee Appraisal failed: " + x.Message });

            }
        }




        //Calculate Weights
        [Authorize]
        [HttpGet]
        [Route("calculateweight/{HeaderNo}")]
        public async Task<IActionResult> CalculateWeght(string HeaderNo)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var calcRes = await codeUnitWebService.HRWS().CalculateEmployeeAppraisalScoreAsync(user.EmployeeId, HeaderNo);
                if (calcRes.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Calculate Weight, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Calculate Weight failed" });

                }

            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Calculate Weightage failed: " + x.Message });

            }
        }

        //Submit to Supervisor
        [Authorize]
        [HttpGet]
        [Route("submittosupervisor/{HeaderNo}")]
        public async Task<IActionResult> SubmitToSupervisor(string HeaderNo)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var subRes = await codeUnitWebService.HRWS().SubmitEmployeeAppraisalScoreAsync(user.EmployeeId, HeaderNo);
                if (subRes.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Submit to Supervisor, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Submit to Supervisor Failed" });

                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Submit to Supervisor Failed: " + x.Message });

            }
        }


        //Get Reflection Section Data
        [Authorize]
        [HttpGet]
        [Route("getreflectiondata/{HeaderNo}")]
        public async Task<IActionResult> GetReflectionData(string HeaderNo)
        {
            try
            {
                //Area of Achievement
                List<ReflectionDataAchieve> areaofAchievementList = new List<ReflectionDataAchieve>();
                var achieveArea = await codeUnitWebService.HRWS().GetAppraisalAreaofAchievementAsync(HeaderNo);
                dynamic achieveSerial = JsonConvert.DeserializeObject(achieveArea.return_value);
                foreach (var item in achieveSerial)
                {
                    ReflectionDataAchieve achievement = new ReflectionDataAchieve
                    {
                        LineNo = item.LineNo,
                        HeaderNo = item.HeaderNo,
                        AreaOfAchievement = item.AreaofAchievement,
                        //SpecificFocusArea = "",
                        //AreaOfDevelopment = "",
                    };
                    areaofAchievementList.Add(achievement);
                }

                //Specific Focus Area
                List<ReflectionDataFocus> specificFocusList = new List<ReflectionDataFocus>();
                var focusArea = await codeUnitWebService.HRWS().GetAppraisalSpecificFocusAsync(HeaderNo);
                dynamic focusSerial = JsonConvert.DeserializeObject(focusArea.return_value);
                foreach (var itm in focusSerial)
                {
                    ReflectionDataFocus focus = new ReflectionDataFocus
                    {
                        LineNo = itm.LineNo,
                        HeaderNo = itm.HeaderNo,
                        SpecificFocusArea = itm.SpecificFocus,
                    };
                    specificFocusList.Add(focus);
                }

                //Area Of Development
                List<ReflectionDataDevelopment> areaofDevelopmentList = new List<ReflectionDataDevelopment>();
                var develArea = await codeUnitWebService.HRWS().GetAppraisalAreaofDevelopmentAsync(HeaderNo);
                dynamic developSerial = JsonConvert.DeserializeObject(develArea.return_value);
                foreach (var ite in developSerial)
                {
                    ReflectionDataDevelopment focus = new ReflectionDataDevelopment
                    {
                        LineNo = ite.LineNo,
                        HeaderNo = ite.HeaderNo,
                       // AreaOfAchievement = "",
                       // SpecificFocusArea = "",
                        AreaOfDevelopment = ite.AreaOfDevelopment,
                    };
                    areaofDevelopmentList.Add(focus);
                }


                return Ok(new { areaofAchievementList, specificFocusList, areaofDevelopmentList });

            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Reflection Section Data failed: " + x.Message });

            }
        }

        //Create Area of Achievement
        [Authorize]
        [HttpPost]
        [Route("createareaofachievement")]
        public async Task<IActionResult> CreateAreaofAchievement([FromBody] ReflectionDataAchieve dataAchieve)
        {
            try
            {
                var resAchieve = await codeUnitWebService.HRWS().CreateAppraisalAreaofAchievementAPIAsync(dataAchieve.HeaderNo, dataAchieve.AreaOfAchievement);
               
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Create Area of Achievement, Success",ExtMessage= resAchieve.return_value });
               
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Create Area of Achievement failed: " + x.Message });

            }
        }

        //Modify Area of Achievement
        [Authorize]
        [HttpPost]
        [Route("modifyareaofachievement")]
        public async Task<IActionResult> ModifyAreaofAchievement([FromBody] ReflectionDataAchieve dataAchieve)
        {
            try
            {
                var resAchieve = await codeUnitWebService.HRWS().ModifyAppraisalAreaofAchievementAsync(dataAchieve.HeaderNo, Int16.Parse(dataAchieve.LineNo) ,dataAchieve.AreaOfAchievement);
                if (resAchieve.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Modiy Area of Achievement, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify Area of Achievement failed" });

                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify Area of Achievement failed: " + x.Message });

            }
        }

        //Delete Area of Achievement
        [Authorize]
        [HttpPost]
        [Route("deleteareaofachievement")]
        public async Task<IActionResult> DeleteAreaofAchievement([FromBody] ReflectionDataAchieve dataAchieve)
        {
            try
            {
                var resAchieve = await codeUnitWebService.HRWS().DeleteAppraisalAreaofAchievementAsync(dataAchieve.HeaderNo, Int16.Parse(dataAchieve.LineNo));
                if (resAchieve.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Delete Area of Achievement, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Delete Area of Achievement failed" });
                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Delete Area of Achievement failed: " + x.Message });

            }
        }



        //Create Area of Development
        [Authorize]
        [HttpPost]
        [Route("createareaofdevelopment")]
        public async Task<IActionResult> CreateAreaofDevelopment([FromBody] ReflectionDataDevelopment reflectionDevelopment)
        {
            try
            {
                var resAchieve = await codeUnitWebService.HRWS().CreateAppraisalAreaofDevelopmentAPIAsync(reflectionDevelopment.HeaderNo, reflectionDevelopment.AreaOfDevelopment);
               
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Create Area of Development, Success",ExtMessage= resAchieve.return_value });
               
                
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Create Area of Developement failed: " + x.Message });

            }
        }

        //Modify Area of Development
        [Authorize]
        [HttpPost]
        [Route("modifyareaofdevelopment")]
        public async Task<IActionResult> ModifyAreaofDevelopment([FromBody] ReflectionDataDevelopment reflectionDevelopment)
        {
            try
            {
                var resAchieve = await codeUnitWebService.HRWS().ModifyAppraisalAreaofDevelopmentAsync(reflectionDevelopment.HeaderNo, Int16.Parse(reflectionDevelopment.LineNo), reflectionDevelopment.AreaOfDevelopment);
                if (resAchieve.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Modiy Area of Development, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify Area of Development failed" });

                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify Area of Development failed: " + x.Message });

            }
        }

        //Delete Area of Development
        [Authorize]
        [HttpPost]
        [Route("deleteareaofdevelopment")]
        public async Task<IActionResult> DeleteAreaofDevelopment([FromBody] ReflectionDataDevelopment reflectionDevelopment)
        {
            try
            {
                var resAchieve = await codeUnitWebService.HRWS().DeleteAppraisalAreaofDevelopmentAsync(reflectionDevelopment.HeaderNo, Int16.Parse(reflectionDevelopment.LineNo));
                if (resAchieve.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Delete Area of Development, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Delete Area of Development failed" });
                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Delete Area of Development failed: " + x.Message });

            }
        }





        //Create Specific Focus 
        [Authorize]
        [HttpPost]
        [Route("createspecificfocus")]
        public async Task<IActionResult> CreateSpecicFocus([FromBody] ReflectionDataFocus dataFocus)
        {
            try
            {
                var resAchieve = await codeUnitWebService.HRWS().CreateAppraisalSpecificFocusAPIAsync(dataFocus.HeaderNo, dataFocus.SpecificFocusArea);
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Create Specific Focus, Success",ExtMessage= resAchieve.return_value });
               
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Create Specific Focus failed: " + x.Message });

            }
        }

        //Modify Specific Focus 
        [Authorize]
        [HttpPost]
        [Route("modifyspecificfocus")]
        public async Task<IActionResult> ModifySpecicFocus([FromBody] ReflectionDataFocus dataFocus)
        {
            try
            {
                var resAchieve = await codeUnitWebService.HRWS().ModifyAppraisalSpecificFocusAsync(dataFocus.HeaderNo, Int16.Parse(dataFocus.LineNo), dataFocus.SpecificFocusArea);
                if (resAchieve.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Modiy Specific Focus, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify Specific Focus failed" });

                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify Specific Focus failed: " + x.Message });

            }
        }

        //Delete Specific Focus 
        [Authorize]
        [HttpPost]
        [Route("deletespecificfocus")]
        public async Task<IActionResult> DeleteSpecicFocus([FromBody] ReflectionDataFocus dataFocus)
        {
            try
            {
                var resAchieve = await codeUnitWebService.HRWS().DeleteAppraisalSpecificFocusAsync(dataFocus.HeaderNo, Int16.Parse(dataFocus.LineNo));
                if (resAchieve.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Delete Specific Focus, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Delete Specific Focus failed" });
                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Delete Specific Focus failed: " + x.Message });

            }
        }



        /*
        * **********************************************************************************************************************
        * 
        *          Supervisor Appraisal
        * 
        * **********************************************************************************************************************
        */


        //Supervisor Appraisals
        [Authorize]
        [HttpGet]
        [Route("getsupervisorappraisals")]
        public async Task<IActionResult> GetSupervisorAppraisal()
        {
            try
            {
                List<EmployeeAppraisalList> supervisorAppraisalLists = new List<EmployeeAppraisalList>();
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var apprRes = await codeUnitWebService.HRWS().GetSupervisorPerformanceAppraisalEmployeeNoAsync(user.EmployeeId);
                dynamic apprSerial = JsonConvert.DeserializeObject(apprRes.return_value);
                foreach (var item in apprSerial.EmployeeAppraisals)
                {
                    EmployeeAppraisalList eal = new EmployeeAppraisalList
                    {
                        No = item.No,
                        EmployeeNo = item.EmployeeNo,
                        KPICode = item.KPICode,
                        EmployeeName = item.EmployeeName,
                        EmployeeDesgnation = item.EmployeeDesgnation,
                        JobTitle = item.JobTitle,
                        ManagerNo = item.ManagerNo,
                        ManagerName = item.ManagerName,
                        ManagerDesignation = item.ManagerDesignation,
                        AppraisalPeriod = item.AppraisalPeriod,
                        AppraisalStartPeriod = item.AppraisalStartPeriod,
                        AppraisalEndPeriod = item.AppraisalEndPeriod,
                        AppraisalLevel = item.AppraisalLevel,
                    };
                    supervisorAppraisalLists.Add(eal);
                }
                return Ok(new { supervisorAppraisalLists });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Supervisor Appraisal Failed: " + x.Message });

            }
        }

        //Get Supervisor Appraisal Performance Standard List
        // Filtere the KPI stds

        [Authorize]
        [HttpGet]
        [Route("getsupervisorappraisalperformancestdperkpi/{KPICode}/{HeaderNo}")]
        public async Task<IActionResult> GetSupervisorAppraisalPerformancestdPerKPI(int KPICode, string HeaderNo)
        {
            try
            {
                List<SupervisorAppraisalStandard> supervisorAppraisalStandards = new List<SupervisorAppraisalStandard>();
                var resL = await codeUnitWebService.HRWS().GetSupervisorPerformanceAppraisalIndicatorsPerKPIAsync(KPICode, HeaderNo);
                dynamic esSer = JsonConvert.DeserializeObject(resL.return_value);

                foreach (var item in esSer)
                {
                    SupervisorAppraisalStandard eas = new SupervisorAppraisalStandard
                    {
                        CriteriaCode = item.CriteriaCode,
                        TargetCode = item.TargetCode,
                        KPIDescription = item.KPIDescription,
                        HeaderNo = item.HeaderNo,
                        StandardCode = item.StandardCode,
                        IndicatorCode = item.IndicatorCode,
                        StandardDescription = item.StandardDescription,
                        StandardWeighting = item.StandardWeighting,
                        Timelines = item.Timelines,
                        ActivityDescription = item.ActivityDescription,
                        TargetedScore = item.TargetedScore,
                        AchievedScoreEmployee = item.AchievedScoreEmployee,
                        OverallAchievedScore = item.OverallAchievedScore,
                        AchievedScoreSupervisor = item.AchievedScoreSupervisor,
                        EmployeeComments = item.EmployeeComments,
                        SupervisorComments = item.SupervisorComments,
                    };
                    supervisorAppraisalStandards.Add(eas);
                }
                return Ok(new { supervisorAppraisalStandards });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Supervisor Appraisal Perfomance Std failed: " + x.Message });

            }
        }


        //Get Reflection Section Data Supervisor
        [Authorize]
        [HttpGet]
        [Route("getreflectionsupervisordata/{HeaderNo}")]
        public async Task<IActionResult> GetReflectionSupervisorData(string HeaderNo)
        {
            try
            {
           

              
                //Area Of Improvement
                List<ReflectionDataDevelopment> areaofDevelopmentList = new List<ReflectionDataDevelopment>();
                var develArea = await codeUnitWebService.HRWS().GetAppraisalAreaofImprovementAsync(HeaderNo);
                dynamic developSerial = JsonConvert.DeserializeObject(develArea.return_value);
                foreach (var ite in developSerial)
                {
                    ReflectionDataDevelopment focus = new ReflectionDataDevelopment
                    {
                        LineNo = ite.LineNo,
                        HeaderNo = ite.HeaderNo,
                        AreaOfDevelopment = ite.AreaofImprovement,
                    };
                    areaofDevelopmentList.Add(focus);
                }

                //Area of Achievement
                List<ReflectionDataAchieve> areaofAchievementList = new List<ReflectionDataAchieve>();
                var achieveArea = await codeUnitWebService.HRWS().GetAppraisalAreaofAchievementAsync(HeaderNo);
                dynamic achieveSerial = JsonConvert.DeserializeObject(achieveArea.return_value);
                foreach (var item in achieveSerial)
                {
                    ReflectionDataAchieve achievement = new ReflectionDataAchieve
                    {
                        LineNo = item.LineNo,
                        HeaderNo = item.HeaderNo,
                        AreaOfAchievement = item.AreaofAchievement,
                        //SpecificFocusArea = "",
                        //AreaOfDevelopment = "",
                    };
                    areaofAchievementList.Add(achievement);
                }


                //Specific Focus Area
                List<ReflectionDataFocus> specificFocusList1 = new List<ReflectionDataFocus>();
                var focusArea1 = await codeUnitWebService.HRWS().GetAppraisalSpecificFocusAsync(HeaderNo);
                dynamic focusSerial1 = JsonConvert.DeserializeObject(focusArea1.return_value);
                foreach (var itm1 in focusSerial1)
                {
                    ReflectionDataFocus focus = new ReflectionDataFocus
                    {
                        LineNo = itm1.LineNo,
                        HeaderNo = itm1.HeaderNo,
                        SpecificFocusArea = itm1.SpecificFocus,
                    };
                    specificFocusList1.Add(focus);
                }

                //Area Of Development supervisor
                List<ReflectionDataDevelopment> areaofDevelopmentList1 = new List<ReflectionDataDevelopment>();
                var develArea1 = await codeUnitWebService.HRWS().GetAppraisalAreaofDevelopmentAsync(HeaderNo);
                dynamic developSerial1 = JsonConvert.DeserializeObject(develArea1.return_value);
                foreach (var ite1 in developSerial1)
                {
                    ReflectionDataDevelopment focus = new ReflectionDataDevelopment
                    {
                        LineNo = ite1.LineNo,
                        HeaderNo = ite1.HeaderNo,
                        // AreaOfAchievement = "",
                        // SpecificFocusArea = "",
                        AreaOfDevelopment = ite1.AreaOfDevelopment,
                    };
                    areaofDevelopmentList1.Add(focus);
                }

                //Specific Training Need supervisor
                List<ReflectionDataFocus> specificFocusList = new List<ReflectionDataFocus>();
                var focusArea = await codeUnitWebService.HRWS().GetAppraisalTrainingNeedsAsync(HeaderNo);
                dynamic focusSerial = JsonConvert.DeserializeObject(focusArea.return_value);
                foreach (var itm in focusSerial)
                {
                    ReflectionDataFocus focus = new ReflectionDataFocus
                    {
                        LineNo = itm.LineNo,
                        HeaderNo = itm.HeaderNo,
                        SpecificFocusArea = itm.TrainingNeed,
                    };
                    specificFocusList.Add(focus);
                }



                return Ok(new { specificFocusList, areaofDevelopmentList, areaofAchievementList, specificFocusList1, areaofDevelopmentList1 });

            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Reflection Section Data failed: " + x.Message });

            }
        }

        //Create Area of Improvement
        [Authorize]
        [HttpPost]
        [Route("createareaofimprovement")]
        public async Task<IActionResult> CreateAreaofImprovement([FromBody] ReflectionDataDevelopment reflectionDevelopment)
        {
            try
            {
                var resAchieve = await codeUnitWebService.HRWS().CreateAppraisalAreaofImprovementAPIAsync(reflectionDevelopment.HeaderNo, reflectionDevelopment.AreaOfDevelopment);
                
                 return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Create Area of Improvement, Success",ExtMessage= resAchieve.return_value });
                
              
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Create Area of Improvement failed: " + x.Message });

            }
        }

        //Modify Area of Improvement
        [Authorize]
        [HttpPost]
        [Route("modifyareaofimprovement")]
        public async Task<IActionResult> ModifyAreaofImprovement([FromBody] ReflectionDataDevelopment reflectionDevelopment)
        {
            try
            {
                var resAchieve = await codeUnitWebService.HRWS().ModifyAppraisalAreaofImprovementAsync(reflectionDevelopment.HeaderNo, Int16.Parse(reflectionDevelopment.LineNo), reflectionDevelopment.AreaOfDevelopment);
                if (resAchieve.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Modiy Area of Improvement, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify Area of Improvement failed" });

                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify Area of Improvement failed: " + x.Message });

            }
        }

        //Delete Area of Improvement
        [Authorize]
        [HttpPost]
        [Route("deleteareaofimprovement")]
        public async Task<IActionResult> DeleteAreaofImprovement([FromBody] ReflectionDataDevelopment reflectionDevelopment)
        {
            try
            {
                var resAchieve = await codeUnitWebService.HRWS().DeleteAppraisalAreaofImprovementAsync(reflectionDevelopment.HeaderNo, Int16.Parse(reflectionDevelopment.LineNo));
                if (resAchieve.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Delete Area of Improvement, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Delete Area of Improvement failed" });
                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Delete Area of Improvement failed: " + x.Message });

            }
        }




        //Create Training Need
        [Authorize]
        [HttpPost]
        [Route("createtrainingneed")]
        public async Task<IActionResult> CreateTrainingNeed([FromBody] ReflectionDataFocus dataFocus)
        {
            try
            {
                var resAchieve = await codeUnitWebService.HRWS().CreateAppraisalTrainingNeedAPIAsync(dataFocus.HeaderNo, dataFocus.SpecificFocusArea);
               
               return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Create Training Need, Success",ExtMessage= resAchieve.return_value });
         
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Create Training Need failed: " + x.Message });

            }
        }

        //Modify Specific Focus 
        [Authorize]
        [HttpPost]
        [Route("modifytrainingneed")]
        public async Task<IActionResult> ModifyTrainingNeed([FromBody] ReflectionDataFocus dataFocus)
        {
            try
            {
                var resAchieve = await codeUnitWebService.HRWS().ModifyAppraisalTrainingNeedAsync(dataFocus.HeaderNo, Int16.Parse(dataFocus.LineNo), dataFocus.SpecificFocusArea);
                if (resAchieve.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Modiy Training Need, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify Training Need failed" });

                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify Training Need failed: " + x.Message });

            }
        }

        //Delete Specific Focus 
        [Authorize]
        [HttpPost]
        [Route("deletetrainingneed")]
        public async Task<IActionResult> DeleteTrainingNeed([FromBody] ReflectionDataFocus dataFocus)
        {
            try
            {
                var resAchieve = await codeUnitWebService.HRWS().DeleteAppraisalTrainingNeedAsync(dataFocus.HeaderNo, Int16.Parse(dataFocus.LineNo));
                if (resAchieve.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Delete Training Need, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Delete Training Need failed" });
                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Delete Training Need failed: " + x.Message });

            }
        }


        //Calculate Weights Supervisor
        [Authorize]
        [HttpGet]
        [Route("supervisorcalculateweight/{HeaderNo}")]
        public async Task<IActionResult> SupervorCalculateWeight(string HeaderNo)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var calcRes = await codeUnitWebService.HRWS().CalculateSupervisorAppraisalScoreAsync(user.EmployeeId, HeaderNo);
                if (calcRes.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Calculate Weight, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Calculate Weight failed" });

                }

            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Calculate Weightage failed: " + x.Message });

            }
        }

        //Supervisor Approve
        [Authorize]
        [HttpGet]
        [Route("superviserapprove/{HeaderNo}")]
        public async Task<IActionResult> SuperviserApprove(string HeaderNo)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var subRes = await codeUnitWebService.HRWS().SubmitSupervisorAppraisalScoreAsync(user.EmployeeId, HeaderNo);
                if (subRes.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Supervisor Approve, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Supervisor Approve Failed" });

                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Supervisor Approve Failed: " + x.Message });

            }
        }



       /*
       * **********************************************************************************************************************
       * 
       *          Supervisor Appraisal Moderated
       * 
       * **********************************************************************************************************************
       */

        //Get Supervisor Moderated Appraisal Evaluation  List
        [Authorize]
        [HttpGet]
        [Route("getsupervisormoderated")]
        public async Task<IActionResult> GetSuperviorModerated()
        {
            try
            {
                List<SupervisorAppraisalModeratedList> supervisorAppraisalModeratedLists = new List<SupervisorAppraisalModeratedList>();
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var modRes = await codeUnitWebService.HRWS().GetModeratedPerformanceAppraisalSupervisorNoAsync(user.EmployeeId);
                dynamic modResSerial = JsonConvert.DeserializeObject(modRes.return_value);
                foreach (var item in modResSerial.EmployeeAppraisals)
                {
                    SupervisorAppraisalModeratedList saml = new SupervisorAppraisalModeratedList
                    {
                        No = item.No,
                        EmployeeNo = item.EmployeeNo,
                        KPICode = item.KPIcode,
                        EmployeeName = item.EmployeeName,
                        EmployeeDesgnation = item.EmployeeDesgnation,
                        JobTitle = item.JobTitle,
                        ManagerNo = item.ManagerNo,
                        ManagerName = item.ManagerName,
                        ManagerDesignation = item.ManagerDesignation,
                        AppraisalPeriod = item.AppraisalPeriod,
                        AppraisalStartPeriod = item.AppraisalStartPeriod,
                        AppraisalEndPeriod = item.AppraisalEndPeriod,
                        AppraisalLevel = item.AppraisalLevel,
                        EmployeeWeightedScore = item.EmployeeWeightedScore,
                        SupervisorWeightedScore = item.SupervisorWeightedScore,
                        OverallWeightedScore = item.OverallWeightedScore,
                    };
                    supervisorAppraisalModeratedLists.Add(saml);
                }
                return Ok(new { supervisorAppraisalModeratedLists });
            }

            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Supervisor Moderated Failed: " + x.Message });

            }
        }



        //Get Supervisor Moderated Appraisal Performance Standard List
        // Filtere the KPI stds

        //[Authorize]
        //[HttpGet]
        //[Route("getsupervisormoderatedappraisalperformancestdperkpi/{KPICode}/{HeaderNo}")]
        //public async Task<IActionResult> GetSupervisorModeratedAppraisalPerformancestdPerKPI(int KPICode, string HeaderNo)
        //{
        //    try
        //    {
        //        List<SupervisorAppraisalStandard> supervisorAppraisalStandards = new List<SupervisorAppraisalStandard>();
        //        var resL = await codeUnitWebService.HRWS().GetSupervisorPerformanceAppraisalIndicatorsPerKPIAsync(KPICode, HeaderNo);
        //        dynamic esSer = JsonConvert.DeserializeObject(resL.return_value);

        //        foreach (var item in esSer)
        //        {
        //            SupervisorAppraisalStandard eas = new SupervisorAppraisalStandard
        //            {
        //                CriteriaCode = item.CriteriaCode,
        //                TargetCode = item.TargetCode,
        //                KPIDescription = item.KPIDescription,
        //                HeaderNo = item.HeaderNo,
        //                StandardCode = item.StandardCode,
        //                IndicatorCode = item.IndicatorCode,
        //                StandardDescription = item.StandardDescription,
        //                StandardWeighting = item.StandardWeighting,
        //                Timelines = item.Timelines,
        //                ActivityDescription = item.ActivityDescription,
        //                TargetedScore = item.TargetedScore,
        //                AchievedScoreEmployee = item.AchievedScoreEmployee,
        //                OverallAchievedScore = item.OverallAchievedScore,
        //                AchievedScoreSupervisor = item.AchievedScoreSupervisor,
        //                EmployeeComments = item.EmployeeComments,
        //                SupervisorComments = item.SupervisorComments,
        //            };
        //            supervisorAppraisalStandards.Add(eas);
        //        }
        //        return Ok(new { supervisorAppraisalStandards });
        //    }
        //    catch (Exception x)
        //    {

        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Supervisor Appraisal Perfomance Std failed: " + x.Message });

        //    }
        //}

        /*
       * 
       **/
        //Update on Supervisor Moderated Employee Appraisal
        [Authorize]
        [HttpPost]
        [Route("updatesupervisormoderatedappraisal")]
        public async Task<IActionResult> UpdateSupervisorModeratedEmployeeAppraisal([FromBody] ModifyEmployeeAppraisal modifyEmployee)
        {
            try
            {
                var updareRes = await codeUnitWebService.HRWS().ModifyModeratedPerformanceAppraisalIndicatorAsync(modifyEmployee.TargetCode, modifyEmployee.IndicatorCode, modifyEmployee.KPICode, modifyEmployee.HeaderNo, modifyEmployee.AchievedScore, modifyEmployee.EmployeeComments);
                if (updareRes.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Modify Employee Appraisal Target, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modify Employee Appraisal Target failed" });

                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Update Employee Appraisal failed: " + x.Message });

            }
        }




        //Calculate Weights Supervisor
        [Authorize]
        [HttpGet]
        [Route("moderatedsupervisorcalculateweight/{HeaderNo}")]
        public async Task<IActionResult> ModeratedSupervorCalculateWeight(string HeaderNo)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var calcRes = await codeUnitWebService.HRWS().CalculateEmployeeSupervisorAppraisalScoreAsync(user.EmployeeId, HeaderNo);
                if (calcRes.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Calculate Weight, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Calculate Weight failed" });

                }

            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Calculate Weightage failed: " + x.Message });

            }
        }

        //Supervisor Approve
        [Authorize]
        [HttpGet]
        [Route("moderatedsuperviserapprove/{HeaderNo}")]
        public async Task<IActionResult> ModeratedSuperviserApprove(string HeaderNo)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var subRes = await codeUnitWebService.HRWS().SubmitEmployeeSupervisorAppraisalScoreAsync(user.EmployeeId, HeaderNo);
                if (subRes.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Supervisor Approve, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Supervisor Approve Failed" });

                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Supervisor Approve Failed: " + x.Message });

            }
        }


        //Supervisor Return to Employee
        [Authorize]
        [HttpGet]
        [Route("moderatedsuperviserreturn/{HeaderNo}")]
        public async Task<IActionResult> ModeratedSuperviserReturn(string HeaderNo)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var subRes = await codeUnitWebService.HRWS().ReturnAppraisalScoreToEmployeeAsync(user.EmployeeId, HeaderNo);
                if (subRes.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Supervisor Return, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Supervisor Return Failed" });

                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Supervisor Return Failed: " + x.Message });

            }
        }


       /*
       * **********************************************************************************************************************
       * 
       *          Employee Appraisal Moderated
       * 
       * **********************************************************************************************************************
       */

        //Get Supervisor Moderated Appraisal Evaluation  List
        [Authorize]
        [HttpGet]
        [Route("getemployeemoderated")]
        public async Task<IActionResult> GetEmployeeModerated()
        {
            try
            {
                List<SupervisorAppraisalModeratedList> employeeAppraisalModeratedLists = new List<SupervisorAppraisalModeratedList>();
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var modRes = await codeUnitWebService.HRWS().GetModeratedPerformanceAppraisalEmployeeNoAsync(user.EmployeeId);
                dynamic modResSerial = JsonConvert.DeserializeObject(modRes.return_value);
                foreach (var item in modResSerial.EmployeeAppraisals)
                {
                    SupervisorAppraisalModeratedList saml = new SupervisorAppraisalModeratedList
                    {
                        No = item.No,
                        EmployeeNo = item.EmployeeNo,
                        KPICode = item.KPIcode,
                        EmployeeName = item.EmployeeName,
                        EmployeeDesgnation = item.EmployeeDesgnation,
                        JobTitle = item.JobTitle,
                        ManagerNo = item.ManagerNo,
                        ManagerName = item.ManagerName,
                        ManagerDesignation = item.ManagerDesignation,
                        AppraisalPeriod = item.AppraisalPeriod,
                        AppraisalStartPeriod = item.AppraisalStartPeriod,
                        AppraisalEndPeriod = item.AppraisalEndPeriod,
                        AppraisalLevel = item.AppraisalLevel,
                        EmployeeWeightedScore = item.EmployeeWeightedScore,
                        SupervisorWeightedScore = item.SupervisorWeightedScore,
                        OverallWeightedScore = item.OverallWeightedScore,
                    };
                    employeeAppraisalModeratedLists.Add(saml);
                }
                return Ok(new { employeeAppraisalModeratedLists });
            }

            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee Moderated Failed: " + x.Message });

            }
        }




        /*
        * **********************************************************************************************************************
        * 
        *          Supervisor/Employee Appraisal Completed
        * 
        * **********************************************************************************************************************
        */

        //Get Supervisor Completed Appraisal Evaluation  List
        [Authorize]
        [HttpGet]
        [Route("getsupervisorcompleted")]
        public async Task<IActionResult> GetSupervisorCompleted()
        {
            try
            {
                List<SupervisorAppraisalModeratedList> employeeAppraisalModeratedLists = new List<SupervisorAppraisalModeratedList>();
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var modRes = await codeUnitWebService.HRWS().GetCompleteSupervisorPerformanceAppraisalSupervisorNoAsync(user.EmployeeId);
                dynamic modResSerial = JsonConvert.DeserializeObject(modRes.return_value);
                foreach (var item in modResSerial.EmployeeAppraisals)
                {
                    SupervisorAppraisalModeratedList saml = new SupervisorAppraisalModeratedList
                    {
                        No = item.No,
                        EmployeeNo = item.EmployeeNo,
                        KPICode = item.KPIcode,
                        EmployeeName = item.EmployeeName,
                        EmployeeDesgnation = item.EmployeeDesgnation,
                        JobTitle = item.JobTitle,
                        ManagerNo = item.ManagerNo,
                        ManagerName = item.ManagerName,
                        ManagerDesignation = item.ManagerDesignation,
                        AppraisalPeriod = item.AppraisalPeriod,
                        AppraisalStartPeriod = item.AppraisalStartPeriod,
                        AppraisalEndPeriod = item.AppraisalEndPeriod,
                        AppraisalLevel = item.AppraisalLevel,
                        EmployeeWeightedScore = item.EmployeeWeightedScore,
                        SupervisorWeightedScore = item.SupervisorWeightedScore,
                        OverallWeightedScore = item.OverallWeightedScore,
                    };
                    employeeAppraisalModeratedLists.Add(saml);
                }
                return Ok(new { employeeAppraisalModeratedLists });
            }

            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee Completed Failed: " + x.Message });

            }
        }


        //Get Employee Completed Appraisal Evaluation  List
        [Authorize]
        [HttpGet]
        [Route("getemployeecompleted")]
        public async Task<IActionResult> GetEmployeeCompleted()
        {
            try
            {
                List<SupervisorAppraisalModeratedList> employeeAppraisalModeratedLists = new List<SupervisorAppraisalModeratedList>();
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var modRes = await codeUnitWebService.HRWS().GetCompletePerformanceAppraisalByEmployeeNoAsync(user.EmployeeId);
                dynamic modResSerial = JsonConvert.DeserializeObject(modRes.return_value);
                foreach (var item in modResSerial.EmployeeAppraisals)
                {
                    SupervisorAppraisalModeratedList saml = new SupervisorAppraisalModeratedList
                    {
                        No = item.No,
                        EmployeeNo = item.EmployeeNo,
                        KPICode = item.KPIcode,
                        EmployeeName = item.EmployeeName,
                        EmployeeDesgnation = item.EmployeeDesgnation,
                        JobTitle = item.JobTitle,
                        ManagerNo = item.ManagerNo,
                        ManagerName = item.ManagerName,
                        ManagerDesignation = item.ManagerDesignation,
                        AppraisalPeriod = item.AppraisalPeriod,
                        AppraisalStartPeriod = item.AppraisalStartPeriod,
                        AppraisalEndPeriod = item.AppraisalEndPeriod,
                        AppraisalLevel = item.AppraisalLevel,
                        EmployeeWeightedScore = item.EmployeeWeightedScore,
                        SupervisorWeightedScore = item.SupervisorWeightedScore,
                        OverallWeightedScore = item.OverallWeightedScore,
                    };
                    employeeAppraisalModeratedLists.Add(saml);
                }
                return Ok(new { employeeAppraisalModeratedLists });
            }

            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee Completed Failed: " + x.Message });

            }
        }

        //View Employee Appraisal Report
        [Route("viewemployeereport/{HNO}")]
        [HttpGet]
        public async Task<IActionResult> ViewEmployeeRepor(string HNO)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;

                var file = await codeUnitWebService.HRWS().GenerateEmployeeAppraisalReportAsync(user.EmployeeId, HNO);

                var stream = new FileStream(file.return_value, FileMode.Open);
                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Employee Report View Success");
                return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Action:Employee Report View Failed,Message:{x.Message}");

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "Report View failed" + x.Message });
            }
        }

        //View Supervisor Apprasal Report
        [Route("viewsupervisorreport/{HNO}")]
        [HttpGet]
        public async Task<IActionResult> ViewSupervisorRepor(string HNO)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;

                var file = await codeUnitWebService.HRWS().GenerateSupervisorAppraisalReportAsync(user.EmployeeId, HNO);

                var stream = new FileStream(file.return_value, FileMode.Open);

                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Supervisor Report View Success");
                return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception x)
            {

                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Action:Supervisor Report View Failed,Message:{x.Message}");

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "Report View failed" + x.Message });
            }
        }



        //View Completed Report
        [Route("viewreport/{HNO}")]
        [HttpGet]
        public async Task<IActionResult> ViewRepor(string HNO)
        {
            try
            {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                var file = await codeUnitWebService.HRWS().GenerateEmployeeSupervisorAppraisalReportAsync(user.EmployeeId, HNO);
            
                var stream = new FileStream(file.return_value, FileMode.Open);
                return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "Report View failed" + x.Message });
            }
        }





    }

}
