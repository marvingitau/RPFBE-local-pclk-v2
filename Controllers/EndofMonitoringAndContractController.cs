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
using RPFBE.Model.EOC;
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
    public class EndofMonitoringAndContractController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<EndofMonitoringAndContractController> logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ICodeUnitWebService codeUnitWebService;
        private readonly IMailService mailService;
        private readonly IOptions<WebserviceCreds> config;

        public EndofMonitoringAndContractController(
                UserManager<ApplicationUser> userManager,
                ApplicationDbContext dbContext,
                ILogger<EndofMonitoringAndContractController> logger,
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
        //Used for probation and endof contract

        [Authorize]
        [HttpGet]
        [Route("createprobationview")]
        public async Task<IActionResult> EmployeeProbationProgress()
        {
            try
            {
                List<EmployeeListModel> employeeListModels = new List<EmployeeListModel>();
                var user =await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                string EID = user.EmployeeId;

                var resEmp = await codeUnitWebService.Client().EmployeeListAsync();
                dynamic resEmpSerial = JsonConvert.DeserializeObject(resEmp.return_value);

                foreach (var emp in resEmpSerial)
                {
                    EmployeeListModel e = new EmployeeListModel
                    {
                        Value = emp.No,
                        Label = emp.Fullname,
                    };
                    employeeListModels.Add(e);
                }
                logger.LogInformation($"User:{user.EmployeeId},Verb:View,Path:createprobationview");
                return Ok(new { employeeListModels, EID });
            }
            catch (Exception x)
            {
                logger.LogInformation($"User:NAp,Verb:View,Path:Probation View Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Create Probation View Failed: "+x.Message });
            }
           
        }

        [Authorize]
        [HttpPost]
        [Route("storeprobationcreate")]
        public async Task<IActionResult> StoreProbationCreate([FromBody] EmployeeEndofForm employeeEndofForm)
        {
            try
            {
                List<ProbationProgress> probationProgresses = new List<ProbationProgress>();
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                //progress status 2
                var res = await codeUnitWebService.Client().CreateProbationProgressGeneralAsync(employeeEndofForm.EmpID,
                    user.EmployeeId, employeeEndofForm.SupervisionTime, employeeEndofForm.ImportantSkills
                    );

                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);


                foreach (var item in resSerial)
                {
                    ProbationProgress pp = new ProbationProgress
                    {
                        UID = user.Id,
                        ProbationStatus = 0,
                        ProbationNo = item.Probationno,

                        EmpID = item.Employeeno,
                        EmpName = item.Employeename,
                        MgrID = item.Managerno,
                        MgrName = item.Managername,
                        CreationDate = item.Creationdate,
                        Department = item.Department,
                        Status = item.Status,
                        Position = item.Position,
                        ImportantSkills= employeeEndofForm.ImportantSkills,
                        SupervisionTime = employeeEndofForm.SupervisionTime,
                        HODEid = item.Hodno,
                    };
                    dbContext.ProbationProgress.Add(pp);
                    await dbContext.SaveChangesAsync();
                    logger.LogInformation($"User:{user.EmployeeId},Verb:POST,Path:Create Probation Success");
                    return Ok(true);

                }
                logger.LogWarning($"User:{user.EmployeeId},Verb:POST,Path:D365 Create Probation Failed");
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "D365 Create Probation Store Failed" });
               
            }
            catch (Exception x)
            {
                logger.LogWarning($"User:NAp,Verb:Create,Path:POST Probation Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Create Probation Store Failed: " + x.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("v1/storeprobationcreatefirstsection")]
        public async Task<IActionResult> StoreProbationCreateV1([FromBody] EmployeeEndofForm employeeEndofForm)
        {
            try
            {
                List<ProbationProgress> probationProgresses = new List<ProbationProgress>();
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;
                var res = await codeUnitWebService.Client().UpdateProbationGenSectionAsync(
                    employeeEndofForm.Probationno,
                    employeeEndofForm.SupervisionTime,
                    employeeEndofForm.ImportantSkills
                    );

                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);
                // Release automatic records
                await codeUnitWebService.Client().UpdateProbationCardProgressStatusAsync(employeeEndofForm.Probationno);


                foreach (var item in resSerial)
                {
                    ProbationProgress pp = new ProbationProgress
                    {
                        UID = user.Id,
                        ProbationStatus = 0,
                        ProbationNo = item.Probationno,
                        SupervisionTime=employeeEndofForm.SupervisionTime,
                        ImportantSkills=employeeEndofForm.ImportantSkills,
                        EmpID = item.Employeeno,
                        EmpName = item.Employeename,
                        MgrID = item.Managerno,
                        MgrName = item.Managername,
                        CreationDate = item.Creationdate,
                        Department = item.Department,
                        Status = item.Status,
                        Position = item.Position,
                        HODEid = item.Hodid,
                    };
                    var probK = dbContext.ProbationProgress.Where(x => x.ProbationNo == employeeEndofForm.Probationno).Count();
                    if(probK == 1)
                    {
                        var probModel = dbContext.ProbationProgress.Where(x => x.ProbationNo == employeeEndofForm.Probationno).First();
                        probModel.UID = user.Id;
                        probModel.ProbationStatus = 0;
                        probModel.ProbationNo = pp.ProbationNo;
                        probModel.SupervisionTime = employeeEndofForm.SupervisionTime;
                        probModel.ImportantSkills = employeeEndofForm.ImportantSkills;
                        probModel.EmpID = pp.EmpID;
                        probModel.EmpName = pp.EmpName;
                        probModel.MgrID = pp.MgrID;
                        probModel.MgrName = pp.MgrName;
                        probModel.CreationDate = pp.CreationDate;
                        probModel.Department = pp.Department;
                        probModel.Status = pp.Status;
                        probModel.Position = pp.Position;
                        probModel.HODEid = pp.HODEid;
                        dbContext.ProbationProgress.Update(probModel);
                        await dbContext.SaveChangesAsync();

                        logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Update Probation First Section Success");
                        return Ok(true);
                    }
                    else
                    {
                        dbContext.ProbationProgress.Add(pp);
                        await dbContext.SaveChangesAsync();

                        logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Update Probation First Section Success");
                        return Ok(true);
                    }
                  

                }
                logger.LogWarning($"User:{user.EmployeeId},Verb:{verb},Path:Update Probation First Section Failed");
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Update Probation First Section Failed" });

            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Action:Update Probation First Section Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Update Probation First Section Failed: " + x.Message });
            }
        }


        //Get the individual(manager) list of created probations
        [Authorize]
        [HttpGet]
        [Route("getprobationlist")]
        public async Task<IActionResult> GetProbationList()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                //var objRes = await codeUnitWebService.Client().GetProbationProgressGeneralListAsync(user.EmployeeId.ToUpper());
                //dynamic objSerial = JsonConvert.DeserializeObject(objRes.return_value);

                //List<EmployeeEndofForm> employeeEndofs = new List<EmployeeEndofForm>();

                //foreach (var item in objSerial)
                //{
                //    EmployeeEndofForm endofForm = new EmployeeEndofForm
                //    {
                //        EmpID = item.Employeeno,
                //        EmpName = item.Employeename,
                //        CreationDate = item.Creationdate,
                //        Department = item.Department,
                //        Status = item.Status,
                //        Position = item.Position,
                //        Probationno = item.Probationno
                //    };
                //    employeeEndofs.Add(endofForm);


                //}
                var employeeEndofs = dbContext.ProbationProgress.Where(x => x.MgrID == user.EmployeeId && x.ProbationStatus==0).ToList();
                logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Path:Get Probation List Success");

                return Ok(new { employeeEndofs });
            }
            catch (Exception x)
            {
                logger.LogError($"User:NAp,Verb:GET,Action:Get Probation List Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation List Failed: " + x.Message });
            }
        }

        //Get the individual(manager) list of created probations
        [Authorize]
        [HttpGet]
        [Route("v1/getprobationlist")]
        public async Task<IActionResult> GetProbationListV1()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var objRes = await codeUnitWebService.Client().GetProbationListAsync(user.EmployeeId);
                dynamic objSerial = JsonConvert.DeserializeObject(objRes.return_value);

                List<ProbationProgress> PROBs = new List<ProbationProgress>();

                if (objSerial != null)
                {
                    foreach (var item in objSerial)
                    {
                        ProbationProgress endofForm = new ProbationProgress
                        {
                            EmpID = item.EmpID,
                            EmpName = item.EmpName,
                            CreationDate = item.CreationDate,
                            Status = item.Status,
                            ProbationNo = item.ProbationNo
                        };
                        PROBs.Add(endofForm);


                    }
                }
                List<ProbationProgress> employeeEndofs = dbContext.ProbationProgress.Where(x => x.MgrID == user.EmployeeId && x.ProbationStatus == 0).ToList();
                PROBs.AddRange(employeeEndofs);
                logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Path:Get Probation List v1 Success");

                return Ok(new { PROBs });
            }
            catch (Exception x)
            {
                logger.LogError($"User:NAp,Verb:GET,Action:Get Probation List v1 Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation List Failed: " + x.Message });
            }
        }


        //Get the HOD Level probation probations List
        [Authorize]
        [HttpGet]
        [Route("getprobationlisthod")]
        public async Task<IActionResult> GetProbationListHOD()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
               
                var employeeProbs = dbContext.ProbationProgress.Where(x => x.HODEid == user.EmployeeId && x.ProbationStatus == 1).ToList();
                logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Path:Get Probation List Success");

                return Ok(new { employeeProbs });
            }
            catch (Exception x)
            {
                logger.LogError($"User:NAp,Verb:GET,Action:Get Probation List Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation List Failed: " + x.Message });
            }
        }

        // Get individual single probation
        [Authorize]
        [HttpGet]
        [Route("getprobationcard/{CardID}")]
        public async Task<IActionResult> GetProbationCard(string CardID)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                var res = await codeUnitWebService.Client().GetProbationProgressGeneralAsync(CardID);
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);
                List<EmployeeEndofForm> employeeEndofForms = new List<EmployeeEndofForm>();

                foreach (var item in resSerial)
                {
                    EmployeeEndofForm endofForm = new EmployeeEndofForm
                    {
                        Probationno = item.Probationno,
                        EmpName = item.Employeename,
                        CreationDate = item.Creationdate,
                        Department = item.Department,
                        Status = item.Status,
                        Position = item.Position,

                        Jobtitle = item.Jobtitle,
                        Branch = item.Branch,
                        Product = item.Product,
                        Employmentyear = item.Employmentyear,
                        Tenureofservice = item.Tenureofservice,
                    };

                    employeeEndofForms.Add(endofForm);
                }

                logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Path:Get Probation Card Success");

                return Ok(new { employeeEndofForms });
            }
            catch (Exception x)
            {
                logger.LogError($"User:NAp,Verb:GET,Action:Get Probation Card Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Failed: " + x.Message });
            }
        }

        //Upload Probation Section One Data
        [Authorize]
        [HttpPost]
        [Route("uploadprobationsectionone/{PID}")]
        public async Task<IActionResult> UploadProbationSectionOne([FromBody] ProbationFirstSection probationFirstSection, string PID)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                bool[] boolData = new bool[100];
                string[] commentArr = new string[20];

                commentArr[0] = probationFirstSection.PerformanceComment;
                commentArr[1] = probationFirstSection.AttendanceComment;
                commentArr[2] = probationFirstSection.AttitudeComment;
                commentArr[3] = probationFirstSection.AppearanceComment;
                commentArr[4] = probationFirstSection.InitiativeComment;
                commentArr[5] = probationFirstSection.DependabilityComment;
                commentArr[6] = probationFirstSection.JudmentComment;
                commentArr[7] = probationFirstSection.AttentionToDetailComment;
                commentArr[8] = probationFirstSection.InterpersonalComment;
                commentArr[9] = probationFirstSection.MannersComment;
                commentArr[10] = probationFirstSection.ResponsiblityComment;
                commentArr[11] = probationFirstSection.LearningCampacityComment;
                commentArr[12] = probationFirstSection.OutputComment;
                commentArr[13] = probationFirstSection.LeadershipComment;
                commentArr[14] = probationFirstSection.PressureComment;

                boolData[0] = probationFirstSection.Outstanding;
                boolData[1] = probationFirstSection.AboveAverage;
                boolData[2] = probationFirstSection.Satisfactory;
                boolData[3] = probationFirstSection.Marginal;
                boolData[4] = probationFirstSection.Unsatisfactory;

                boolData[5] = probationFirstSection.ExcellentAttendance;
                boolData[6] = probationFirstSection.OccasionalAbsence;
                boolData[7] = probationFirstSection.RepeatedAbsence;
                boolData[8] = probationFirstSection.UnjustifiedAbsence;

                boolData[9] = probationFirstSection.AlwaysInterested;
                boolData[10] = probationFirstSection.ReasonablyDevoted;
                boolData[11] = probationFirstSection.PassiveAttitude;
                boolData[12] = probationFirstSection.ActiveDislikeofWork;

                boolData[13] = probationFirstSection.AlwaysNeat;
                boolData[14] = probationFirstSection.GenerallyNeat;
                boolData[15] = probationFirstSection.SometimesCareles;
                boolData[16] = probationFirstSection.AttirenotSuitable;

                boolData[17] = probationFirstSection.SelfStarter;
                boolData[18] = probationFirstSection.NeedsStimilus;
                boolData[19] = probationFirstSection.NeedsCSupervision;
                boolData[20] = probationFirstSection.ShowNoInitiative;

                boolData[21] = probationFirstSection.AlwayOnTime;
                boolData[22] = probationFirstSection.OccasionallyLate;
                boolData[23] = probationFirstSection.RepeatedLate;
                boolData[24] = probationFirstSection.RarelyOnTime;

                boolData[25] = probationFirstSection.DecisionLogical;
                boolData[26] = probationFirstSection.GenSoundJudgment;
                boolData[27] = probationFirstSection.ReqFreqCorrection;
                boolData[28] = probationFirstSection.JudgmentOftenFaulty;

                boolData[29] = probationFirstSection.RarelyMakesErrs;
                boolData[30] = probationFirstSection.FewErrThanMost;
                boolData[31] = probationFirstSection.AvgAccuracy;
                boolData[32] = probationFirstSection.UnacceptablyErratic;

                boolData[33] = probationFirstSection.FriendlyOutgoing;
                boolData[34] = probationFirstSection.SomewhatBusinesslike;
                boolData[35] = probationFirstSection.GregariousToPoint;
                boolData[36] = probationFirstSection.SullenAndWithdrawn;

                boolData[37] = probationFirstSection.AlwayscourteousTactful;
                boolData[38] = probationFirstSection.GenCourteous;
                boolData[39] = probationFirstSection.SometimesIncosiderate;
                boolData[40] = probationFirstSection.ArouseAntagonism;

                boolData[41] = probationFirstSection.SeeksAddResponsibility;
                boolData[42] = probationFirstSection.WillinglyAcceptResp;
                boolData[43] = probationFirstSection.AssumesWhenUnavoidable;
                boolData[44] = probationFirstSection.AlwaysAvoidResponsibility;

                boolData[45] = probationFirstSection.GraspImmediately;
                boolData[46] = probationFirstSection.QuickerThanAvg;
                boolData[47] = probationFirstSection.AvgLearning;
                boolData[48] = probationFirstSection.SlowLearner;
                boolData[49] = probationFirstSection.UnableToGraspNew;

                boolData[50] = probationFirstSection.ExcepHighProductivity;
                boolData[51] = probationFirstSection.CompleteMoreThanAvg;
                boolData[52] = probationFirstSection.AdequatePerHr;
                boolData[53] = probationFirstSection.InadequateOutput;

                boolData[54] = probationFirstSection.AssumesLeadershipInit;
                boolData[55] = probationFirstSection.WillLeadEncouraged;
                boolData[56] = probationFirstSection.CanLeadifNecessary;
                boolData[57] = probationFirstSection.RefusesLeadership;
                boolData[58] = probationFirstSection.AttemptbutInefficient;

                boolData[59] = probationFirstSection.NeverFalter;
                boolData[60] = probationFirstSection.MaintainPoise;
                boolData[61] = probationFirstSection.DependableExcUnderPress;
                boolData[62] = probationFirstSection.CantTakePressure;

                var res = await codeUnitWebService.Client().UpdateProbationProgressFirstSectionAsync(PID, boolData, commentArr);

                logger.LogInformation($"User:{user.EmployeeId},Verb:POST,Path:Upload Probation Section One Success");

                return Ok(res.return_value);
            }
            catch (Exception x)
            {
                logger.LogError($"User:NAp,Verb:POST,Action:Probation Section One Upload Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Section One Upload Failed: " + x.Message });
            }
        }
        //Upload Probation Recommendation Section
        [Authorize]
        [HttpPost]
        [Route("uploadprobationrecommendation/{PID}")]
        public async Task<IActionResult> UploadProbationRecommendation([FromBody] ProbationRecommendation probationRecommendation,string PID)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                string[] textArr = new string[10];
                bool[] boolArr = new bool[5];

                textArr[0] = probationRecommendation.EmployeeStrongestPoint;
                textArr[1] = probationRecommendation.EmployeeWeakestPoint;
                textArr[2] = probationRecommendation.EmployeeQualifiedForPromo;
                textArr[3] = probationRecommendation.PromoPosition;
                textArr[4] = probationRecommendation.PromotableInTheFuture;
                textArr[5] = probationRecommendation.EffectiveDifferentAssignment;
                textArr[6] = probationRecommendation.WhichAssignment;
                textArr[7] = probationRecommendation.AdditionalComment;

                boolArr[0] = probationRecommendation.confirm;
                boolArr[1] = probationRecommendation.Extend;
                boolArr[2] = probationRecommendation.Terminate;

                var res = await codeUnitWebService.Client().UpdateProbationRecommendationSectionAsync(PID,textArr, boolArr);

                var probprog = dbContext.ProbationProgress.Where(x => x.ProbationNo == PID).First();
                probprog.UIDComment = probationRecommendation.AdditionalComment;
                dbContext.ProbationProgress.Update(probprog);
                await dbContext.SaveChangesAsync();

                logger.LogInformation($"User:{user.EmployeeId},Verb:POST,Path:Probation Recommendation Upload Success");
                return Ok(res.return_value);

            }
            catch (Exception x)
            {
                logger.LogError($"User:NAp,Verb:POST,Action:Probation Recommendation Upload Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Recommendation Upload Failed: " + x.Message });
            }
        }

        //Move Probation To HR
        [Authorize]
        [HttpGet]
        [Route("moveprobationfrommanagertohr/{PID}")]
        public async Task<IActionResult> MoveProbationFromManagerToHR(string PID)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                var probModel = dbContext.ProbationProgress.Where(p => p.ProbationNo == PID && p.ProbationStatus ==0).First();
                probModel.UID = user.Id;
                probModel.ProbationStatus = 1;

                dbContext.ProbationProgress.Update(probModel);
                await dbContext.SaveChangesAsync();

                //Mail HR
                // var emailArr = dbContext.Users.Where(x => x.Rank == "HR")
                //     .Select(t => t.Email).ToArray();

                // var unameArr = dbContext.Users.Where(x => x.Rank == "HR")
                //     .Select(t => t.UserName).ToArray();

                // List<ProbationProgressMail> v = new List<ProbationProgressMail>();
                //// v.AddRange(userList);
                //mailService.SendEmail(emailArr, unameArr, PID);
                var probationMailHR = await codeUnitWebService.WSMailer().EmployeeProbationManagerToHRAsync(PID);

                // return Ok(userList);
                logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Path:Probation Move To HR Success");
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Probation Moved: "});
            }
            catch (Exception x)
            {
                logger.LogError($"User:NAp,Verb:GET,Action:Probation Move To HR Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Move Failed: " + x.Message });
            }
        }

        //Move Probation To HOD from Immediate Manager 
        /*[Authorize]
        [HttpGet]
        [Route("moveprobationfromsupervisortohod/{PID}")]
        public async Task<IActionResult> MoveProbationFromSupervisorToHOD(string PID)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var immediateManager = await codeUnitWebService.Client().GetImmediateManagerAsync(PID);

                var probModel = dbContext.ProbationProgress.Where(p => p.ProbationNo == PID && p.ProbationStatus == 0).First();
                probModel.UID = user.Id;
                probModel.ProbationStatus = 1;
                probModel.ImmediateManagerID = immediateManager.return_value.ToString();

                dbContext.ProbationProgress.Update(probModel);
                await dbContext.SaveChangesAsync();

             

                var probationMailHR = await codeUnitWebService.WSMailer().EmployeeProbationHODToImmediateManagerAsync(PID);

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Probation Moved: " });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Move Failed: " + x.Message });
            }
        }
        */

        //Move Probation To Immediate Manager From HOD
        [Authorize]
        [HttpGet]
        [Route("moveprobationfromhodtomanger/{PID}")]
        public async Task<IActionResult> MoveProbationFromHODToManager(string PID)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;
                //var immediateManager = await codeUnitWebService.Client().GetImmediateManagerAsync(PID);
                var probModel = dbContext.ProbationProgress.Where(p => p.ProbationNo == PID && p.ProbationStatus == 0).First();
                //probModel.UID = user.Id;
                probModel.ProbationStatus = 1;

                dbContext.ProbationProgress.Update(probModel);
                await dbContext.SaveChangesAsync();

                //Mail HR
                // var emailArr = dbContext.Users.Where(x => x.Rank == "HR")
                //     .Select(t => t.Email).ToArray();

                // var unameArr = dbContext.Users.Where(x => x.Rank == "HR")
                //     .Select(t => t.UserName).ToArray();

                // List<ProbationProgressMail> v = new List<ProbationProgressMail>();
                //// v.AddRange(userList);
                ///

                //---mailService.SendEmail(emailArr, unameArr, PID);

                var probationMailHR = await codeUnitWebService.WSMailer().EmployeeProbationHODToImmediateManagerAsync(PID);

                // return Ok(userList);
                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Probation Move To ImmediateMgr From HOD Success");
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Probation Moved: " });
            }
            catch (Exception x)
            {
                var verb = Request.HttpContext.Request.Method;
                logger.LogError($"User:NAp,Verb:{verb},Action:Probation Move To ImmediateMgr From HOD Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Move Failed: " + x.Message });
            }
        }

        // Move probation from Supervisor to HOD
        [Authorize]
        [HttpGet]
        [Route("moveprobationfromsupervisortohod/{PID}")]
        public async Task<IActionResult> MoveProbationFromSupervisorToHOD(string PID)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;
                var probModel = dbContext.ProbationProgress.Where(p => p.ProbationNo == PID && p.ProbationStatus == 0).First();
                probModel.ProbationStatus = 1;

                dbContext.ProbationProgress.Update(probModel);
                await dbContext.SaveChangesAsync();

                //Mail HOD

                var probationMailHR = await codeUnitWebService.WSMailer().SupervisorToHODProbationAsync(probModel.HODEid, probModel.ProbationNo);
                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path::Probation Movement Success");
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Probation Moved" });
            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{user.EmployeeId},Verb:GET,Action:Probation Movement failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Move Failed: " + x.Message });
            }
        }


        //Probation Card Data
        [Authorize]
        [HttpGet]
        [Route("probationcarddata/{PID}")]
        public async Task<IActionResult> ProbationCardData(string PID)
        {
            try
            {
                List<ProbationFirstSection> probationFirstList = new List<ProbationFirstSection>();
                var probprogress = dbContext.ProbationProgress.Where(x => x.ProbationNo == PID).First();
                var res = await codeUnitWebService.Client().GetProbationCardDataAsync(PID);
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);
                foreach (var item in resSerial)
                {
                    ProbationFirstSection probationFirstSection = new ProbationFirstSection
                    {

                        Probationno=item.Probationno,
                        Employeeno = item.Employeeno,
                        Employeename =item.Employeename,
                        Creationdate =item.Creationdate,
                        Department =item.Department,
                        Status =item.Status,
                        Position =item.Position,
                        Managername =item.Manangername,
                        Skill = item.Skill,
                        Supervisionduration = item.Supervisionduration,


                        Outstanding = item.Outstanding == "Yes" ? true : false,
                        AboveAverage = item.Aboveaverage == "Yes" ? true : false,
                        Satisfactory = item.Satisfactory == "Yes" ? true : false,
                        Marginal = item.Marginal == "Yes" ? true : false,
                        Unsatisfactory = item.Unsatisfactory == "Yes" ? true : false,
                        PerformanceComment = "",

                        ExcellentAttendance = item.ExcellentAttendance == "Yes" ? true : false,
                        OccasionalAbsence = item.OccasionalAbsence == "Yes" ? true : false,
                        RepeatedAbsence = item.RepeatedAbsence == "Yes" ? true : false,
                        UnjustifiedAbsence = item.UnjustifiedAbsence == "Yes" ? true : false,
                        AttendanceComment = item.AttendanceComment,

                        AlwaysInterested = item.AlwaysInterested == "Yes" ? true : false,
                        ReasonablyDevoted = item.ReasonablyDevoted == "Yes" ? true : false,
                        PassiveAttitude = item.PassiveAttitude == "Yes" ? true : false,
                        ActiveDislikeofWork = item.ActiveDislikeofWork == "Yes" ? true : false,
                        AttitudeComment = item.AttitudeComment,

                        AlwaysNeat = item.AlwaysNeat == "Yes" ? true : false,
                        GenerallyNeat = item.GenerallyNeat == "Yes" ? true : false,
                        SometimesCareles = item.SometimesCareles == "Yes" ? true : false,
                        AttirenotSuitable = item.AttirenotSuitable == "Yes" ? true : false,
                        AppearanceComment = item.AppearanceComment,


                        SelfStarter = item.SelfStarter == "Yes" ? true : false,
                        NeedsStimilus = item.NeedsStimilus == "Yes" ? true : false,
                        NeedsCSupervision = item.NeedsCSupervision == "Yes" ? true : false,
                        ShowNoInitiative = item.ShowNoInitiative == "Yes" ? true : false,
                        InitiativeComment = item.InitiativeComment,

                        AlwayOnTime = item.AlwayOnTime == "Yes" ? true : false,
                        OccasionallyLate = item.OccasionallyLate == "Yes" ? true : false,
                        RepeatedLate = item.RepeatedLate == "Yes" ? true : false,
                        RarelyOnTime = item.RarelyOnTime == "Yes" ? true : false,
                        DependabilityComment = item.DependabilityComment,

                        DecisionLogical = item.DecisionLogical == "Yes" ? true : false,
                        GenSoundJudgment = item.GenSoundJudgment == "Yes" ? true : false,
                        ReqFreqCorrection = item.ReqFreqCorrection == "Yes" ? true : false,
                        JudgmentOftenFaulty = item.JudgmentOftenFaulty == "Yes" ? true : false,
                        JudmentComment = item.JudmentComment,

                        RarelyMakesErrs = item.RarelyMakesErrs == "Yes" ? true : false,
                        FewErrThanMost = item.FewErrThanMost == "Yes" ? true : false,
                        AvgAccuracy = item.AvgAccuracy == "Yes" ? true : false,
                        UnacceptablyErratic = item.UnacceptablyErratic == "Yes" ? true : false,
                        AttentionToDetailComment = item.AttentionToDetailComment,

                        FriendlyOutgoing = item.FriendlyOutgoing == "Yes" ? true : false,
                        SomewhatBusinesslike = item.SomewhatBusinesslike == "Yes" ? true : false,
                        GregariousToPoint = item.GregariousToPoint == "Yes" ? true : false,
                        SullenAndWithdrawn = item.SullenAndWithdrawn == "Yes" ? true : false,
                        InterpersonalComment = item.InterpersonalComment,

                        AlwayscourteousTactful = item.AlwayscourteousTactful == "Yes" ? true : false,
                        GenCourteous = item.GenCourteous == "Yes" ? true : false,
                        SometimesIncosiderate = item.SometimesIncosiderate == "Yes" ? true : false,
                        ArouseAntagonism = item.ArouseAntagonism == "Yes" ? true : false,
                        MannersComment = item.MannersComment,

                        SeeksAddResponsibility = item.SeeksAddResponsibility == "Yes" ? true : false,
                        WillinglyAcceptResp = item.WillinglyAcceptResp == "Yes" ? true : false,
                        AssumesWhenUnavoidable = item.AssumesWhenUnavoidable == "Yes" ? true : false,
                        AlwaysAvoidResponsibility = item.AlwaysAvoidResponsibility == "Yes" ? true : false,
                        ResponsiblityComment = item.ResponsiblityComment,

                        GraspImmediately = item.GraspImmediately == "Yes" ? true : false,
                        QuickerThanAvg = item.QuickerThanAvg == "Yes" ? true : false,
                        AvgLearning = item.AvgLearning == "Yes" ? true : false,
                        SlowLearner = item.SlowLearner == "Yes" ? true : false,
                        UnableToGraspNew = item.UnableToGraspNew == "Yes" ? true : false,
                        LearningCampacityComment = item.LearningCampacityComment,

                        ExcepHighProductivity = item.ExcepHighProductivity == "Yes" ? true : false,
                        CompleteMoreThanAvg = item.CompleteMoreThanAvg == "Yes" ? true : false,
                        AdequatePerHr = item.AdequatePerHr == "Yes" ? true : false,
                        InadequateOutput = item.InadequateOutput == "Yes" ? true : false,
                        OutputComment = item.OutputComment,

                        AssumesLeadershipInit = item.AssumesLeadershipInit == "Yes" ? true : false,
                        WillLeadEncouraged = item.WillLeadEncouraged == "Yes" ? true : false,
                        CanLeadifNecessary = item.CanLeadifNecessary == "Yes" ? true : false,
                        RefusesLeadership = item.RefusesLeadership == "Yes" ? true : false,
                        AttemptbutInefficient = item.AttemptbutInefficient == "Yes" ? true : false,
                        LeadershipComment = item.LeadershipComment,

                        NeverFalter = item.NeverFalter == "Yes" ? true : false,
                        MaintainPoise = item.MaintainPoise == "Yes" ? true : false,
                        DependableExcUnderPress = item.DependableExcUnderPress == "Yes" ? true : false,
                        CantTakePressure = item.CantTakePressure == "Yes" ? true : false,
                        PressureComment = item.PressureComment,


                        HRcomment = item.HRcomment,
                        MDcomment = item.MDcomment,
                        HODComment = probprogress!=null? probprogress.HODComment: item.MDcomment,

                        empStrongestpt = item.empStrongestpt,
                        empWeakestPt = item.empWeakestPt,
                        qualifiedPromo = item.qualifiedPromo,
                        promoPstn = item.promoPstn,
                        promotable = item.promotable,
                        effectiveWithDifferent = item.effectiveWithDifferent,
                        differentAssingment = item.differentAssingment,
                        recommendationSectionComment = item.recommendationSectionComment,

                        empRecConfirm = item.empRecConfirm == "Yes" ? "true" : "false",
                        empRecExtProb = item.empRecExtProb == "Yes" ? "true" : "false",
                        empRecTerminate = item.empRecTerminate == "Yes" ? "true" : "false",

                        Jobtitle = item.Jobtitle,
                        Branch = item.Branch,
                        Product = item.Product,
                        Employmentyear = item.Employmentyear,
                        Tenureofservice = item.Tenureofservice,
                        Probationstart = item.Probationstart,
                        Probationexpiry = item.Probationexpiry,
                    };

                    probationFirstList.Add(probationFirstSection);

                }
                return Ok(new { probationFirstList });
              
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Failed: " + x.Message });
            }
        }


        /**
         * *****************************************************************************************************************
         *                                                      IMMEDIATE MANAGER SECTION
         * 
         * ************************************************************************************************************************
         */
        //Get the Immediate Manager list
        [Authorize]
        [HttpGet]
        [Route("getimmediatemanagerprobationlist")]
        public async Task<IActionResult> GetImmediateManagerProbationList()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var employeeEndofs = dbContext.ProbationProgress.Where(x => x.ProbationStatus == 999).ToList();

                return Ok(new { employeeEndofs });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation List Failed: " + x.Message });
            }
        }

        //Push Probation from HOD to HR
        [Authorize]
        [HttpPost]
        [Route("hodpushtohr/{PID}")]
        public async Task<IActionResult> ImmediateHODPushToHR([FromBody] ProbationRecommendationModel probationFirst, string PID)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;
               
                    var probModel = dbContext.ProbationProgress.Where(x => x.ProbationNo == PID).FirstOrDefault();
                    probModel.ProbationStatus = 2;
                    //probModel.UIDTwo = user.Id;
                    probModel.HODComment = probationFirst.HODComment;

                    dbContext.ProbationProgress.Update(probModel);
                    await dbContext.SaveChangesAsync();

                ////Mail HR
                ///@email
                var probationMailHR = await codeUnitWebService.WSMailer().EmployeeProbationManagerToHRAsync(PID);

                //var emailArr = dbContext.Users.Where(x => x.Rank == "MD" || x.Rank =="FD")
                //    .Select(t => t.Email).ToArray();



                //var unameArr = dbContext.Users.Where(x => x.Rank == "MD" || x.Rank == "FD")
                //    .Select(t => t.UserName).ToArray();

                //List<ProbationProgressMail> v = new List<ProbationProgressMail>();
                //// v.AddRange(userList);
                //mailService.SendEmail(emailArr, unameArr, PID);


                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:HOD Push to HR Success");
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Probation Card Moved " });
            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Action:HR Stage Requsition Single failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Update Failed: " + x.Message });
            }
        }
        
        /**
         * *****************************************************************************************************************
         *                                                      HR SECTION
         * 
         * ************************************************************************************************************************
         */



        //Get the HR ready probation List
        [Authorize]
        [HttpGet]
        [Route("gethrprobationlist")]
        public async Task<IActionResult> GetHRProbationList()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var employeeEndofs = dbContext.ProbationProgress.Where(x=>x.ProbationStatus == 2).ToList();

                return Ok(new { employeeEndofs });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation List Failed: " + x.Message });
            }
        }
        //Get the HEAD-HR list of created probations
        [Authorize]
        [HttpGet]
        [Route("getheadhrprobationlist")]
        public async Task<IActionResult> GetHeadHRProbationList()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var employeeEndofs = dbContext.ProbationProgress.Where(x => x.ProbationStatus == 2 || x.ProbationStatus == 4).ToList();

                return Ok(new { employeeEndofs });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation List Failed: " + x.Message });
            }
        }

        //HR Push the comment
        [Authorize]
        [HttpPost]
        [Route("hrpushtomdfd/{PID}")]
        public async Task<IActionResult> HRPushToMDFD([FromBody] ProbationRecommendationModel probationFirst,string PID)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var resRemarks = await codeUnitWebService.Client().UpdateProbationHRremarkAsync(PID, probationFirst.HRcomment);
                if (bool.Parse(resRemarks.return_value))
                {
                   var probModel = dbContext.ProbationProgress.Where(x => x.ProbationNo == PID).FirstOrDefault();
                    probModel.ProbationStatus = 3;
                    probModel.UIDTwo = user.Id;
                    probModel.UIDTwoComment = probationFirst.HRcomment;

                    dbContext.ProbationProgress.Update(probModel);
                    await dbContext.SaveChangesAsync();

                    ////Mail MD/FD
                    ///@email
                    var mailsMFD = await codeUnitWebService.WSMailer().EmployeeProbationHRToMDFDAsync(PID);

                    //var emailArr = dbContext.Users.Where(x => x.Rank == "MD" || x.Rank =="FD")
                    //    .Select(t => t.Email).ToArray();



                    //var unameArr = dbContext.Users.Where(x => x.Rank == "MD" || x.Rank == "FD")
                    //    .Select(t => t.UserName).ToArray();

                    //List<ProbationProgressMail> v = new List<ProbationProgressMail>();
                    //// v.AddRange(userList);
                    //mailService.SendEmail(emailArr, unameArr, PID);

                    return Ok(bool.Parse(resRemarks.return_value));
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Update Failed: D365 failed "});
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Update Failed: " + x.Message });
            }
        }

        //HR Approves
        [Authorize]
        [HttpGet]
        [Route("hrapproveprobation/{PID}")]
        public async Task<IActionResult> HRApproveProbation(string PID)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var resRemarks = await codeUnitWebService.Client().ApproveProbationHRAsync(PID);
                if (bool.Parse(resRemarks.return_value))
                {
                    var probModel = dbContext.ProbationProgress.Where(x => x.ProbationNo == PID).First();
                    probModel.Status = "Approved";
                    probModel.ProbationStatus = 4; //approv is 4 while reject  is 5
                    dbContext.ProbationProgress.Update(probModel);
                    await dbContext.SaveChangesAsync();

                    ////Mail MD/FD
                    //@email
                    var mailsManager = await codeUnitWebService.WSMailer().EmployeeProbationHRApprovesAsync(PID);
                    //var emailArr = dbContext.Users.Where(x => x.Rank == "HR")
                    //    .Select(t => t.Email).ToArray();



                    //var unameArr = dbContext.Users.Where(x => x.Rank == "HR")
                    //    .Select(t => t.UserName).ToArray();

                    //List<ProbationProgressMail> v = new List<ProbationProgressMail>();
                    //// v.AddRange(userList);
                    //mailService.SendEmail(emailArr, unameArr, PID);

                    return Ok(bool.Parse(resRemarks.return_value));
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Update Failed: D365 failed " });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Update Failed: " + x.Message });
            }
        }




        /**
         * *****************************************************************************************************************
         *                                                      FD SECTION
         * 
         * ************************************************************************************************************************
         */

        //FD Dashboard
        [Authorize]
        [HttpGet]
        [Route("fddashboard")]
        public async Task<IActionResult> FDDashboard()
        {
            try
            {
                var usr = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var pendingCount = dbContext.ProbationProgress.Where(x => x.ProbationStatus == 2).Count();
                var doneCount = dbContext.ProbationProgress.Where(x => x.ProbationStatus > 2).Count();

                return Ok(new { pendingCount, doneCount });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "FD Failed " + x.Message });
            }
        }

        //Get the MFD ready probation List
        [Authorize]
        [HttpGet]
        [Route("getfdprobationlist")]
        public async Task<IActionResult> GetFDProbationList()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var employeeEndofs = dbContext.ProbationProgress.Where(x => x.ProbationStatus == 3).ToList();

                return Ok(new { employeeEndofs });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation List Failed: " + x.Message });
            }
        }

        //FD Approves
        [Authorize]
        [HttpPost]
        [Route("fdapproveprobation/{PID}")]
        public async Task<IActionResult> FDApproveProbation([FromBody] ProbationRecommendationModel probationRecommendationModel,string PID)
        {
            try { 
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var resRemarks = await codeUnitWebService.Client().UpdateProbationMFDremarkAsync(PID, probationRecommendationModel.MDcomment);
                if (bool.Parse(resRemarks.return_value))
                {
                    var probModel = dbContext.ProbationProgress.Where(x => x.ProbationNo == PID).First();
                    probModel.ProbationStatus = 4;
                    probModel.UIDThree = user.Id;
                    probModel.UIDThreeComment = probationRecommendationModel.MDcomment;

                    dbContext.ProbationProgress.Update(probModel);
                    await dbContext.SaveChangesAsync();

                        //Mail MD/FD
                        //@email
                        var emailHRManager = await codeUnitWebService.WSMailer().EmployeeProbationMDFDApprovesAsync(PID);
                    //var emailArr = dbContext.Users.Where(x => x.Rank == "HR")
                    //    .Select(t => t.Email).ToArray();



                    //var unameArr = dbContext.Users.Where(x => x.Rank == "HR")
                    //    .Select(t => t.UserName).ToArray();

                    //List<ProbationProgressMail> v = new List<ProbationProgressMail>();
                    //// v.AddRange(userList);
                    //mailService.SendEmail(emailArr, unameArr, PID);

                    return Ok(bool.Parse(resRemarks.return_value));
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Update Failed: D365 failed " });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Update Failed: " + x.Message });
            }
        }

        //FD Rejects
        [Authorize]
        [HttpPost]
        [Route("fdrejectprobation/{PID}")]
        public async Task<IActionResult> FDRejectProbation([FromBody] ProbationRecommendationModel probationRecommendationModel, string PID)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var resRemarks = await codeUnitWebService.Client().UpdateProbationMFDremarkAsync(PID, probationRecommendationModel.MDcomment);
                if (bool.Parse(resRemarks.return_value))
                {
                    var rejectRes = await codeUnitWebService.Client().RejectProbationMFDAsync(PID);
                    var probModel = dbContext.ProbationProgress.Where(x => x.ProbationNo == PID).First();
                    probModel.ProbationStatus = 4;
                    probModel.UIDThree = user.Id;
                    probModel.Status = "Rejected";
                    probModel.UIDThreeComment = probationRecommendationModel.MDcomment;

                    dbContext.ProbationProgress.Update(probModel);
                    await dbContext.SaveChangesAsync();

                    //Mail MD/FD
                    //@email
                    var mailManagerHR = await codeUnitWebService.WSMailer().EmployeeProbationMDFDRejectsAsync(PID);
                    //var emailArr = dbContext.Users.Where(x => x.Rank == "HR")
                    //    .Select(t => t.Email).ToArray();



                    //var unameArr = dbContext.Users.Where(x => x.Rank == "HR")
                    //    .Select(t => t.UserName).ToArray();

                    //List<ProbationProgressMail> v = new List<ProbationProgressMail>();
                    //// v.AddRange(userList);
                    //mailService.SendEmail(emailArr, unameArr, PID,false);

                    return Ok(bool.Parse(resRemarks.return_value));
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Update Failed: D365 failed " });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Update Failed: " + x.Message });
            }
        }


        //FD Rejects
        [Authorize]
        [HttpPost]
        [Route("probationreversal")]
        public async Task<IActionResult> ProbationReversal([FromBody] ProbationProgress probation)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var probModel = dbContext.ProbationProgress.Where(x => x.ProbationNo == probation.ProbationNo).First();
                probModel.ProbationStatus = probation.ProbationStatus;
                probModel.BackTrackingReason = probation.BackTrackingReason;

                dbContext.ProbationProgress.Update(probModel);
                await dbContext.SaveChangesAsync();

                //Mail Reversal Recepient
                //@email
                //@recipient
                if(probModel.ProbationStatus == 0)
                {
                    //Status 1 :: HOD
                    var rec = dbContext.Users.Where(x => x.Id == probModel.UID).First();

                    //var mailManagerHR = await codeUnitWebService.WSMailer().ProbationReversalAsync(
                    //    rec.EmployeeId,
                    //    probation.BackTrackingReason,
                    //    probation.ProbationNo
                    //    );

                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Probation Card Re-versal Success " });

                }
                else
                {
                    //Status 2 :: HR
                    var rec = dbContext.Users.Where(x => x.Id == probModel.UIDTwo).First();

                    //var mailManagerHR = await codeUnitWebService.WSMailer().ProbationReversalAsync(
                    //    rec.EmployeeId,
                    //    probation.BackTrackingReason,
                    //    probation.ProbationNo
                    //    );

                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Probation Card Re-versal Success " });

                }


            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Re-versal Failed: " + x.Message });
            }
        }

        /*******************************************************************************************************************
        *-- -------------------------------------------------                                                      
        *------------------------------------------------------END OF CONTRACT SECTION
        * ----------------------------------------------------
        * ************************************************************************************************************************
        */

        [Authorize]
        [HttpPost]
        [Route("storeendofcontractcreate")]
        public async Task<IActionResult> StoreEndofContractCreate([FromBody] EndofContractProgress  endofContract )
        {
            try
            {
                List<EndofContractProgress> endofContracts = new List<EndofContractProgress>();
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                //prgress status 2 rec
                var res = await codeUnitWebService.Client().CreateEndofContractGeneralAsync(
                    endofContract.EmpID,
                    user.EmployeeId, //Immediate Supervisor
                    endofContract.SupervisionTime, 
                    endofContract.DoRenew,
                    endofContract.RenewReason,
                    endofContract.Howlong
                    );


                // This will be moved to update of record.
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);
                foreach (var item in resSerial)
                {
                    EndofContractProgress pp = new EndofContractProgress
                    {
                        UID = user.Id,
                        ContractStatus = 0,
                        ContractNo = item.Probationno,

                        EmpID = item.Employeeno,
                        EmpName = item.Employeename,
                        MgrID = item.Managerno,
                        MgrName = item.Managername,
                        CreationDate = item.Creationdate,
                        Department = item.Department,
                        Status = item.Status,
                        Position = item.Position,
                        RenewReason = endofContract.RenewReason,
                        Howlong = endofContract.Howlong,
                        SupervisionTime = item.Supervisiontime,
                        DoRenew = item.Dorenew,
                        HODEid = item.Hodno,
                    };
                    dbContext.EndofContractProgress.Add(pp);
                    await dbContext.SaveChangesAsync();

                    return Ok(true);

                }
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "D365 Create Contract Store Failed" });

            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Create Contract Store Failed: " + x.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("geteocgeneralcard/{CardID}")]
        public async Task<IActionResult> GetEOCGeneralCard(string CardID)
        {
            try
            {
                var res = await codeUnitWebService.Client().GetContractCardGeneralAsync(CardID);
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);
                List<EmployeeEndofForm> employeeEndofForms = new List<EmployeeEndofForm>();

                foreach (var item in resSerial)
                {
                    EmployeeEndofForm endofForm = new EmployeeEndofForm
                    {
                        EmpName = item.Employeename,
                        CreationDate = item.Creationdate,
                        Department = item.Department,
                        Status = item.Status,
                        Position = item.Position,

                        Contractno = item.Contractno,
                        Jobtitle = item.Jobtitle,
                        Branch = item.Branch,
                        Product = item.Product,
                        Employmentyear = item.Employmentyear,
                        Tenureofservice = item.Tenureofservice
                    };

                    employeeEndofForms.Add(endofForm);
                }

                return Ok(new { employeeEndofForms });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Card Failed: " + x.Message });
            }
        }


        //Store the Initial Card Data
        [Authorize]
        [HttpPost]
        [Route("v1/storeendofcontractfirstdata")]
        public async Task<IActionResult> StoreEndofContractCreateV1([FromBody] EndofContractProgress endofContract)
        {
            try
            {
                List<EndofContractProgress> endofContracts = new List<EndofContractProgress>();
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var res = await codeUnitWebService.Client().UpdateEndofContractGenSectionAsync(
                    endofContract.ContractNo,
                    endofContract.SupervisionTime,
                    endofContract.DoRenew,
                    endofContract.Howlong,
                    endofContract.RenewReason
                    );

                // This create the record in the Aux DB for progression.
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);
                foreach (var item in resSerial)
                {
                    EndofContractProgress pp = new EndofContractProgress
                    {
                        UID = user.Id,
                        ContractStatus = 0,
                        ContractNo = item.Contractno,

                        EmpID = item.Employeeno,
                        EmpName = item.Employeename,
                        MgrID = item.Managerno,
                        MgrName = item.Managername,
                        CreationDate = item.Creationdate,
                        Department = item.Department,
                        Status = item.Status,
                        Position = item.Position,
                        RenewReason = endofContract.RenewReason,
                        Howlong = endofContract.Howlong,
                        SupervisionTime = item.Supervisiontime,
                        DoRenew = item.Dorenew,
                        HODEid = item.Hodno,
                    };
                    var endofMod = dbContext.EndofContractProgress.Where(x => x.ContractNo == endofContract.ContractNo).Count();
                    //Release the autogenerated record from queue
                    await codeUnitWebService.Client().UpdateEndofContractCardStatusAsync(endofContract.ContractNo);
                    if (endofMod == 1)
                    {
                        var eocModel = dbContext.EndofContractProgress.Where(x => x.ContractNo == endofContract.ContractNo).First();
                        eocModel.UID = user.Id;
                        eocModel.ContractStatus = 0;
                        eocModel.ContractNo = pp.ContractNo;
                        eocModel.EmpID = pp.EmpID;
                        eocModel.EmpName = pp.EmpName;
                        eocModel.MgrID = pp.MgrID;
                        eocModel.MgrName = pp.MgrName;
                        eocModel.CreationDate = pp.CreationDate;
                        eocModel.Department = pp.Department;
                        eocModel.Status = pp.Status;
                        eocModel.Position = pp.Position;
                        eocModel.RenewReason = endofContract.RenewReason;
                        eocModel.Howlong = endofContract.Howlong;
                        eocModel.SupervisionTime = pp.SupervisionTime;
                        eocModel.DoRenew = pp.DoRenew;
                        eocModel.HODEid = pp.HODEid;

                        dbContext.EndofContractProgress.Update(eocModel);
                        await dbContext.SaveChangesAsync();

                        return Ok(true);
                    }
                    else
                    {
                        dbContext.EndofContractProgress.Add(pp);
                        await dbContext.SaveChangesAsync();

                        return Ok(true);
                    }
                  

                }
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "D365 Update Contract General Data Failed" });

            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Update Contract General Data Failed: " + x.Message });
            }
        }


        //Get all Ranks Lev3l EOC
        [Authorize]
        [HttpGet]
        [Route("getstaffcontractlist")]
        public async Task<IActionResult> GetStaffContractList()
        {
            try
            {
                //The pull will be from D365 directly
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var employeeContracts = dbContext.EndofContractProgress.Where(x => x.ContractStatus == 0 && x.MgrID == user.EmployeeId).ToList();

                return Ok(new { employeeContracts });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract List Failed: " + x.Message });
            }
        }

        //Get HOD EOD list
        [Authorize]
        [HttpGet]
        [Route("gethodeoclist")]
        public async Task<IActionResult> GetHODEOCList()
        {
            try
            {
                //The pull will be from D365 directly
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var employeeContracts = dbContext.EndofContractProgress.Where(x => x.ContractStatus == 1 && x.HODEid == user.EmployeeId).ToList();

                return Ok(new { employeeContracts });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract List Failed: " + x.Message });
            }
        }


        //Get all Ranks Lev3l EOC plus Dynamics recs
        [Authorize]
        [HttpGet]
        [Route("v1/getstaffcontractlist")]
        public async Task<IActionResult> GetStaffContractListExtend()
        {
            try
            {
                //The pull will be from D365 directly Then Aux to append to the end.
                List<EndofContractProgress> EOCs = new List<EndofContractProgress>();

                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var eoc = await codeUnitWebService.Client().GetEndofContractListAsync(user.EmployeeId);
                dynamic eocSerial = JsonConvert.DeserializeObject(eoc.return_value);
                if(eocSerial != null) {
                    foreach (var item in eocSerial)
                    {
                        EndofContractProgress eocp = new EndofContractProgress
                        {
                            ContractNo = item.ContractNo,
                            EmpName = item.EmpName,
                            EmpID = item.EmpID,
                            CreationDate = item.CreationDate,
                            Status = item.Status,
                        };
                        EOCs.Add(eocp);
                    }
                }
              

                List<EndofContractProgress> employeeContracts = dbContext.EndofContractProgress.Where(x => x.ContractStatus == 0 && x.MgrID == user.EmployeeId).ToList();
                EOCs.AddRange(employeeContracts);

                return Ok(new { EOCs });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract List Failed: " + x.Message });
            }
        }

        //Upload Contact Section One Data
        [Authorize]
        [HttpPost]
        [Route("uploadcontractsectionone/{PID}")]
        public async Task<IActionResult> UploadContractSectionOne([FromBody] ProbationFirstSection probationFirstSection, string PID)
        {
            try
            {
                bool[] boolData = new bool[50];
                string[] commentArr = new string[25];

                commentArr[0] = probationFirstSection.PerformanceComment;
                commentArr[1] = probationFirstSection.AttendanceComment;
                commentArr[2] = probationFirstSection.AttitudeComment;
                commentArr[3] = probationFirstSection.AppearanceComment;
             

                boolData[0] = probationFirstSection.Outstanding;
                boolData[1] = probationFirstSection.AboveAverage;
                boolData[2] = probationFirstSection.Satisfactory;
                boolData[3] = probationFirstSection.Marginal;
                boolData[4] = probationFirstSection.Unsatisfactory;

                boolData[5] = probationFirstSection.ExcellentAttendance;
                boolData[6] = probationFirstSection.OccasionalAbsence;
                boolData[7] = probationFirstSection.RepeatedAbsence;
                boolData[8] = probationFirstSection.UnjustifiedAbsence;

                boolData[9] = probationFirstSection.AlwaysInterested;
                boolData[10] = probationFirstSection.ReasonablyDevoted;
                boolData[11] = probationFirstSection.PassiveAttitude;
                boolData[12] = probationFirstSection.ActiveDislikeofWork;

                boolData[13] = probationFirstSection.AlwaysNeat;
                boolData[14] = probationFirstSection.GenerallyNeat;
                boolData[15] = probationFirstSection.SometimesCareles;
                boolData[16] = probationFirstSection.AttirenotSuitable;

              

                var res = await codeUnitWebService.Client().UpdateContractProgressFirstSectionAsync(PID, boolData, commentArr);


                return Ok(res.return_value);
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Section One Upload Failed: " + x.Message });
            }
        }
        //Upload Contract Recommendation Section
        [Authorize]
        [HttpPost]
        [Route("uploadcontractrecommendation/{PID}")]
        public async Task<IActionResult> UploadContractRecommendation([FromBody] ProbationRecommendation probationRecommendation, string PID)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                string[] textArr = new string[10];

                textArr[0] = probationRecommendation.EmployeeStrongestPoint;
                textArr[1] = probationRecommendation.EmployeeWeakestPoint;
                textArr[2] = probationRecommendation.EmployeeQualifiedForPromo;
                textArr[3] = probationRecommendation.PromoPosition;
                textArr[4] = probationRecommendation.PromotableInTheFuture;
                textArr[5] = probationRecommendation.EffectiveDifferentAssignment;
                textArr[6] = probationRecommendation.WhichAssignment;
                textArr[7] = probationRecommendation.AdditionalComment;

              

                var res = await codeUnitWebService.Client().UpdateContractRecommendationSectionAsync(PID, textArr);

                var cModel = dbContext.EndofContractProgress.Where(p => p.ContractNo == PID).First();
                cModel.UIDComment = probationRecommendation.AdditionalComment;
                dbContext.EndofContractProgress.Update(cModel);
                await dbContext.SaveChangesAsync();

                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Post Contract: {PID} Recommendation");

                return Ok(res.return_value);

            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:Post Contract: {PID} Recommendation Failed:{x.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Recommendation Upload Failed: " + x.Message });
            }
        }

        //Move EOC To HR from HOD
        [Authorize]
        [HttpGet]
        [Route("movecontractfrommanagertohr/{PID}")]
        public async Task<IActionResult> MoveContractFromManagerToHR(string PID)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {

                var cModel = dbContext.EndofContractProgress.Where(p => p.ContractNo == PID && p.ContractStatus == 1).First();
                cModel.UID = user.Id;
                cModel.ContractStatus = 2;

                dbContext.EndofContractProgress.Update(cModel);
                await dbContext.SaveChangesAsync();
                await codeUnitWebService.Client().UpdateEndofContractCardStatusAsync(PID);

                //Mail HR
                //@email

                var mailHR = await codeUnitWebService.WSMailer().EmployeeEOCManagerToHRAsync(PID);

                //var emailArr = dbContext.Users.Where(x => x.Rank == "HR")
                //    .Select(t => t.Email).ToArray();

                //var unameArr = dbContext.Users.Where(x => x.Rank == "HR")
                //    .Select(t => t.UserName).ToArray();

                //List<ProbationProgressMail> v = new List<ProbationProgressMail>();
                //// v.AddRange(userList);
                //mailService.SendEmail(emailArr, unameArr, PID);

                // return Ok(userList);
                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Move Contract: {PID} From Supervisor to HR Success");


                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Contract Moved: " });
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:Move Contract: {PID} From Supervisor to HR Failed: {x.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Move Failed: " + x.Message });
            }
        }

        //Move Probation To HR from HOD
        [Authorize]
        [HttpPost]
        [Route("v1/movecontractfromhodtohr")]
        public async Task<IActionResult> MoveContractFromHODToHR(EndofContractProgress endofContractProgress)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {

                var cModel = dbContext.EndofContractProgress.Where(p => p.ContractNo == endofContractProgress.ContractNo && p.ContractStatus == 1).First();
                cModel.UID = user.Id;
                cModel.ContractStatus = 2;
                cModel.HODComment = endofContractProgress.HODComment;
                dbContext.EndofContractProgress.Update(cModel);
                await dbContext.SaveChangesAsync();

                // codeUnitWebService.Client().UpdateEndofContractCardStatusAsync(endofContractProgress.ContractNo);

                //Mail HR
                //@email

                var mailHR = await codeUnitWebService.WSMailer().EmployeeEOCManagerToHRAsync(endofContractProgress.ContractNo);

                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Move Contract: {endofContractProgress.ContractNo} From HOD to HR Success");

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Contract Moved: " });
            }
            catch (Exception x)
            {
                logger.LogError($"User:NAp,Verb:POST,Action:Move Contract:{endofContractProgress.ContractNo} from HOD to HR failed,Message:{x.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Move Failed: " + x.Message });
            }
        }


        //Move Contract To HOD from Immediate Supervisor
        [Authorize]
        [HttpPost]
        [Route("movecontractfromsupervisortohod")]
        public async Task<IActionResult> MoveContractFromSupervisorToHOD([FromBody] EndofContractProgress endofContractProgress)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;

                var cModel = dbContext.EndofContractProgress.Where(p => p.ContractNo == endofContractProgress.ContractNo && p.ContractStatus == 0).First();
                cModel.UID = user.Id;
                cModel.ContractStatus = 1;

                dbContext.EndofContractProgress.Update(cModel);
                await dbContext.SaveChangesAsync();

                //await codeUnitWebService.Client().UpdateEndofContractCardStatusAsync(endofContractProgress.ContractNo);

                //Mail Specific HOD
                //@email

                var mailHR = await codeUnitWebService.WSMailer().SupervisorToHODEOCAsync(cModel.ContractNo, cModel.HODEid);


                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Contract Card Move Success");
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Contract Moved" });
            }
            catch (Exception x)
            {
                logger.LogError($"User:NAp,Verb:POST,Action:Contract Card Move failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Move Failed: " + x.Message });
            }
        }


        //Contact Card Data
        [Authorize]
        [HttpGet]
        [Route("contractcarddata/{PID}")]
        public async Task<IActionResult> ContractCardData(string PID)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                List<ProbationFirstSection> probationFirstList = new List<ProbationFirstSection>();
                var res = await codeUnitWebService.Client().GetContractCardDataAsync(PID);
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);

                foreach (var item in resSerial)
                {
                    ProbationFirstSection contractFirstSection = new ProbationFirstSection
                    {

                        Probationno = item.Contractno,
                        Employeeno = item.Employeeno,
                        Employeename = item.Employeename,
                        Creationdate = item.Creationdate,
                        Department = item.Department,
                        Status = item.Status,
                        Position = item.Position,
                        Managername = item.Manangername,


                        Outstanding = item.Outstanding == "Yes" ? true : false,
                        AboveAverage = item.Aboveaverage == "Yes" ? true : false,
                        Satisfactory = item.Satisfactory == "Yes" ? true : false,
                        Marginal = item.Marginal == "Yes" ? true : false,
                        Unsatisfactory = item.Unsatisfactory == "Yes" ? true : false,
                        PerformanceComment = "",

                        ExcellentAttendance = item.ExcellentAttendance == "Yes" ? true : false,
                        OccasionalAbsence = item.OccasionalAbsence == "Yes" ? true : false,
                        RepeatedAbsence = item.RepeatedAbsence == "Yes" ? true : false,
                        UnjustifiedAbsence = item.UnjustifiedAbsence == "Yes" ? true : false,
                        AttendanceComment = item.AttendanceComment,

                        AlwaysInterested = item.AlwaysInterested == "Yes" ? true : false,
                        ReasonablyDevoted = item.ReasonablyDevoted == "Yes" ? true : false,
                        PassiveAttitude = item.PassiveAttitude == "Yes" ? true : false,
                        ActiveDislikeofWork = item.ActiveDislikeofWork == "Yes" ? true : false,
                        AttitudeComment = item.AttitudeComment,

                        AlwaysNeat = item.AlwaysNeat == "Yes" ? true : false,
                        GenerallyNeat = item.GenerallyNeat == "Yes" ? true : false,
                        SometimesCareles = item.SometimesCareles == "Yes" ? true : false,
                        AttirenotSuitable = item.AttirenotSuitable == "Yes" ? true : false,
                        AppearanceComment = item.AppearanceComment,



                 


                        HRcomment = item.HRcomment,
                        MDcomment = item.MDcomment,

                        empStrongestpt = item.empStrongestpt,
                        empWeakestPt = item.emprovementArea,
                        qualifiedPromo = item.qualifiedPromo,
                        promoPstn = item.promoPstn,
                        promotable = item.promotable,
                        effectiveWithDifferent = item.effectiveWithDifferent,
                        differentAssingment = item.differentAssingment,
                        recommendationSectionComment = item.recommendationSectionComment,
                        empRecConfirm = item.empRecConfirm,
                        empRecExtProb = item.empRecExtProb,
                        empRecTerminate = item.empRecTerminate,

                        Jobtitle = item.Jobtitle,
                        Branch = item.Branch,
                        Product = item.Product,
                        Employmentyear = item.Employmentyear,
                        Tenureofservice = item.Tenureofservice,
                        Contractstart = item.Contractstart,
                        Contractexpiry = item.Contractexpiry,


                    };

                    probationFirstList.Add(contractFirstSection);

                }
                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Action:Get Contract Data {PID}");

                return Ok(new { probationFirstList });

            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Action:Get Contract Data {PID} Failed: {x.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Data Failed: " + x.Message });
            }
        }

        // HOD upload End Of Contract Support Documents

        [Authorize]
        [Route("hoduploadendofcontractdocs/{ID}")]
        [HttpPost]
        public async Task<IActionResult> HODUploadEndOfContractDocs([FromForm] IFormFile formFile, string ID)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);


                var subDirectory = "Files/Endofcontract";
                var target = Path.Combine(webHostEnvironment.ContentRootPath, subDirectory);
                string fileName = new String(Path.GetFileNameWithoutExtension(formFile.FileName).Take(10).ToArray()).Replace(' ', '-');
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(formFile.FileName);
                var path = Path.Combine(target, fileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }

                /* 
                 * var host = HttpContext.Request.Host.ToUriComponent();
                 * var url = $"{HttpContext.Request.Scheme}://{host}/{path}";
                 * return Content(url);
                */

                if (dbContext.MonitoringDoc.Where(x => x.UID == user.Id && x.MonitoringID == ID).Count() > 0)
                {
                    var specificCV = dbContext.MonitoringDoc.Where(x => x.UID == user.Id && x.MonitoringID == ID).FirstOrDefault();
                    specificCV.Filepath = path;
                    specificCV.Filename = formFile.FileName;
                    specificCV.MonitoringID = ID;
                    dbContext.Update(specificCV);
                    await dbContext.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "End Of Contract Doc Updated" });

                }
                else
                {
                    MonitoringDoc mData = new MonitoringDoc
                    {
                        UID = user.Id,
                        Filepath = path,
                        Filename = formFile.FileName,
                        MonitoringID = ID,

                    };
                    dbContext.MonitoringDoc.Add(mData);
                    dbContext.SaveChanges();
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "End of Contract Doc Uploaded" });
                }

            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = x.Message });
            }
        }

        // HOD upload Probation Support Documents

        [Authorize]
        [Route("hoduploadprobationdocs/{ID}")]
        [HttpPost]
        public async Task<IActionResult> HODUploadProbationDocs([FromForm] IFormFile formFile, string ID)
        {
            //***********************************USES END OF CONTRACT DOCUMENTS DB AND FILE*******************************************
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {


                var subDirectory = "Files/Endofcontract";
                var target = Path.Combine(webHostEnvironment.ContentRootPath, subDirectory);
                string fileName = new String(Path.GetFileNameWithoutExtension(formFile.FileName).Take(10).ToArray()).Replace(' ', '-');
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(formFile.FileName);
                var path = Path.Combine(target, fileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }

                /* 
                 * var host = HttpContext.Request.Host.ToUriComponent();
                 * var url = $"{HttpContext.Request.Scheme}://{host}/{path}";
                 * return Content(url);
                */

                if (dbContext.MonitoringDoc.Where(x => x.UID == user.Id && x.MonitoringID == ID).Count() > 0)
                {
                    var specificCV = dbContext.MonitoringDoc.Where(x => x.UID == user.Id && x.MonitoringID == ID).FirstOrDefault();
                    specificCV.Filepath = path;
                    specificCV.Filename = formFile.FileName;
                    specificCV.MonitoringID = ID;
                    dbContext.Update(specificCV);
                    await dbContext.SaveChangesAsync();

                    logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Action:HOD Probation Document {ID} Updated");

                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Probation Document Updated" });

                }
                else
                {
                    MonitoringDoc mData = new MonitoringDoc
                    {
                        UID = user.Id,
                        Filepath = path,
                        Filename = formFile.FileName,
                        MonitoringID = ID,

                    };
                    dbContext.MonitoringDoc.Add(mData);
                    dbContext.SaveChanges();

                    logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Action:HOD Probation Document {ID} Uploaded");

                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Probation Document Uploaded" });
                }

            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Action:HOD Probation Document {ID} Upload Failed: {x.Message}");

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = x.Message });
            }
        }

        //HOD Rerverses
        [Authorize]
        [HttpPost]
        [Route("contractreversalfromhod")]
        public async Task<IActionResult> HODReverseContract([FromBody] EndofContractProgressDTO contractProgress)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
                {
                  
                    var contModel = dbContext.EndofContractProgress.Where(x => x.ContractNo == contractProgress.ContractNo).First();
                    contModel.ContractStatus = contractProgress.ContractStatus;
                    contModel.BackTrackingReason = contractProgress.BackTrackingReason;
                    dbContext.EndofContractProgress.Update(contModel);
                    await dbContext.SaveChangesAsync();

                    if (contractProgress.ContractStatus == 0)
                    {

                    //Status 0 :: Immediate Supervisor
                        var supervisor = dbContext.Users.Where(x => x.Id == contModel.UID).FirstOrDefault();
                        var mailHR = await codeUnitWebService.WSMailer().EndofContractReversalAsync(
                            contractProgress.ContractNo,
                            supervisor.EmployeeId,
                            contractProgress.BackTrackingReason,
                            "HOD"
                          );

                        logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Action:HOD Reverses Contract:{contractProgress.ContractNo} to Supervisor:{supervisor.EmployeeId}");

                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Contract Card Reversal Success" });
                    }
                    logger.LogWarning($"User:{user.EmployeeId},Verb:{verb},Action:HOD Reverses Contract Failed - Stage not selected");

                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Card Reversal Failed - Stage not selected" });

                }
                catch (Exception x)
                {
                 
                    logger.LogError($"User:{user.EmployeeId},Verb:{verb},Action:HOD Reverses Contract Failed: {x.Message}");

                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Card Update Failed: " + x.Message });
                }

            
           
        }


        //HR Rerverses
        [Authorize]
        [HttpPost]
        [Route("contractreversalfromhr")]
        public async Task<IActionResult> HRReverseContract([FromBody] EndofContractProgress contractProgress)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                var contModel = dbContext.EndofContractProgress.Where(x => x.ContractNo == contractProgress.ContractNo).First();
                contModel.ContractStatus = contractProgress.ContractStatus;
                contModel.BackTrackingReason = contractProgress.BackTrackingReason;
                dbContext.EndofContractProgress.Update(contModel);
                await dbContext.SaveChangesAsync();

                //@email
                try
                {

                    if (contractProgress.ContractStatus == 0)
                    {

                        //Status 0 :: Immediate Supervisor
                        var rec = dbContext.Users.Where(x => x.Id == contModel.UID).First();
                        var mailHR = await codeUnitWebService.WSMailer().EndofContractReversalAsync(
                            contractProgress.ContractNo,
                            rec.EmployeeId,
                            contractProgress.BackTrackingReason,
                            "HR"
                          );

                        logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Action:HR Reverses Contract:{contractProgress.ContractNo} to Supervisor:{rec.EmployeeId}");

                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Contract Card Reversal Success" });

                    }
                    else if(contractProgress.ContractStatus == 1)
                    {
                        //Status 1 :: HOD
                        var rec = dbContext.Users.Where(x => x.Id == contModel.UID).First();
                        var mailHR = await codeUnitWebService.WSMailer().EndofContractReversalAsync(
                            contractProgress.ContractNo,
                            contModel.HODEid,
                            contractProgress.BackTrackingReason,
                            "HR"
                          );
                        logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Action:HR Reverses Contract:{contractProgress.ContractNo} to HOD:{contModel.HODEid}");

                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Contract Card Reversal Success" });
                    }
                    else
                    {
                        logger.LogWarning($"User:{user.EmployeeId},Verb:{verb},Action:HR Reverses Contract Failed - Stage not selected");

                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Card Reversal Failed - Stage not selected" });
                    }
                }
                catch (Exception e)
                {
                    logger.LogError($"User:{user.EmployeeId},Verb:{verb},Action:HR Reverses Contract Failed: {e.Message}");

                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Creator Missing: " + e.Message });
                }

            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Action:HR Reverses Contract Failed: {x.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Card Update Failed: " + x.Message });
            }
        }


        //********************************* Bucket List **********************************************

        //HOD  Bucket List => Specialized Bucket List
        [Authorize]
        [HttpGet]
        [Route("gethodspecializedbucketlist")]
        public async Task<IActionResult> GetHODSpecializedBucketList()
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {

                var employeeContracts = dbContext.EndofContractProgress.Where(x => x.ContractStatus == 10 && x.UIDFour == user.Id).ToList();
                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Action:HOD View Specialized Bucket");
                return Ok(new { employeeContracts });
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Action:HOD View Specialized Bucket Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Bucket Contract List Failed: " + x.Message });
            }
        }
        //HR Bucket List => Specialized Bucket List
        [Authorize]
        [HttpGet]
        [Route("gethrspecializedbucketlist")]
        public async Task<IActionResult> GetHRSpecializedBucketList()
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {

                var employeeContracts = dbContext.EndofContractProgress.Where(x => x.ContractStatus == 10 && x.UIDTwo == user.Id).ToList();
                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Action:HEAD-HR View Specialized Bucket");

                return Ok(new { employeeContracts });
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Action:HEAD-HR View Specialized Bucket Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Bucket Contract List Failed: " + x.Message });
            }
        }

        //HEAD  Bucket List => Standard Bucket List
        [Authorize]
        [HttpGet]
        [Route("getheadstandardbucketlist")]
        public async Task<IActionResult> GetStandardBucketList()
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {

                var employeeContracts = dbContext.EndofContractProgress.Where(x => x.ContractStatus == 10).ToList();
                logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Action:HEAD-HR View Std Bucket");

                return Ok(new { employeeContracts });
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:GET,Action:HEAD-HR View Std Bucket Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Bucket Contract List Failed: " + x.Message });
            }
        }

        //Moved Contract to Bucket List HOD
        [Authorize]
        [HttpGet]
        [Route("movecontracttobuckethod/{PID}")]
        public async Task<IActionResult> MoveContractToBucketHOD(string PID)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {

                var cModel = dbContext.EndofContractProgress.Where(p => p.ContractNo == PID).First();
                cModel.UIDFour = user.Id;
                cModel.ContractStatus = 10;

                dbContext.EndofContractProgress.Update(cModel);
                await dbContext.SaveChangesAsync();
                // await codeUnitWebService.Client().UpdateEndofContractCardStatusAsync(PID);

                //Mail HR
                //@email

                //var mailHR = await codeUnitWebService.WSMailer().EmployeeEOCManagerToHRAsync(PID);


                logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Action:HOD Move Contract {PID} To Bucket");
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Contract Moved " });
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:GET,Action:HOD Move Contract {PID} To Bucket");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Move Failed: " + x.Message });
            }
        }

        //Moved Contract to Bucket List HR
        [Authorize]
        [HttpGet]
        [Route("movecontracttobuckethr/{PID}")]
        public async Task<IActionResult> MoveContractToBucketHR(string PID)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {

                var cModel = dbContext.EndofContractProgress.Where(p => p.ContractNo == PID).First();
                cModel.UIDTwo = user.Id;
                cModel.ContractStatus = 10;

                dbContext.EndofContractProgress.Update(cModel);
                await dbContext.SaveChangesAsync();
                // await codeUnitWebService.Client().UpdateEndofContractCardStatusAsync(PID);

                //Mail HR
                //@email

                //var mailHR = await codeUnitWebService.WSMailer().EmployeeEOCManagerToHRAsync(PID);


                logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Action:HR Move Contract {PID} To Bucket");
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Contract Moved " });
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:HR Move Contract {PID} To Bucket  Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Move Failed: " + x.Message });
            }
        }



        /**
        * *****************************************************************************************************************
        *                                                      HR SECTION
        * 
        * ************************************************************************************************************************
        */


        //Get the HR list of created contracts
        [Authorize]
        [HttpGet]
        [Route("gethrcontractlist")]
        public async Task<IActionResult> GetHRContractList()
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                var employeeEndofs = dbContext.EndofContractProgress.Where(x => x.ContractStatus == 2).ToList();
                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:HR View Contract List");

                return Ok(new { employeeEndofs });
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:HR View Contract List  Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract List Failed: " + x.Message });
            }
        }

        //Get the HR list of created contracts
        [Authorize]
        [HttpGet]
        [Route("getheadhrcontractlist")]
        public async Task<IActionResult> GetHEADHRContractList()
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                var employeeEndofs = dbContext.EndofContractProgress.Where(x => x.ContractStatus == 2 || x.ContractStatus == 4 ).ToList();
                
                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:HEAD-HR View Contract List");
                return Ok(new { employeeEndofs });
            } 
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:HEAD-HR View Contract List  Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract List Failed: " + x.Message });
            }
        }


        //HR Push the comment
        [Authorize]
        [HttpPost]
        [Route("hrpushcontracttomdfd/{PID}")]
        public async Task<IActionResult> HRPushContractToMDFD([FromBody] ProbationRecommendationModel probationFirst, string PID)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                var resRemarks = await codeUnitWebService.Client().UpdateContractHRremarkAsync(PID, probationFirst.HRcomment);
                if (bool.Parse(resRemarks.return_value))
                {
                    var contModel = dbContext.EndofContractProgress.Where(x => x.ContractNo == PID).First();
                    contModel.ContractStatus = 3;
                    contModel.UIDTwo = user.Id;
                    contModel.UIDTwoComment = probationFirst.HRcomment;

                    dbContext.EndofContractProgress.Update(contModel);
                    await dbContext.SaveChangesAsync();

                    ////Mail MD/FD
                    ///@email
                    
                    var mailStaff = await codeUnitWebService.WSMailer().EmployeeEOCHRToMDFDAsync(PID);


                    logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:HR Push Contract {PID} To MD/FD Success");
                    return Ok(bool.Parse(resRemarks.return_value));
                }
                logger.LogWarning($"User:{user.EmployeeId},Verb:{verb},Path:HR Push Contract {PID} To MD/FD Failed: D365 Failed");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Card Update Failed: D365 failed " });
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:HR Push Contract {PID} To MD/FD  Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Card Update Failed: " + x.Message });
            }
        }

        //HR push to Head HR
        [Authorize]
        [HttpPost]
        [Route("hrpushcontracttoheadhr/{PID}")]
        public async Task<IActionResult> HRPushContractToHeadHR([FromBody] ProbationRecommendationModel probationFirst, string PID)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                var resRemarks = await codeUnitWebService.Client().UpdateContractHRremarkAsync(PID, probationFirst.HRcomment);
                if (bool.Parse(resRemarks.return_value))
                {
                    var contModel = dbContext.EndofContractProgress.Where(x => x.ContractNo == PID).First();
                    contModel.ContractStatus = 4;
                    contModel.UIDTwo = user.Id;
                    contModel.UIDTwoComment = probationFirst.HRcomment;

                    dbContext.EndofContractProgress.Update(contModel);
                    await dbContext.SaveChangesAsync();

                    ////Mail HEAD HR
                    ///@email

                    var mailStaff = await codeUnitWebService.WSMailer().EmployeeEOCHRToHeadHRAsync(PID);


                    logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:HR Push Contract {PID} To Head-HR Success");

                    return Ok(bool.Parse(resRemarks.return_value));
                }
                logger.LogWarning($"User:{user.EmployeeId},Verb:{verb},Path:HR Push Contract {PID} To Head-HR:  D365 failed");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Card Update Failed: D365 failed " });
            
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:HR Push Contract {PID} To Head-HR  Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Card Update Failed: " + x.Message });
            }
        }


        //HR Approves
        [Authorize]
        [HttpGet]
        [Route("hrapprovecontract/{PID}")]
        public async Task<IActionResult> HRApproveContract(string PID)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                var resRemarks = await codeUnitWebService.Client().ApproveContractHRAsync(PID);
                if (bool.Parse(resRemarks.return_value))
                {
                    var contModel = dbContext.EndofContractProgress.Where(x => x.ContractNo == PID).First();
                    contModel.Status = "Approved";
                    contModel.ContractStatus = 4;
                    dbContext.EndofContractProgress.Update(contModel);
                    await dbContext.SaveChangesAsync();

                    ////Mail MD/FD
                    //@email

                    var mailFMD = await codeUnitWebService.WSMailer().EmployeeEOCHRApprovesAsync(PID);
                    logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:HR Approve Contract {PID}  Success");

                    return Ok(bool.Parse(resRemarks.return_value));
                }
                logger.LogWarning($"User:{user.EmployeeId},Verb:{verb},Path:HR Approve Contract {PID}:  D365 failed");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Card Update Failed: D365 failed " });
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:HR Approve Contract {PID}  Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Card Update Failed: " + x.Message });
            }
        }
        //HR View Attached Document
        [Route("getendofdoc/{PID}")]
        [HttpGet]
        public async Task<IActionResult> GetEndOfDoc(string PID)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                //var path = System.AppContext.BaseDirectory;
                var dbres = dbContext.MonitoringDoc.Where(x => x.MonitoringID == PID).First();
                var file = dbres.Filepath;

                // Response...
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file,
                    Inline = true // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:View Monitoring Doc {PID}  Success");

                return File(System.IO.File.ReadAllBytes(file), "application/pdf");
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:View Monitoring Doc {PID} Failed: {x.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Supporting Doc View failed: " + x.Message });
            }
        }

        //Non Renewal
        [Authorize]
        [HttpPost]
        [Route("nonrenewaleocontract")]
        public async Task<IActionResult> NonRenewalEndofContract([FromBody] EndofContractHeader header)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            try
            {
                var resRemarks = await codeUnitWebService.Client().ApproveContractHRAsync(header.EocID);
                if (bool.Parse(resRemarks.return_value))
                {
                    var contModel = dbContext.EndofContractProgress.Where(x => x.ContractNo == header.EocID).First();
                    contModel.Status = "Approved";
                    contModel.ContractStatus = 4;
                    dbContext.EndofContractProgress.Update(contModel);
                    await dbContext.SaveChangesAsync();

                    ////Mail MD/FD
                    //@email
                    var mailStaff = await codeUnitWebService.WSMailer().EndofContractNonRenewalAsync(header.TerminationDate, header.StaffNo);
                    
                    var file = mailStaff.return_value;
                    System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = file,
                        Inline = true // false = prompt the user for downloading;  true = browser to try to show the file inline
                    };
                    Response.Headers.Add("Content-Disposition", cd.ToString());
                    Response.Headers.Add("X-Content-Type-Options", "nosniff");

                    logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Contract {header.EocID} Non-Renewal Success");

                    return File(System.IO.File.ReadAllBytes(file), "application/msword");

                    //return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "End of Contract Non Renewal, Success" });
                   
                   
                    

                }
                else
                {
                    logger.LogWarning($"User:{user.EmployeeId},Verb:{verb},Path:Contract {header.EocID} Non-Renewal Failed:D365 Return False");
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "End of Contract Non Renewal Failed" });

                }
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:Contract {header.EocID} Non-Renewal Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "End of Contract Non Renewal Failed: " + x.Message });
            }
        }

        //Renewal
        [Authorize]
        [HttpPost]
        [Route("renewaleocontract")]
        public async Task<IActionResult> RenewalEndofContract([FromBody] EndofContractHeader header)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            try
            {
                var resRemarks = await codeUnitWebService.Client().ApproveContractHRAsync(header.EocID);
                if (bool.Parse(resRemarks.return_value))
                {
                    var contModel = dbContext.EndofContractProgress.Where(x => x.ContractNo == header.EocID).First();
                    contModel.Status = "Approved";
                    contModel.ContractStatus = 4;
                    dbContext.EndofContractProgress.Update(contModel);
                    await dbContext.SaveChangesAsync();

                    //@email
                    //Mail depending 
                    if(header.NewSalary.Length <= 0)
                    {
                        var mailStaff = await codeUnitWebService.WSMailer().EndofContractRenewalAsync(header.RenewalTime, header.ContractedDate, header.StartDate, header.EndDate, header.StaffNo,header.DateFormulae);

                        var file = mailStaff.return_value;
                        System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                        {
                            FileName = file,
                            Inline = true // false = prompt the user for downloading;  true = browser to try to show the file inline
                        };
                        Response.Headers.Add("Content-Disposition", cd.ToString());
                        Response.Headers.Add("X-Content-Type-Options", "nosniff");

                        logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Contract {header.EocID} Renewal Without Salary Change Success");
                        return File(System.IO.File.ReadAllBytes(file), "application/msword");
                     }
                    else
                    {
                        var mailStaff = await codeUnitWebService.WSMailer().EndofContractRenewalWithARaiseAsync(header.RenewalTime, header.ContractedDate, header.StartDate, header.EndDate, header.StaffNo,header.NewSalary, header.DateFormulae);
                        
                        var file = mailStaff.return_value;
                        System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                        {
                            FileName = file,
                            Inline = true // false = prompt the user for downloading;  true = browser to try to show the file inline
                        };
                        Response.Headers.Add("Content-Disposition", cd.ToString());
                        Response.Headers.Add("X-Content-Type-Options", "nosniff");

                        logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Contract {header.EocID} Renewal With Salary Change Success");

                        return File(System.IO.File.ReadAllBytes(file), "application/msword");
                            }


                }
                else
                {
                    logger.LogWarning($"User:{user.EmployeeId},Verb:{verb},Path:Contract {header.EocID} Renewal Failed:D365 Return False");
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "End of Contract  Renewal Failed" });

                }
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:Contract {header.EocID} Renewal Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "End of Contract Renewal Failed: " + x.Message });
            }
        }







        /**
        * *****************************************************************************************************************
        *                                                      FD SECTION
        * 
        * ************************************************************************************************************************
        */

        //Get the FD list of created probations
        [Authorize]
        [HttpGet]
        [Route("getfdcontractlist")]
        public async Task<IActionResult> GetFDContractList()
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                var employeeEndofs = dbContext.EndofContractProgress.Where(x => x.ContractStatus == 3).ToList();

                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:MD/FD Get Contract List Success");
                return Ok(new { employeeEndofs });
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:MD/FD Get Contract List Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract List Failed: " + x.Message });
            }
        }

        //FD Approves
        [Authorize]
        [HttpPost]
        [Route("fdapprovecontract/{PID}")]
        public async Task<IActionResult> FDApproveContract([FromBody] ProbationRecommendationModel probationRecommendationModel, string PID)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                var resRemarks = await codeUnitWebService.Client().UpdateContractMFDremarkAsync(PID, probationRecommendationModel.MDcomment);
                
                if (bool.Parse(resRemarks.return_value))
                {
                    var contModel = dbContext.EndofContractProgress.Where(x => x.ContractNo == PID).First();
                    contModel.ContractStatus = 4;
                    contModel.UIDThree = user.Id;
                    contModel.UIDThreeComment = probationRecommendationModel.MDcomment;

                    dbContext.EndofContractProgress.Update(contModel);
                    await dbContext.SaveChangesAsync();

                    //Mail MD/FD
                    //@email
                    var mailHR = await codeUnitWebService.WSMailer().EmployeeEOCMDFDApprovesAsync(PID);
                    logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:MD/FD Contract {PID} Approval Success");

                    return Ok(bool.Parse(resRemarks.return_value));
                }

                
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Card Update Failed: D365 failed " });
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:MD/FD Contract {PID} Approval Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Card Update Failed: " + x.Message });
            }
        }

        //FD Reject
        [Authorize]
        [HttpPost]
        [Route("fdrejectcontract/{PID}")]
        public async Task<IActionResult> FDRejectContract([FromBody] ProbationRecommendationModel probationRecommendationModel, string PID)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            try
            {
                var resRemarks = await codeUnitWebService.Client().UpdateContractMFDremarkAsync(PID, probationRecommendationModel.MDcomment);
                if (bool.Parse(resRemarks.return_value))
                {
                    var rejectRes = await codeUnitWebService.Client().RejectContractMFDAsync(PID);
                    var contModel = dbContext.EndofContractProgress.Where(x => x.ContractNo == PID).First();
                    contModel.ContractStatus = 4;
                    contModel.UIDThree = user.Id;
                    contModel.Status = "Rejected";
                    contModel.UIDThreeComment = probationRecommendationModel.MDcomment;

                    dbContext.EndofContractProgress.Update(contModel);
                    await dbContext.SaveChangesAsync();

                    //Mail MD/FD
                    //@email
                    var mailHR = await codeUnitWebService.WSMailer().EmployeeEOCMDFDRejectsAsync(PID);

                    logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:MD/FD Contract {PID} Reject  Success");
                    return Ok(bool.Parse(resRemarks.return_value));
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Card Update Failed: D365 failed " });
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:MD/FD Contract {PID} Reject Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract  Reject Failed: " + x.Message });
            }
        }

        //FD Rerverses
        [Authorize]
        [HttpPost]
        [Route("contractreversal")]
        public async Task<IActionResult> FDReverseContract([FromBody] EndofContractProgress contractProgress)
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var verb = Request.HttpContext.Request.Method;
            try
            {
                var contModel = dbContext.EndofContractProgress.Where(x => x.ContractNo == contractProgress.ContractNo).First();
                contModel.ContractStatus = contractProgress.ContractStatus;
                contModel.BackTrackingReason = contractProgress.BackTrackingReason;
                dbContext.EndofContractProgress.Update(contModel);
                await dbContext.SaveChangesAsync();

                //@email
                try
                {
               
                    if(contractProgress.ContractStatus == 1)
                    {

                    //Status 1 :: HOD
                    //var rec = dbContext.Users.Where(x => x.Id == contModel.UID).First();
                    var mailHR = await codeUnitWebService.WSMailer().EndofContractReversalAsync(
                        contractProgress.ContractNo,
                        contModel.HODEid,
                        contractProgress.BackTrackingReason,
                        "MD"
                      );

                        logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:MD/FD Contract {contractProgress.ContractNo} Reversal To HOD Success");

                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Contract Card Reversal Success" });
                    }
                    else
                    {
                        //Status 2 :: HR
                        var rec = dbContext.Users.Where(x => x.Id == contModel.UIDTwo).First();

                        var mailHR = await codeUnitWebService.WSMailer().EndofContractReversalAsync(
                            contractProgress.ContractNo,
                            rec.EmployeeId,
                            contractProgress.BackTrackingReason,
                            "MD"
                          );

                        logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:MD/FD Contract {contractProgress.ContractNo} Reversal To HR Success");

                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Contract Card Reversal Success" });
                    }
                }
                catch (Exception e)
                {
                    logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:MD/FD Contract {contractProgress.ContractNo} Reversal Failed: {e.Message}");

                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Creator Missing: "+e.Message});
                }
                  
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:MD/FD Contract {contractProgress.ContractNo} Reversal Failed: {x.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Card Update Failed: " + x.Message });
            }
        }


        //***************************************************************************************
        //
        //
        //                                   EMPLOYEE PROBATION PROGRESS
        //
        //****************************************************************************************


        //Non-Confirmation
        [Authorize]
        [HttpPost]
        [Route("nonconfirmation")]
        public async Task<IActionResult> NonConfirmation([FromBody] ProbationHeader header)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
        
                var res = await codeUnitWebService.Client().ApproveProbationHRAsync(header.ProbationID);
                if (res.return_value == "true")
                {
                    var probModel = dbContext.ProbationProgress.Where(x => x.ProbationNo == header.ProbationID).First();
                    probModel.Status = "Approved";
                    probModel.ProbationStatus = 4; //approv is 4 while reject  is 5
                    dbContext.ProbationProgress.Update(probModel);
                    await dbContext.SaveChangesAsync();

                    //mail Employee
                    var mailEmployee = await codeUnitWebService.WSMailer().ProbationNonConfirmationAsync(header.StaffID, header.ProbationEndDate);
                    var file = mailEmployee.return_value;
                    System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = file,
                        Inline = true // false = prompt the user for downloading;  true = browser to try to show the file inline
                    };
                    Response.Headers.Add("Content-Disposition", cd.ToString());
                    Response.Headers.Add("X-Content-Type-Options", "nosniff");

                    logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Probation {header.ProbationID} Non-Confirmation Success");
                    return File(System.IO.File.ReadAllBytes(file), "application/msword");

                    //mail MD/FD
                    //var mailsManager = await codeUnitWebService.WSMailer().EmployeeProbationHRApprovesAsync(header.ProbationID);
                    //return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Non Confirmation Success" });
                }
                else
                {
                    logger.LogWarning($"User:{user.EmployeeId},Verb:{verb},Path:Probation {header.ProbationID} Confirmation Failed: D365 Approval Returned False");

                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Success", Message = "Non Confirmation Failed" });
                }

            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:Probation {header.ProbationID} Non-Confirmation Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Non Confirmation Failed " + x.Message });
            }
        }

        //Confirmation
        [Authorize]
        [HttpPost]
        [Route("confirmation")]
        public async Task<IActionResult> Confirmation([FromBody] ProbationHeader header)
        {
            var verb = Request.HttpContext.Request.Method;
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                var res = await codeUnitWebService.Client().ApproveProbationHRAsync(header.ProbationID);
                if (res.return_value == "true")
                {
                    var probModel = dbContext.ProbationProgress.Where(x => x.ProbationNo == header.ProbationID).First();
                    probModel.Status = "Approved";
                    probModel.ProbationStatus = 4; //approv is 4 while reject  is 5
                    dbContext.ProbationProgress.Update(probModel);
                    await dbContext.SaveChangesAsync();

                    //mail Employee
                    var mailEmployee = await codeUnitWebService.WSMailer().ProbationConfirmationAsync(header.StaffID, header.ProbationDate, header.ProbationExpire);
                    var file = mailEmployee.return_value;
                    System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = file,
                        Inline = true // false = prompt the user for downloading;  true = browser to try to show the file inline
                    };
                    Response.Headers.Add("Content-Disposition", cd.ToString());
                    Response.Headers.Add("X-Content-Type-Options", "nosniff");

                    logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Probation {header.ProbationID} Confirmation Success");
                    return File(System.IO.File.ReadAllBytes(file), "application/msword");


                    //mail MD/FD
                    //var mailsManager = await codeUnitWebService.WSMailer().EmployeeProbationHRApprovesAsync(header.ProbationID);

                    //return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Confirmation Success" });
                }
                else
                {
                    logger.LogWarning($"User:{user.EmployeeId},Verb:{verb},Path:Probation {header.ProbationID} Confirmation Failed: D365 Approval Returned False");

                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Success", Message = " Confirmation Failed" });
                }
            }
            catch (Exception x)
            {
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:Probation {header.ProbationID} Confirmation Failed: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Confirmation Failed " + x.Message });
            }
        }

        //Extension
        [Authorize]
        [HttpPost]
        [Route("extension")]
        public async Task<IActionResult> Extension([FromBody] ProbationHeader header)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;

                var res = await codeUnitWebService.Client().ApproveProbationHRAsync(header.ProbationID);
                if (res.return_value == "true")
                {

                    /* var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                     var monModel = dbContext.PerformanceMonitoring.Where(x => x.PerformanceId == header.ProbationID).FirstOrDefault();
                     monModel.Progresscode = 2;
                     monModel.HRId = user.Id;
                     monModel.ApprovalStatus = "Approved";
                     dbContext.PerformanceMonitoring.Update(monModel);
                     await dbContext.SaveChangesAsync();*/
                    var probModel = dbContext.ProbationProgress.Where(x => x.ProbationNo == header.ProbationID).First();
                    probModel.Status = "Approved";
                    probModel.ProbationStatus = 4; //approv is 4 while reject  is 5
                    dbContext.ProbationProgress.Update(probModel);
                    await dbContext.SaveChangesAsync();


                    //mail Employee
                    var mailEmployee = await codeUnitWebService.WSMailer().ProbationExtensionAsync(header.StaffID, header.ExtendDate, header.NextReviewDate, header.ExtendDuration,header.DateFormulae);
                    var file = mailEmployee.return_value;

                    logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Probation Extension Success");
                    System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = file,
                        Inline = true // false = prompt the user for downloading;  true = browser to try to show the file inline
                    };
                    Response.Headers.Add("Content-Disposition", cd.ToString());
                    Response.Headers.Add("X-Content-Type-Options", "nosniff");

                    return File(System.IO.File.ReadAllBytes(file), "application/msword");


                    //mail MD/FD
                    //var mailsManager = await codeUnitWebService.WSMailer().EmployeeProbationHRApprovesAsync(header.ProbationID);

                    //return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Extension Success" });
                }
                else
                {
                    logger.LogWarning($"User:{user.EmployeeId},Verb:{verb},Path:Probation Extension Failed");
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Success", Message = " Extension Failed" });
                }
            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;
                logger.LogError($"User:{user.EmployeeId},Verb:POST,Action:Probation Extension Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Extension Failed " + x.Message });
            }
        }



        //HOD Rerverses
        [Authorize]
        [HttpPost]
        [Route("probationreversalfromhod")]
        public async Task<IActionResult> HODReverseProbation([FromBody] ProbationProgress probationProgress)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;
                var contModel = dbContext.ProbationProgress.Where(x => x.ProbationNo == probationProgress.ProbationNo).First();
                contModel.ProbationStatus = probationProgress.ProbationStatus;
                contModel.BackTrackingReason = probationProgress.BackTrackingReason;
                dbContext.ProbationProgress.Update(contModel);
                await dbContext.SaveChangesAsync();

                //@email
                try
                {

                    if (probationProgress.ProbationStatus == 0)
                    {

                        //Status 0 :: Immediate Supervisor
                        var rec = dbContext.Users.Where(x => x.Id == contModel.UID).First();
                        var mailHR = await codeUnitWebService.WSMailer().ProbationReversalAsync(
                            probationProgress.ProbationNo,
                            rec.EmployeeId,
                            probationProgress.BackTrackingReason,
                            "HOD"
                          );
                        logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:HOD Probation Card Reversal To Immediate Supervisor:{probationProgress.ProbationNo} Success");
                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Probation Card Reversal Success" });
                    }
                    logger.LogWarning($"User:{user.EmployeeId},Verb:{verb},Path:HOD Probation Card Reversal Failed - Stage not selected");
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD Probation Card Reversal Failed - Stage not selected" });

                }
                catch (Exception e)
                {
                    logger.LogError($"User:{user.EmployeeId},Verb:POST,Action:HOD Probation Card Reversal Failed,Message:{e.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD Probation Creator Missing: " + e.Message });
                }

            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{user.EmployeeId},Verb:POST,Action:Probation Card Reversal Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Reversal Failed: " + x.Message });
            }
        }
        
        //HR Rerverses
        [Authorize]
        [HttpPost]
        [Route("probationreversalfromhr")]
        public async Task<IActionResult> HRReverseProbation([FromBody] ProbationProgress probationProgress)
        {
            var verb = Request.HttpContext.Request.Method;
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var contModel = dbContext.ProbationProgress.Where(x => x.ProbationNo == probationProgress.ProbationNo).First();
                contModel.ProbationStatus = probationProgress.ProbationStatus;
                contModel.BackTrackingReason = probationProgress.BackTrackingReason;
                dbContext.ProbationProgress.Update(contModel);
                await dbContext.SaveChangesAsync();

                //@email
                try
                {

                    if (probationProgress.ProbationStatus == 0)
                    {

                        //Status 0 :: Immediate Supervisor
                        var rec = dbContext.Users.Where(x => x.Id == contModel.UID).First();
                        var mailHR = await codeUnitWebService.WSMailer().ProbationReversalAsync(
                            probationProgress.ProbationNo,
                            rec.EmployeeId,
                            probationProgress.BackTrackingReason,
                            "HR"
                          );

                        logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path: Probation {probationProgress.ProbationNo} Reversal From HR To Immediate Supervisor:{rec.EmployeeId} Success");
                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Probation Card Reversal Success" });

                    }
                    else if (probationProgress.ProbationStatus == 1)
                    {
                        //Status 1 :: HOD
                        var rec = dbContext.Users.Where(x => x.Id == contModel.UID).First();
                        var mailHR = await codeUnitWebService.WSMailer().EndofContractReversalAsync(
                            probationProgress.ProbationNo,
                            contModel.HODEid,
                            probationProgress.BackTrackingReason,
                            "HR"
                          );

                        logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path: Probation {probationProgress.ProbationNo} Reversal From HR To HOD:{contModel.HODEid} Success");

                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Probation Card Reversal Success" });
                    }
                    else
                    {
                        logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path: Probation {probationProgress.ProbationNo} Reversal From HR Failed - Stage not Selected");

                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Reversal Failed - Stage not selected" });
                    }
                }
                catch (Exception e)
                {
                    logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path: Probation {probationProgress.ProbationNo} Reversal From HR Error:{e.Message}");

                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Creator Missing: " + e.Message });
                }

            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path: Probation {probationProgress.ProbationNo} Reversal From HR Error:{x.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Update Failed: " + x.Message });
            }
        }




        //Moved Probation to Bucket List HOD
        [Authorize]
        [HttpGet]
        [Route("moveprobationtobuckethod/{PID}")]
        public async Task<IActionResult> MoveProbationToBucketHOD(string PID)
        {
            
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;

                var cModel = dbContext.ProbationProgress.Where(p => p.ProbationNo == PID).First();
                cModel.UIDFour = user.Id;
                cModel.ProbationStatus = 10;
                dbContext.ProbationProgress.Update(cModel);
                await dbContext.SaveChangesAsync();


                // await codeUnitWebService.Client().UpdateEndofContractCardStatusAsync(PID);

                //Mail HR
                //@email

                //var mailHR = await codeUnitWebService.WSMailer().EmployeeEOCManagerToHRAsync(PID);


                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Action: HOD Move Probation {PID} To Bucket");
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Probation Moved " });
            }
            catch (Exception x)
            {
                var verb = Request.HttpContext.Request.Method;
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path: HOD Move Probation {PID} to Bucket List Error:{x.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Move Failed: " + x.Message });
            }
        }

        //Moved Probation to Bucket List HR
        [Authorize]
        [HttpGet]
        [Route("moveprobationtobuckethr/{PID}")]
        public async Task<IActionResult> MoveProbationToBucketHR(string PID)
        {
            var verb = Request.HttpContext.Request.Method;
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                var cModel = dbContext.ProbationProgress.Where(p => p.ProbationNo == PID).First();
                cModel.UIDTwo = user.Id;
                cModel.ProbationStatus = 10;

                dbContext.ProbationProgress.Update(cModel);
                await dbContext.SaveChangesAsync();
                // await codeUnitWebService.Client().UpdateEndofContractCardStatusAsync(PID);

                //Mail HR
                //@email

                //var mailHR = await codeUnitWebService.WSMailer().EmployeeEOCManagerToHRAsync(PID);


                logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Action:HR Move Probation {PID} To Bucket");
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Probation Moved " });
            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path: HR Move Probation {PID} to Bucket List Error:{x.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Proation Move Failed: " + x.Message });
            }
        }



        //HOD  Bucket List => Specialized Bucket List
        [Authorize]
        [HttpGet]
        [Route("gethodprobationbucketlist")]
        public async Task<IActionResult> GetHODProbationBucketList()
        {
            var verb = Request.HttpContext.Request.Method;
            try
            {

                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var employeeProbations = dbContext.ProbationProgress.Where(x => x.ProbationStatus == 10 && x.UIDFour == user.Id).ToList();
                
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path: HOD View Probation Bucket List Success");

                return Ok(new { employeeProbations });
            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path: HOD View Probation Bucket List Error:{x.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Bucket Probation List Failed: " + x.Message });
            }
        }

        //HR Bucket List => Specialized Bucket List
        [Authorize]
        [HttpGet]
        [Route("gethrprobationbucketlist")]
        public async Task<IActionResult> GetHRProbationBucketList()
        {
            var verb = Request.HttpContext.Request.Method;
            try
            {

                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var employeeProbations = dbContext.ProbationProgress.Where(x => x.ProbationStatus == 10 && x.UIDTwo == user.Id).ToList();
                
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path: HR View Probation Bucket List Success");
                return Ok(new { employeeProbations });
            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path: HR View Probation Bucket List Error:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Bucket Probation List Failed: " + x.Message });
            }
        }

        //HEAD  Bucket List => Standard Bucket List
        [Authorize]
        [HttpGet]
        [Route("getheadprobationbucketlist")]
        public async Task<IActionResult> GetProbationBucketList()
        {
            var verb = Request.HttpContext.Request.Method;
            try
            {

                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var employeeProbations = dbContext.ProbationProgress.Where(x => x.ProbationStatus == 10).ToList();

                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:HEAD HR View Probation Bucket List Success");
                return Ok(new { employeeProbations });
            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:HEAD HR View Probation Bucket List Error:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Bucket Probation List Failed: " + x.Message });
            }
        }



        //HR push to Head HR
        [Authorize]
        [HttpPost]
        [Route("hrpushprobationtoheadhr/{PID}")]
        public async Task<IActionResult> HRPushProbationToHeadHR([FromBody] ProbationRecommendationModel probationFirst, string PID)
        {
            var verb = Request.HttpContext.Request.Method;
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var resRemarks = await codeUnitWebService.Client().UpdateProbationHRremarkAsync(PID, probationFirst.HRcomment);
                if (bool.Parse(resRemarks.return_value))
                {
                    var contModel = dbContext.ProbationProgress.Where(x => x.ProbationNo == PID).First();
                    contModel.ProbationStatus = 4;
                    contModel.UIDTwo = user.Id;
                    contModel.UIDTwoComment = probationFirst.HRcomment;

                    dbContext.ProbationProgress.Update(contModel);
                    await dbContext.SaveChangesAsync();

                    ////Mail HEAD HR
                    ///@email

                    var mailStaff = await codeUnitWebService.WSMailer().EmployeeProbationHRToHeadHRAsync(PID);


                    logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:HR Move Contract No {PID} To Head HR");
                    return Ok(bool.Parse(resRemarks.return_value));
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Update Failed: D365 failed " });
            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:HR Move Contract No {PID} To Head HR Error:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Probation Card Update Failed: " + x.Message });
            }
        }



        //Move contract from Bucket
        [Authorize]
        [HttpGet]
        [Route("movecontractfrombucket/{CID}/{SID}")]
        public async Task<IActionResult> MoveContractFromBucket(string CID,string SID)
        {
            var verb = Request.HttpContext.Request.Method;
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                var cModel = dbContext.EndofContractProgress.Where(p => p.ContractNo == CID).First();
                //cModel.UIDTwo = user.Id;
                cModel.ContractStatus = int.Parse(SID);

                dbContext.EndofContractProgress.Update(cModel);
                await dbContext.SaveChangesAsync();
                // await codeUnitWebService.Client().UpdateEndofContractCardStatusAsync(PID);

                //Mail HR
                //@email

                //var mailHR = await codeUnitWebService.WSMailer().EmployeeEOCManagerToHRAsync(PID);


                logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Move Contract No {CID} from Bucket List");
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Contract Moved " });
            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{user.EmployeeId},Verb:{verb},Path:Move Contract No {CID} from Bucket List Error: {x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Contract Move Failed: " + x.Message });
            }
        }


        //Moved Probation from Bucket List
        [Authorize]
        [HttpGet]
        [Route("moveprobationfrombucket/{PID}/{SID}")]
        public async Task<IActionResult> MoveProbationFromBucket(string PID,string SID)
        {
            var verb = Request.HttpContext.Request.Method;
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                var cModel = dbContext.ProbationProgress.Where(p => p.ProbationNo == PID).First();
                //cModel.UIDTwo = user.Id;
                cModel.ProbationStatus = int.Parse(SID);

                dbContext.ProbationProgress.Update(cModel);
                await dbContext.SaveChangesAsync();
                // await codeUnitWebService.Client().UpdateEndofContractCardStatusAsync(PID);

                //Mail HR
                //@email

                //var mailHR = await codeUnitWebService.WSMailer().EmployeeEOCManagerToHRAsync(PID);

                logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Path:Move Probation No {PID} from Bucket List");

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Probation Moved " });
            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{user.EmployeeId},Verb:GET,Path:Move Probation No {PID} from Bucket List Failed:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Proation Move Failed: " + x.Message });
            }
        }




    }
}
