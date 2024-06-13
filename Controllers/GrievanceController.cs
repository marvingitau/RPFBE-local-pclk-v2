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
using RPFBE.Model.DBEntity;
using RPFBE.Model.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GrievanceController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<HomeController> logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ICodeUnitWebService codeUnitWebService;
        private readonly IMailService mailService;
        private readonly IOptions<WebserviceCreds> config;



        public GrievanceController(
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
        [HttpGet]
        [Route("designer")]
        public IActionResult Index()
        {
            return Ok("https://github.com/marvingitau");
        }

        [Authorize]
        [Route("getsystemdimensions")]
        [HttpGet]
        public async Task<IActionResult> GetSystemDim()
        {
            try { 


                List<DimensionModel> dimensionList = new List<DimensionModel>();
                var res = await codeUnitWebService.Client().GetSystemDimensionsAsync();
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);

                foreach (var item in resSerial)
                {
                    DimensionModel dm = new DimensionModel
                    {
                        Value = item.Value,
                        Label = item.Label,
                        Dimensionno = item.Dimensionno,
                        Dimensioncode = item.Dimensioncode
                    };
                    dimensionList.Add(dm);

                }

                //station;section;department
                var stationList = dimensionList.Where(x => x.Dimensioncode == config.Value.Station).ToList();
                var sectionList = dimensionList.Where(x => x.Dimensioncode == config.Value.Section).ToList();
                var departmentList = dimensionList.Where(x => x.Dimensioncode == config.Value.Department).ToList();


                List<EmployeeListModel> employeeList = new List<EmployeeListModel>();

                var resEmp = await codeUnitWebService.Client().EmployeeListAsync();
                dynamic resEmpSerial = JsonConvert.DeserializeObject(resEmp.return_value);

                foreach (var emp in resEmpSerial)
                {
                    EmployeeListModel e = new EmployeeListModel
                    {
                        Value = emp.No,
                        Label = emp.Fullname,
                    };
                    employeeList.Add(e);

                }


                return Ok(new { stationList, sectionList, departmentList, employeeList });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Dimension get failed: " + x.Message });
            }
        }
     
        //Grieavance List
        [Authorize]
        [HttpGet]
        [Route("staffgrievancelist")]
        public IActionResult StaffGrievanceList()
        {
            try
            {
                var grievancelist = dbContext.GrievanceList.Where(x => x.GID != "").Select(x=>new { x.Supervisorname,x.Employeename,x.GID,x.Employeeno,x.Supervisor,x.Currentstage,x.Nextstage,x.Resolved}).ToList();
                return Ok(new { grievancelist });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance List Failed: " + x.Message });
            }
        }
        //Get Grievance Single Record
        [Authorize]
        [HttpGet]
        [Route("singlegrievance/{GID}")]
        public async Task<IActionResult> SingleGrievance(string GID)
        {
            try
            {
                var grievancesingle = dbContext.GrievanceList.Where(x => x.GID == GID).First();
                //Get the Rank Remarks.
                List<GrievanceRanksRemark> grievanceRanksRemarks = new List<GrievanceRanksRemark>();
                var rankRemaks = await codeUnitWebService.Client().GrievanceRankRemarksAsync(GID);
                dynamic rankRemarkSerial = JsonConvert.DeserializeObject(rankRemaks.return_value);
                foreach (var item in rankRemarkSerial)
                {
                    GrievanceRanksRemark grr = new GrievanceRanksRemark
                    {
                        HRrem = item.HRremark,
                        HRref = item.HRreference,

                        HODrem = item.HODremark,
                        HODref = item.HODreference,

                        HOSref = item.HOSreference,
                        HOSrem = item.HOSremark,

                        MDrem = item.MDremark,
                        MDref = item.MDreference,

                        Suprem = item.HODremark,
                        Supref = item.HODreference,

                        HeadHRref = item.HOSreference,
                        HeadHRrem = item.HOSremark,
                    };
                    grievanceRanksRemarks.Add(grr);
                }

                List<EmployeeListModel> employeeList = new List<EmployeeListModel>();

                var resEmp = await codeUnitWebService.Client().EmployeeListAsync();
                dynamic resEmpSerial = JsonConvert.DeserializeObject(resEmp.return_value);

                foreach (var emp in resEmpSerial)
                {
                    EmployeeListModel e = new EmployeeListModel
                    {
                        Value = emp.No,
                        Label = emp.Fullname,
                    };
                    employeeList.Add(e);

                }

                return Ok(new { grievancesingle,grievanceRanksRemarks, employeeList });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance Single Failed: " + x.Message });
            }
        }


        //Modify/create the Rank value
        [Authorize]
        [HttpPost]
        [Route("modifyrankremarks/{GID}")]
        public async Task<IActionResult> ModifyRankRemark([FromBody] GrievanceRanksRemark ranksRemark,string GID)
        {
            try
            {
                var modifyRes = await codeUnitWebService.Client().GrievanceModifyRankRemarksAsync(GID, ranksRemark.HRrem, ranksRemark.HRref, ranksRemark.HOSrem,
                    ranksRemark.HOSref, ranksRemark.HODrem, ranksRemark.HODref, ranksRemark.MDrem, ranksRemark.MDref);
                if (modifyRes.return_value == "true")
                {
                    return StatusCode(StatusCodes.Status200OK, new { modifyRes.return_value });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance Modify Single Rank Remark Failed: D365 "});
                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance Modify Single Rank Remark Failed: " + x.Message });
            }
        }

        //Forward the grievance
        [Authorize]
        [HttpGet]
        [Route("forwardgrievance/{GID}")]

        public async Task<IActionResult> ForwardGrievance(string GID)
        {
            try
            {
                var res =await codeUnitWebService.Client().GrievanceForwardAsync(GID);
                dynamic resSeria = JsonConvert.DeserializeObject(res.return_value);

                var grievanceModel = dbContext.GrievanceList.Where(x => x.GID == GID).First();
                if (res.return_value == "false")
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance Forward Failed D365 " });
                }
                else
                {
                    foreach (var item in resSeria)
                    {


                        grievanceModel.Currentstage = item.Currentstage;
                        grievanceModel.Nextstage = item.Nextstage;

                        dbContext.GrievanceList.Update(grievanceModel);
                        await dbContext.SaveChangesAsync();

                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Forward Grievance Success " });
                    }
                }

                return StatusCode(StatusCodes.Status200OK, new { res.return_value });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Forward Grievance Failed: " + x.Message });
            }
        }

        //Resolve
        [Authorize]
        [HttpGet]
        [Route("resolvegrievance/{GID}")]
        public async Task<IActionResult> ResolveGrievance(string GID)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var res = await codeUnitWebService.Client().GrievanceResolveAsync(GID,user.EmployeeId);
                if (res.return_value=="false")
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance Resolve Failed D365 " });
                }
                else
                {
                    var grievanceRecord = dbContext.GrievanceList.Where(x => x.GID == GID).First();
                    grievanceRecord.Resolver = user.Name;
                    grievanceRecord.ResolverID = user.EmployeeId;
                    grievanceRecord.Resolved = true;

                    dbContext.GrievanceList.Update(grievanceRecord);
                    await dbContext.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, new { res.return_value });
                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Resolve Grievance Failed: " + x.Message });
            }
        }

        //Staff create grievance and upload details   Prog 1 cycle 1
        [Authorize]
        [HttpPost]
        [Route("creategrievance")]
        public async Task<IActionResult> CreateGrievance([FromBody] GrievanceCard grievanceCard)
        {
            try
            {

                string[] textArr = new string[20];
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                textArr[0] = grievanceCard.EmpID;
                textArr[1] = grievanceCard.Station;
                textArr[2] = grievanceCard.Section;
                textArr[3] = grievanceCard.Dept;
                textArr[4] = grievanceCard.NextStageStaff;
                textArr[5] = grievanceCard.CurrentStage;
                textArr[6] = grievanceCard.NextStage;
                textArr[7] = grievanceCard.GrievanceType;

                textArr[8] = grievanceCard.Subject;
                textArr[9] = grievanceCard.Description;
                textArr[10] = grievanceCard.StepTaken;
                textArr[11] = grievanceCard.Outcome;
                textArr[12] = grievanceCard.Comment;
                textArr[13] = grievanceCard.Recommendation;


                var res = await codeUnitWebService.Client().CreateGrievanceAsync(textArr, grievanceCard.GrievanceDate, grievanceCard.DateofIssue, grievanceCard.WorkEnv, grievanceCard.EmployeeRln);

                dynamic resSeria = JsonConvert.DeserializeObject(res.return_value);


                if (res.return_value == "false")
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance Initialization Failed D365 " });
                }
                else
                {
                    foreach (var item in resSeria)
                    {
                        GrievanceList grievanceList = new GrievanceList
                        {
                            GID = item.GID,
                            Employeeno = grievanceCard.EmpID,
                            Supervisor = item.Supervisorid,
                            Currentstage = grievanceCard.CurrentStage,
                            Nextstage = grievanceCard.NextStage,

                            Employeename = item.Employee,
                            Supervisorname = item.Supervisor,
                            GrievanceType = grievanceCard.GrievanceType,
                            Subject = grievanceCard.Subject,
                            Description = grievanceCard.Description,
                            StepTaken = grievanceCard.StepTaken,
                            Outcome = grievanceCard.Outcome,
                            Comment = grievanceCard.Comment,
                            Recommendation = grievanceCard.Recommendation,
                            Resolved = false,

                            CycleNo = 1,
                            ProgressNo = 1,
                            NextStageStaff = grievanceCard.NextStageStaff,

                            StepOneEmp = user.EmployeeId,
                            StepOneRank = user.Rank,

                            Action = Auth.Action.CREATED,
                            Actionuser = user.EmployeeId,
                            Actiondetails = Auth.Action.CREATED + " Record",
                        };

                        dbContext.GrievanceList.Add(grievanceList);
                        await dbContext.SaveChangesAsync();
                        return StatusCode(StatusCodes.Status200OK, new { grievanceList.GID });
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance Initialization Foreachloop Failed  " });

                }


                //dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);
                //foreach (var item in resSerial)
                //{
                //    GrievanceList grievanceList = new GrievanceList
                //    {
                //        GID = item.GID,
                //        Employeeno = item.Employeeno,
                //        Supervisor = item.Supervisor,
                //        Currentstage = item.Currentstage,
                //        Nextstage = item.Nextstage
                //    };

                //    dbContext.GrievanceList.Add(grievanceList);
                //    await dbContext.SaveChangesAsync();
                //}

            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance Creation Failed: " + x.Message });
            }
        }


        //Upload the Recommendations  Prog 2 cycle 1
        [Authorize]
        [HttpPost]
        [Route("uploadprogressonecycleone")]
        public async Task<IActionResult> UploadFromProgressOneCycleOne([FromBody] GrievanceCard grievanceCard)
        {
            try
            {

                string[] textArr = new string[20];
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var rec = dbContext.GrievanceList.Where(x => x.GID == grievanceCard.GID).FirstOrDefault();


                textArr[0] = grievanceCard.StepTaken;
                textArr[1] = grievanceCard.Outcome;
                textArr[2] = grievanceCard.Comment;
                textArr[3] = grievanceCard.Recommendation;
                textArr[4] = grievanceCard.GeneralRemark; //depend on rank
                textArr[5] = user.Rank;
                textArr[6] = user.EmployeeId;
                textArr[7] = "Employee";


                var res = await codeUnitWebService.Client().UpdateGrievanceAsync(textArr, grievanceCard.GID);

                rec.Currentstage = rec.Nextstage;
                rec.StepTaken = grievanceCard.StepTaken;
                rec.Outcome = grievanceCard.Outcome;
                rec.Comment = grievanceCard.Comment;
                rec.Recommendation = grievanceCard.Recommendation;
                rec.CycleNo = 1;
                rec.ProgressNo = 2;
                rec.Nextstage = "Employee";
                rec.NextStageStaff = grievanceCard.NextStageStaff;

                rec.StepTwoEmp = user.EmployeeId;
                rec.StepTwoRank = user.Rank;

                rec.Action = Auth.Action.UPDATED;
                rec.Actionuser = user.EmployeeId;
                rec.Actiondetails = Auth.Action.UPDATED + " Record";

                dbContext.GrievanceList.Update(rec);
                await dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Grievance Updated/"+res.return_value });
                  

                


              

            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance Creation Failed: " + x.Message });
            }
        }

        //Upload Prog 3 cycle 2
        [Authorize]
        [HttpPost]
        [Route("uploadprogressthreecycletwo")]
        public async Task<IActionResult> UploadFromProgressThreeCycleTwo([FromBody] GrievanceCard grievanceCard)
        {
            try
            {

                string[] textArr = new string[20];
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var res = await codeUnitWebService.Client().ReasonForCycleTwoGrievanceAsync(grievanceCard.GID, grievanceCard.CycletwoInitreason, grievanceCard.NextStageStaff, grievanceCard.NextStage);
                
                var rec = dbContext.GrievanceList.Where(x => x.GID == grievanceCard.GID).FirstOrDefault();
                rec.CycletwoInitreason = grievanceCard.CycletwoInitreason;
                rec.CycleNo = 2;
                rec.ProgressNo = 3;
                rec.Currentstage = rec.Nextstage;
                rec.Nextstage = grievanceCard.NextStage;
                rec.NextStageStaff = grievanceCard.NextStageStaff;

                rec.StepThreeEmp = user.EmployeeId;
                rec.StepThreeRank = user.Rank;

                rec.Action = Auth.Action.UPDATED;
                rec.Actionuser = user.EmployeeId;
                rec.Actiondetails = Auth.Action.UPDATED + " Record 2nd Cycled 3rd Prog";

                dbContext.GrievanceList.Update(rec);
                await dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Grievance Updated/" + res.return_value });


            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance Creation Failed: " + x.Message });
            }
        }

        //Upload Prog 4 cycle 2
        [Authorize]
        [HttpPost]
        [Route("uploadprogressfourcycletwo")]
        public async Task<IActionResult> UploadFromProgressForuCycleTwo([FromBody] GrievanceCard grievanceCard)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                string[] textArr = new string[20];
                textArr[0] = grievanceCard.Cycletwosteps;
                textArr[1] = grievanceCard.Cycletwooutcome;
                textArr[2] = grievanceCard.Cycletworecommendation;
                textArr[3] = grievanceCard.NextStageStaff;
                textArr[4] = grievanceCard.GeneralRemark;
                textArr[5] = user.Rank;
                textArr[6] = user.EmployeeId;
                textArr[7] = "Employee";

                var res = await codeUnitWebService.Client().UpdateGrievanceCycleTwoAsync(textArr,grievanceCard.GID);

                var rec = dbContext.GrievanceList.Where(x => x.GID == grievanceCard.GID).FirstOrDefault();
                rec.Currentstage = rec.Nextstage;
                rec.Cycletwosteps = grievanceCard.Cycletwosteps;
                rec.Cycletwooutcome = grievanceCard.Cycletwooutcome;
                rec.Cycletworecommendation = grievanceCard.Cycletworecommendation;
                rec.CycleNo = 2;
                rec.ProgressNo = 4;
                rec.Nextstage = "Employee";
                rec.NextStageStaff = grievanceCard.NextStageStaff;

                rec.StepFourEmp = user.EmployeeId;
                rec.StepFourRank = user.Rank;

                rec.Action = Auth.Action.UPDATED;
                rec.Actionuser = user.EmployeeId;
                rec.Actiondetails = Auth.Action.UPDATED + " Record 2nd Cycled 4th Prog";

                dbContext.GrievanceList.Update(rec);
                await dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Grievance Updated/" + res.return_value });



            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance Creation Failed: " + x.Message });
            }
        }

        //Upload Prog 5 cycle 3
        [Authorize]
        [HttpPost]
        [Route("uploadprogressfivecyclethree")]
        public async Task<IActionResult> UploadFromProgressFiveCycleThree([FromBody] GrievanceCard grievanceCard)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                string[] textArr = new string[20];
                textArr[0] = grievanceCard.NextStage;
                textArr[1] = grievanceCard.NextStageStaff;

               

                var res = await codeUnitWebService.Client().UpdateGrievanceAppealAsync(textArr, grievanceCard.GID);

                var rec = dbContext.GrievanceList.Where(x => x.GID == grievanceCard.GID).FirstOrDefault();
 
                rec.CycleNo = 3;
                rec.ProgressNo = 5;
                rec.Currentstage = rec.Nextstage;
                rec.Nextstage = grievanceCard.NextStage;
                rec.NextStageStaff = grievanceCard.NextStageStaff;

                rec.StepFiveEmp = user.EmployeeId;
                rec.StepFiveRank = user.Rank;

                rec.Action = Auth.Action.UPDATED;
                rec.Actionuser = user.EmployeeId;
                rec.Actiondetails = Auth.Action.UPDATED + " Record 3nd Cycled 5th Prog";

                dbContext.GrievanceList.Update(rec);
                await dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Grievance Updated/" + res.return_value });



            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance Creation Failed: " + x.Message });
            }
        }

        //Dismis Appeal => Prog 6 cycle 3
        [Authorize]
        [HttpPost]
        [Route("dismissappeal")]
        public async Task<IActionResult> DismissAppeal([FromBody] GrievanceCard grievanceCard)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                var res = await codeUnitWebService.Client().DeclineGrievaneAppealAsync(grievanceCard.GID, grievanceCard.AppealAlternativeRemark, grievanceCard.NextStageStaff, grievanceCard.NextStage,user.EmployeeId);

                var rec = dbContext.GrievanceList.Where(x => x.GID == grievanceCard.GID).FirstOrDefault();

                rec.Resolver = user.Name;
                rec.ResolverID = user.EmployeeId;
                rec.Resolved = true;
                rec.CycleNo = 3;
                rec.ProgressNo = 6;
                rec.Currentstage = rec.Nextstage;
                rec.Nextstage = grievanceCard.NextStage;
                rec.NextStageStaff = grievanceCard.NextStageStaff;
                rec.AppealAlternativeRemark = grievanceCard.AppealAlternativeRemark;

                rec.StepSixEmp = user.EmployeeId;
                rec.StepSixRank = user.Rank;

                rec.Action = Auth.Action.COMPLETED;
                rec.Actionuser = user.EmployeeId;
                rec.Actiondetails = Auth.Action.COMPLETED + " Record 3nd Cycled 6th Prog";

                dbContext.GrievanceList.Update(rec);
                await dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Grievance Dismissed" + res.return_value });



            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance Dismissed Failed: " + x.Message });
            }
        }


        //Uphold Appeal => Prog 6 cycle 3
        [Authorize]
        [HttpPost]
        [Route("upholdappeal")]
        public async Task<IActionResult> UpholdAppeal([FromBody] GrievanceCard grievanceCard)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                var res = await codeUnitWebService.Client().UpholdGrievaneAppealAsync(grievanceCard.GID, grievanceCard.AppealOutcomeRemark, grievanceCard.NextStageStaff, grievanceCard.NextStage, user.EmployeeId); ;

                var rec = dbContext.GrievanceList.Where(x => x.GID == grievanceCard.GID).FirstOrDefault();

                rec.Resolver = user.Name;
                rec.ResolverID = user.EmployeeId;
                rec.Resolved = true;
                rec.CycleNo = 3;
                rec.ProgressNo = 6;
                rec.Currentstage = rec.Nextstage;
                rec.Nextstage = grievanceCard.NextStage;
                rec.NextStageStaff = grievanceCard.NextStageStaff;
                rec.AppealOutcomeRemark = grievanceCard.AppealOutcomeRemark;

                rec.StepSixEmp = user.EmployeeId;
                rec.StepSixRank = user.Rank;

                rec.Action = Auth.Action.COMPLETED;
                rec.Actionuser = user.EmployeeId;
                rec.Actiondetails = Auth.Action.COMPLETED + " Record 3nd Cycled 6th Prog";

                dbContext.GrievanceList.Update(rec);
                await dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Grievance Appeal Success" + res.return_value });



            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance Appeal Failed: " + x.Message });
            }
        }


        //************************************** APPROVAL LEVEL ****************************************
        //First Cycle, first Status 
        [Authorize]
        [HttpGet]
        [Route("getapprovalgrievancelist")]
        public async Task<IActionResult> GetApprovalGrievanceList()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                //.Select(x => new { x.Supervisorname, x.Employeename, x.GID, x.Employeeno, x.Supervisor, x.Currentstage, x.Nextstage, x.Resolved }).ToList();
                var grievancelist = dbContext.GrievanceList.Where(x => x.NextStageStaff == user.EmployeeId && (x.ProgressNo == 1 || x.ProgressNo == 2) && x.CycleNo == 1).ToList();
                return Ok(new { grievancelist });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Grievance Approval List Failed: " + x.Message });
            }
        }

        //2 Cycle, 3 Status 
        [Authorize]
        [HttpGet]
        [Route("getctwosthreegrievancelist")]
        public async Task<IActionResult> GetCtwoSThreeGrievanceList()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                //.Select(x => new { x.Supervisorname, x.Employeename, x.GID, x.Employeeno, x.Supervisor, x.Currentstage, x.Nextstage, x.Resolved }).ToList();
                var grievancelist = dbContext.GrievanceList.Where(x => x.NextStageStaff == user.EmployeeId && (x.ProgressNo == 3 || x.ProgressNo == 4) && x.CycleNo == 2).ToList();
                return Ok(new { grievancelist });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Grievance Approval List Failed: " + x.Message });
            }
        }

        //3 Cycle, 5 Status 
        [Authorize]
        [HttpGet]
        [Route("getcthreesfivegrievancelist")]
        public async Task<IActionResult> GetCthreeSfiveGrievanceList()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                //.Select(x => new { x.Supervisorname, x.Employeename, x.GID, x.Employeeno, x.Supervisor, x.Currentstage, x.Nextstage, x.Resolved }).ToList();
                var grievancelist = dbContext.GrievanceList.Where(x => x.NextStageStaff == user.EmployeeId && (x.ProgressNo == 5 || x.ProgressNo == 6) && x.CycleNo == 3).ToList();
                return Ok(new { grievancelist });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Grievance Approval List Failed: " + x.Message });
            }
        }

        //Resolved Record
        [Authorize]
        [HttpGet]
        [Route("getresolvedgrievancelist")]
        public async Task<IActionResult> GetResolvedGrievanceList()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                //.Select(x => new { x.Supervisorname, x.Employeename, x.GID, x.Employeeno, x.Supervisor, x.Currentstage, x.Nextstage, x.Resolved }).ToList();
                var grievancelist = dbContext.GrievanceList.Where(x => x.Employeeno == user.EmployeeId && x.Resolved == true).ToList();
                return Ok(new { grievancelist });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Grievance Resolved List Failed: " + x.Message });
            }
        }

        //Reverse Grievance
        [Authorize]
        [HttpPost]
        [Route("reversegrievance")]
        public async Task<IActionResult> ReverseGrievance([FromBody] GrievanceCard grievanceCard)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                
                var rec = dbContext.GrievanceList.Where(x => x.GID == grievanceCard.GID).FirstOrDefault();

                if(grievanceCard.Stage == "2")
                {
                    rec.CycleNo = 1;
                    rec.ProgressNo = 1;
                    rec.ReverseOneReason = grievanceCard.ReverseReason;
                    rec.Nextstage = rec.StepTwoRank;
                    rec.NextStageStaff = rec.StepTwoEmp;
                }
                else
                {
                    rec.CycleNo = 2;
                    rec.ProgressNo = 3;
                    rec.ReverseThreeReason = grievanceCard.ReverseReason;
                    rec.Nextstage = rec.StepFourRank;
                    rec.NextStageStaff = rec.StepFourEmp;
                }

                rec.Action = Auth.Action.UPDATED;
                rec.Actionuser = user.EmployeeId;
                rec.Actiondetails = Auth.Action.UPDATED + " Record Reversed";

                dbContext.GrievanceList.Update(rec);
                await dbContext.SaveChangesAsync();

                var res = await codeUnitWebService.Client().ReverseGrievaneAsync(grievanceCard.GID, grievanceCard.ReverseReason, rec.NextStageStaff, rec.Nextstage, user.EmployeeId); ;

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Grievance Reversal Success/"+res.return_value });



            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Grievance Appeal Failed: " + x.Message });
            }
        }


    }
}
