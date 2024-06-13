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
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Controllers
{
    //performance management controller
    [ApiController]
    [Route("api/[controller]")]
    public class PerformanceController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<HomeController> logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ICodeUnitWebService codeUnitWebService;
        private readonly IMailService mailService;
        private readonly IOptions<WebserviceCreds> config;

        public PerformanceController(
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

        //Get Job Performance KPIs
        [Authorize]
        [HttpGet]
        [Route("getjobkpis")]
        public async Task<IActionResult> GetJobKPI()
        {
            try
            {
                List<JobKPIList> jobKPIs = new List<JobKPIList>();
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var res = await codeUnitWebService.HRWS().GetEmployeeJobPerfromanceTargetsEmployeeNoAsync(user.EmployeeId);
                dynamic resSeria = JsonConvert.DeserializeObject(res.return_value);

                foreach (var item in resSeria)
                {
                    JobKPIList jk = new JobKPIList
                    {
                        No = item.No,
                        EmployeeName = item.EmployeeName,
                        Period = item.Period,
                        EmployeeDesgnation = item.EmployeeDesgnation,
                        JobNo = item.JobNo,
                        JobTitle = item.JobTitle,
                        ManagerNo = item.ManagerNo,
                        ManagerName = item.ManagerName,
                        ManagerDesignation = item.ManagerDesignation,
                        AppraisalPeriod = item.AppraisalPeriod,
                        AppraisalStartPeriod = item.AppraisalStartPeriod,
                        AppraisalEndPeriod = item.AppraisalEndPeriod,
                        EmployeeWeightedScore = item.EmployeeWeightedScore,
                        SupervisorWeightedScore = item.SupervisorWeightedScore,
                        OverallWeightedScore = item.OverallWeightedScore,
                    };
                    jobKPIs.Add(jk);
                }
                return Ok(new { jobKPIs });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Job Performance KPIs failed: " + x.Message });

            }
        }
        //Get single KPIs List
        //Employee
        [Authorize]
        [HttpGet]
        [Route("getsinglekpi/{AppraisalNo}")]
        public async Task<IActionResult> GetSingleKPI(string AppraisalNo)
        {
            try
            {
                //No need for the JobKPIList since will use the react-router-dom redirect from card.
                List<PerformanceIndicators> performanceIndicators = new List<PerformanceIndicators>();
                var resInd = await codeUnitWebService.HRWS().GetEmployeeKPICodesAsync(AppraisalNo);
                dynamic resIndSerial = JsonConvert.DeserializeObject(resInd.return_value);

                foreach (var itm in resIndSerial)
                {
                    PerformanceIndicators pi = new PerformanceIndicators
                    {
                        Value = itm.KPICode,
                        Label = itm.Description,
                        Weighting = itm.Weighting,
                        TargetedScore = itm.TargetedScore,
                        AchievedScoreEmployee = itm.AchievedScoreEmployee,
                        WeightedScoreEmployee = itm.WeightedScoreEmployee,
                    };
                    performanceIndicators.Add(pi);
                }


                return Ok(new { performanceIndicators });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Job Performance KPI failed: " + x.Message });

            }
        }
        
        //Get single KPIs List
        //Supervisor
        [Authorize]
        [HttpGet]
        [Route("getsupervisorsinglekpi/{AppraisalNo}")]
        public async Task<IActionResult> GetSupervisorSingleKPI(string AppraisalNo)
        {
            try
            {
                //No need for the JobKPIList since will use the react-router-dom redirect from card.
                List<PerformanceIndicatorsSupervisor> performanceIndicators = new List<PerformanceIndicatorsSupervisor>();
                var resInd = await codeUnitWebService.HRWS().GetSupervisorPerformanceAppraisalHeaderNoAsync(AppraisalNo); 
                dynamic resIndSerial = JsonConvert.DeserializeObject(resInd.return_value);

                foreach (var itm in resIndSerial.EmployeeAppraisals[0].EmployeeKPIS)
                {
                    PerformanceIndicatorsSupervisor pi = new PerformanceIndicatorsSupervisor
                    {
                        Value = itm.KPIcode,
                        Label = itm.Description,
                        Weighting = itm.Weighting,
                        TargetedScore = itm.TargetedScore,
                        AchievedScoreSupervisor = itm.AchievedScoreSupervisor,
                        WeightedScoreSupervisor = itm.WeightedResultsSupervisor,
                    };
                    performanceIndicators.Add(pi);
                }


                return Ok(new { performanceIndicators });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Job Performance KPI failed: " + x.Message });

            }
        }

        //Get single KPIs List
        //Supervisor -Moderated
        [Authorize]
        [HttpGet]
        [Route("getsupervisormoderatedsinglekpi/{AppraisalNo}")]
        public async Task<IActionResult> GetSupervisorModeratedSingleKPI(string AppraisalNo)
        {
            try
            {
                //No need for the JobKPIList since will use the react-router-dom redirect from card.
                List<PerformanceIndicatorsSupervisorModeration> performanceIndicators = new List<PerformanceIndicatorsSupervisorModeration>();
                var resInd = await codeUnitWebService.HRWS().GetModeratedPerformanceAppraisalHeaderNoAsync(AppraisalNo);
                dynamic resIndSerial = JsonConvert.DeserializeObject(resInd.return_value);

                foreach (var itm in resIndSerial.EmployeeAppraisals[0].EmployeeKPIS)
                {
                    PerformanceIndicatorsSupervisorModeration pi = new PerformanceIndicatorsSupervisorModeration
                    {
                        Value = itm.KPIcode,
                        Label = itm.Description,
                        HeaderNo = itm.HeaderNo,
                        ObjectiveWeightage = itm.ObjectiveWeightage,
                        TargetedScore = itm.TargetedScore,
                        AchievedScoreSupervisor = itm.AchievedScoreSupervisor,
                        WeightedResultsSupervisor = itm.WeightedResultsSupervisor,
                        AchievedScoreEmployee = itm.AchievedScoreEmployee,
                        WeightedResultsEmployee = itm.WeightedResultsEmployee,
                        OverallAchievedScore = itm.OverallAchievedScore,
                        OverallWeightedResults = itm.OverallWeightedResults,
                    };
                    performanceIndicators.Add(pi);
                }


                return Ok(new { performanceIndicators });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Job Performance KPI failed: " + x.Message });

            }
        }



        //Get Both  Performance Activities and Stds
        [Authorize]
        [HttpGet]
        [Route("getperformanceactivitiesstandards/{KPICode}/{KPICardNo}")]
        public async Task<IActionResult>GetPerformanceActivitiesStandards(int KPICode,string KPICardNo)
        {
            try
            {
                List<PerformanceActivityList> performanceActivities = new List<PerformanceActivityList>();
                var res = await codeUnitWebService.HRWS().GetEmployeePerformanceActivitiesAsync(KPICode, KPICardNo);
                dynamic reSerial = JsonConvert.DeserializeObject(res.return_value);

                foreach (var item in reSerial)
                {
                    PerformanceActivityList pl = new PerformanceActivityList
                    {
                        CriteriaCode = item.CriteriaCode,
                        TargetCode = item.TargetCode,
                        HeaderNo = item.HeaderNo,
                        Activitycode = item.Activitycode,
                        ActivityDescription = item.ActivityDescription,
                        ActivityWeighting = item.ActivityWeighting,
                        Value = item.Activitycode,
                        Label = item.ActivityDescription,
                    };
                    performanceActivities.Add(pl);
                }

                List<PerformanceStandardList> performanceStandards = new List<PerformanceStandardList>();
                var resStd = await codeUnitWebService.HRWS().GetEmployeePerformanceIndicatorsAsync(KPICode, KPICardNo);
                dynamic resSerialStd = JsonConvert.DeserializeObject(resStd.return_value);

                foreach (var item2 in resSerialStd)
                {
                    PerformanceStandardList ps = new PerformanceStandardList
                    {
                        CriteriaCode = item2.CriteriaCode,
                        TargetCode = item2.TargetCode,
                        KPIDescription = item2.KPIDescription,
                        HeaderNo = item2.HeaderNo,
                        StandardCode = item2.StandardCode,
                        StandardDescription = item2.StandardDescription,
                        StandardWeighting = item2.StandardWeighting,
                        Timelines = item2.Timelines,
                        ActivityDescription = item2.ActivityDescription,
                        TargetedScore = item2.TargetedScore,
                    };
                    performanceStandards.Add(ps);
                }
              

                return Ok(new { performanceActivities, performanceStandards });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Performance Activities and  Standards failed: " + x.Message });

            }
        }
        //Get  Performance Activities
        [Authorize]
        [HttpGet]
        [Route("getperformanceactivities/{KPICode}/{KPICardNo}")]
        public async Task<IActionResult> GetPerformanceActivities(int KPICode, string KPICardNo)
        {
            try
            {
                List<PerformanceActivityList> performanceActivities = new List<PerformanceActivityList>();
                var res = await codeUnitWebService.HRWS().GetEmployeePerformanceActivitiesAsync(KPICode, KPICardNo);
                dynamic reSerial = JsonConvert.DeserializeObject(res.return_value);

                foreach (var item in reSerial)
                {
                    PerformanceActivityList pl = new PerformanceActivityList
                    {
                        CriteriaCode = item.CriteriaCode,
                        TargetCode = item.TargetCode,
                        HeaderNo = item.HeaderNo,
                        Value = item.Activitycode,
                        Label = item.ActivityDescription,
                        Activitycode = item.Activitycode, //rep with value
                        ActivityDescription = item.ActivityDescription, //rep with label
                        ActivityWeighting = item.ActivityWeighting,
                    };
                    performanceActivities.Add(pl);
                }



                return Ok(new { performanceActivities});
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Performance Activities and  Standards failed: " + x.Message });

            }
        }

        //Get Performance Standards
        [Authorize]
        [HttpGet]
        [Route("getperformancestandards/{KPICode}/{KPICardNo}")]
        public async Task<IActionResult> GetPerformanceStandards(int KPICode, string KPICardNo)
        {
            try
            {
                List<PerformanceStandardList> performanceStandards = new List<PerformanceStandardList>();
                var res = await codeUnitWebService.HRWS().GetEmployeePerformanceIndicatorsAsync(KPICode, KPICardNo);
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);

                foreach (var item in resSerial)
                {
                    PerformanceStandardList ps = new PerformanceStandardList
                    {
                        CriteriaCode = item.CriteriaCode,
                        TargetCode = item.TargetCode,
                        KPIDescription = item.KPIDescription,
                        HeaderNo = item.HeaderNo,
                        StandardCode = item.StandardCode,
                        StandardDescription = item.StandardDescription,
                        StandardWeighting = item.StandardWeighting,
                        Timelines = item.Timelines,
                        ActivityDescription = item.ActivityDescription,
                        TargetedScore = item.TargetedScore,
                    };
                    performanceStandards.Add(ps);
                }
                return Ok(new { performanceStandards });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Performance Standards failed: " + x.Message });

            }
        }

        //Create Performance Activity
        [Authorize]
        [HttpPost]
        [Route("createperformanceactivity")]
        public async Task<IActionResult> CreatePerformanceActivity([FromBody] CreateActivity createActivity)
        {
            try
            {
                var resCreate = await codeUnitWebService.HRWS().CreatePerformanceActivityAsync(createActivity.KPICode, createActivity.HeaderNo, createActivity.ActivityDescription, createActivity.Remarks);
                if (resCreate.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Create Performance Activity Success"});
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Create Performance Activity failed" });

                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Create Performance Activity failed: " + x.Message });

            }
        }

        //Delete Performance Activity
        [Authorize]
        [HttpPost]
        [Route("deleteperformanceactivity")]
        public async Task<IActionResult> DeletePerformanceActivity([FromBody] DeleteActivity deleteActivity)
        {
            try
            {
                var delRes = await codeUnitWebService.HRWS().DeletePerformanceActivityAsync(deleteActivity.TargetCode, deleteActivity.KPICode, deleteActivity.HeaderNo);
                if (delRes.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Delete Performance Activity Success" });

                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Delete Performance Activity failed" });

                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Delete Performance Activity failed: " + x.Message });

            }
        }


        //Modify Performance Activity
        [Authorize]
        [HttpPost]
        [Route("modifyperformanceactivity")]
        public async Task<IActionResult> ModifyPerformanceActivity([FromBody] CreateActivity createActivity)
        {
            try
            {
                var resCreate = await codeUnitWebService.HRWS().ModifyPerformanceActivityAsync(createActivity.TargetCode,createActivity.KPICode, createActivity.HeaderNo, createActivity.ActivityDescription, createActivity.Remarks);
                if (resCreate.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Modification of Performance Activity Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modification of Performance Activity failed" });

                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modification of Performance Activity failed: " + x.Message });

            }
        }

        /*
         * PERFORMANCE STANDARDS
         * 
         */

        //Create performance standard
        [Authorize]
        [HttpPost]
        [Route("createperformancestandard")]
        public async Task<IActionResult> CreatePerformanceStandard([FromBody] CreateStandard createStandard)
        {
            try
            {
                var resCreStd = await codeUnitWebService.HRWS().CreateEmployeePerformanceIndicatorAsync(createStandard.HeaderNo, createStandard.ActivityCode, createStandard.TargetCode, createStandard.StandardDescription, createStandard.TargetScore, createStandard.Timelines);
                if (resCreStd.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Create Performance Standard Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Create Performance Standard failed" });

                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Create Performance Standard failed: " + x.Message });

            }
        }


        /*
         * ***********************************************************************************************************************
         * 
         *          APPRAISAL TARGETS SECTION
         * 
         * **********************************************************************************************************************
         */

        //Get Employee Appraisal Target(s) incase of multiple Jobed individual
        [Authorize]
        [HttpGet]
        [Route("getemployeetargets")]
        public async Task<IActionResult> GetEmployeeAppraisalTargets()
        {
            try
            {
                List<AppraisalTargetList> employeeAppraisalList = new List<AppraisalTargetList>();
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var appRes = await codeUnitWebService.HRWS().GetEmployeePerfromanceTargetsEmployeeNoAsync(user.EmployeeId);
                dynamic appResSerial = JsonConvert.DeserializeObject(appRes.return_value);

                foreach (var item in appResSerial.EmployeeAppraisals)
                {
                    AppraisalTargetList apprs = new AppraisalTargetList
                    {
                        No = item.No,
                        EmployeeName = item.EmployeeName,
                        Period = item.Period,
                        EmployeeDesgnation = item.EmployeeDesgnation,
                        JobNo = item.JobNo,
                        JobTitle = item.JobTitle,
                        ManagerNo = item.ManagerNo,
                        ManagerName = item.ManagerName,
                        ManagerDesignation = item.ManagerDesignation,
                        AppraisalPeriod = item.AppraisalPeriod,
                        AppraisalStartPeriod = item.AppraisalStartPeriod,
                        AppraisalEndPeriod = item.AppraisalEndPeriod,
                        EmployeeWeightedScore = item.EmployeeWeightedScore,
                        SupervisorWeightedScore = item.SupervisorWeightedScore,
                        OverallWeightedScore = item.OverallWeightedScore,
                    };
                    employeeAppraisalList.Add(apprs);
                }
                return Ok(new { employeeAppraisalList });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Employee Appraisal Target failed: " + x.Message });

            }
        }

        //Approve Appraisal Target.
        [Authorize]
        [HttpGet]
        [Route("approveappraisaltarget/{AID}")]
        public async Task<IActionResult>ApproveAppraisalTarget(string AID)
        {
            try
            {
                var user =await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var apprRes = await codeUnitWebService.HRWS().SubmitEmployeeTargetsAsync(user.EmployeeId, AID);
                if (apprRes.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Approving Appraisal Target, Success" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Approving Appraisal Target failed" });

                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Approving Appraisal Target failed: " + x.Message });

            }
        }
    }
}
