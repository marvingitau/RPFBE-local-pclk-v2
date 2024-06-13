using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RPFBE.Auth;
using RPFBE.Model.DBEntity;
using RPFBE.Model.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Controllers
{
    public enum ScaleOne
    {
        Poor = 1,
        BelowAverage,
        Average,
        AboveAverage,
        Excellent
    }
    public enum ScaleTwo
    {
        Never = 1,
        Seldom,
        Often,
        Usually,
        Always
    }
    //[EnableCors("CorsPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<HomeController> logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ICodeUnitWebService codeUnitWebService;
        private readonly IMailService mailService;

        public EmployeeController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            ILogger<HomeController> logger,
            IWebHostEnvironment webHostEnvironment,
            ICodeUnitWebService codeUnitWebService,
            IMailService mailService
        )
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.logger = logger;
            this.webHostEnvironment = webHostEnvironment;
            this.codeUnitWebService = codeUnitWebService;
            this.mailService = mailService;
        }

        [Authorize]
        [HttpGet]
        [Route("employeedashboard")]
        public async Task<IActionResult> EmployeeDashboard()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var exitCount = dbContext.ExitInterviewCard.Where(x => x.EID == user.EmployeeId).Count();

                logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Path:View Employee Dasboard Success");
                return Ok(new { exitCount });
            }
            catch (Exception x)
            {
                logger.LogError($"User:NAp,Verb:GET,Action:View Employee Dasboard Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee Dashboard err" + x.Message });
            }
           
        }

        //Get the Form Meta data
        [Authorize]
        [HttpGet]
        [Route("employeeexitform")]
        public async Task<IActionResult> EmployeeExitFormMeta()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var exitModel = dbContext.ExitInterviewCard.Where(x => x.EID == user.EmployeeId).FirstOrDefault();
                if(exitModel != null)
                {
                    logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Path:Get Employee Exit Meta Success");
                    return Ok(new { exitModel });
                }
                else
                {
                    logger.LogWarning($"User:{user.EmployeeId},Verb:GET,Path:Get Employee Exit Meta Failed/Null");
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee Exit Meta Null "});

                }
            }
            catch (Exception x)
            {
                logger.LogError($"User:NAp,Verb:GET,Action:Get Employee Exit Meta Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee Exit Meta err " + x.Message });
            }
        }

        //Push the Exit Form Data
        [Authorize]
        [HttpPost]
        [Route("employeepushform")]
        public async Task<IActionResult> EmployeePushForm ([FromBody] ExitInterviewForm exitInterview)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var exitcard = dbContext.ExitInterviewCard.Where(x => x.EID == user.EmployeeId).First(); //get the exit card pk
                string[] textArr = new string[10];
                bool[] boolArr = new bool[5];
                int[] intArr = new int[40];

                var duplicate = dbContext.ExitInterviewForm.Where(x => x.UID == user.Id).Count();
                //Update if record exist
                if (duplicate > 0)
                {
                    string modextform = "";
                    if (true)
                    {

                        intArr[0] = int.Parse(exitInterview.Fairnessofworkload);
                        intArr[1] = int.Parse(exitInterview.Salary);
                        intArr[2] = int.Parse(exitInterview.WorkingconditionOne);
                        intArr[3] = int.Parse(exitInterview.Toolsprovided);
                        intArr[4] = int.Parse(exitInterview.Trainingreceived);
                        intArr[5] = int.Parse(exitInterview.Rxtioncoworker);
                        intArr[6] = int.Parse(exitInterview.Typeworkperformed);
                        intArr[7] = int.Parse(exitInterview.Supervisonreceived);
                        intArr[8] = int.Parse(exitInterview.Decisionaffected);
                        intArr[9] = int.Parse(exitInterview.Recruitmentprocess);
                        intArr[10] = int.Parse(exitInterview.Employeeorientation);
                        intArr[11] = int.Parse(exitInterview.Trainingopportunity);
                        intArr[12] = int.Parse(exitInterview.Careerdevops);
                        intArr[13] = int.Parse(exitInterview.Employeemorale);
                        intArr[14] = int.Parse(exitInterview.Fairtreatment);
                        intArr[15] = int.Parse(exitInterview.Recognitionofwelldone);
                        intArr[16] = int.Parse(exitInterview.Suportofworklifebal);
                        intArr[17] = int.Parse(exitInterview.Cooperationinoffice);
                        intArr[18] = int.Parse(exitInterview.Communicationmgtemp);
                        intArr[19] = int.Parse(exitInterview.Performancedevplan);
                        intArr[20] = int.Parse(exitInterview.Interestinvemp);
                        intArr[21] = int.Parse(exitInterview.CommitmentCustServ);
                        intArr[22] = int.Parse(exitInterview.ConcernedQualityExcellence);
                        intArr[23] = int.Parse(exitInterview.AdminPolicy);
                        intArr[24] = int.Parse(exitInterview.RecognitionAccomp);
                        intArr[25] = int.Parse(exitInterview.ClearlyCommExpectation);
                        intArr[26] = int.Parse(exitInterview.TreatedFairly);
                        intArr[27] = int.Parse(exitInterview.CoarchedTrainedDev);
                        intArr[28] = int.Parse(exitInterview.ProvidedLeadership);
                        intArr[29] = int.Parse(exitInterview.EncouragedTeamworkCoop);
                        intArr[30] = int.Parse(exitInterview.ResolvedConcerns);
                        intArr[31] = int.Parse(exitInterview.ListeningToSuggetions);
                        intArr[32] = int.Parse(exitInterview.KeptTeamInfo);
                        intArr[33] = int.Parse(exitInterview.SupportedWorkLifeBal);
                        intArr[34] = int.Parse(exitInterview.AppropriateChallengingAssignments);

                        boolArr[0] = exitInterview.Typeofwork == "YES";
                        boolArr[1] = exitInterview.Workingcondition == "YES";
                        boolArr[2] = exitInterview.Payment == "YES";
                        boolArr[3] = exitInterview.Manager == "YES";
                        boolArr[4] = exitInterview.OtherReason == "YES";

                        textArr[0] = exitInterview.Whatulldosummarydous;
                        textArr[1] = exitInterview.TheJobLeaving;
                        textArr[2] = exitInterview.TheOrgoverla;
                        textArr[3] = exitInterview.YourSupervisorMgr;
                        textArr[4] = exitInterview.AnyOtherSuggetionQ;
                        textArr[5] = exitInterview.OtherReasonComment;
                        //WS
                        var resWS = await codeUnitWebService.Client().UpdateExitFormAsync(exitcard.ExitNo, intArr, boolArr, textArr);
                        modextform = resWS.return_value;

                    }

                    //update
                    var mode = dbContext.ExitInterviewForm.Where(x => x.UID == user.Id).FirstOrDefault();
                    mode.Typeofwork = exitInterview.Typeofwork;
                    mode.Workingcondition = exitInterview.Workingcondition;
                    mode.Payment = exitInterview.Payment;
                    mode.Manager = exitInterview.Manager;

                  
                    mode.Fairnessofworkload = exitInterview.Fairnessofworkload.ToString();
                    mode.Salary = exitInterview.Salary.ToString();
                    mode.WorkingconditionOne = exitInterview.WorkingconditionOne.ToString();
                    mode.Toolsprovided = exitInterview.Toolsprovided.ToString();
                    mode.Trainingreceived = exitInterview.Trainingreceived.ToString();
                    mode.Rxtioncoworker = exitInterview.Rxtioncoworker.ToString();
                    mode.Typeworkperformed = exitInterview.Typeworkperformed.ToString();
                    mode.Supervisonreceived = exitInterview.Supervisonreceived.ToString();
                    mode.Decisionaffected = exitInterview.Decisionaffected.ToString();
                    mode.Recruitmentprocess = exitInterview.Recruitmentprocess.ToString();
                    mode.Employeeorientation = exitInterview.Employeeorientation.ToString();
                    mode.Trainingopportunity = exitInterview.Trainingopportunity.ToString();
                    mode.Careerdevops = exitInterview.Careerdevops.ToString();
                    mode.Employeemorale = exitInterview.Employeemorale.ToString();
                    mode.Fairtreatment = exitInterview.Fairtreatment.ToString();
                    mode.Recognitionofwelldone = exitInterview.Recognitionofwelldone.ToString();
                    mode.Suportofworklifebal = exitInterview.Suportofworklifebal.ToString();
                    mode.Cooperationinoffice = exitInterview.Cooperationinoffice.ToString();
                    mode.Communicationmgtemp = exitInterview.Communicationmgtemp.ToString();
                    mode.Performancedevplan = exitInterview.Performancedevplan.ToString();
                    mode.Interestinvemp = exitInterview.Interestinvemp.ToString();
                    mode.CommitmentCustServ = exitInterview.CommitmentCustServ.ToString();
                    mode.ConcernedQualityExcellence = exitInterview.ConcernedQualityExcellence.ToString();
                    mode.AdminPolicy = exitInterview.AdminPolicy.ToString();
                    mode.RecognitionAccomp = exitInterview.RecognitionAccomp.ToString();
                    mode.ClearlyCommExpectation = exitInterview.ClearlyCommExpectation.ToString();
                    mode.TreatedFairly = exitInterview.TreatedFairly.ToString();
                    mode.CoarchedTrainedDev = exitInterview.CoarchedTrainedDev.ToString();
                    mode.ProvidedLeadership = exitInterview.ProvidedLeadership.ToString();
                    mode.EncouragedTeamworkCoop = exitInterview.EncouragedTeamworkCoop.ToString();
                    mode.ResolvedConcerns = exitInterview.ResolvedConcerns.ToString();
                    mode.ListeningToSuggetions = exitInterview.ListeningToSuggetions.ToString();
                    mode.KeptTeamInfo = exitInterview.KeptTeamInfo.ToString();
                    mode.SupportedWorkLifeBal = exitInterview.SupportedWorkLifeBal.ToString();
                    mode.AppropriateChallengingAssignments = exitInterview.AppropriateChallengingAssignments.ToString();

                    mode.Whatulldosummarydous = exitInterview.Whatulldosummarydous;
                    mode.TheJobLeaving = exitInterview.TheJobLeaving;
                    mode.TheOrgoverla = exitInterview.TheOrgoverla;
                    mode.YourSupervisorMgr = exitInterview.YourSupervisorMgr;
                    mode.AnyOtherSuggetionQ = exitInterview.AnyOtherSuggetionQ;
                    mode.NowDate = exitInterview.NowDate;
                    mode.ExitCardRef = exitInterview.ExitCardRef;
                    mode.OtherReason = exitInterview.OtherReason;
                    mode.OtherReasonComment = exitInterview.OtherReasonComment;

                    dbContext.ExitInterviewForm.Update(mode);
                    var resp = await dbContext.SaveChangesAsync();

                  

                    //Update card
                    var rec = dbContext.ExitInterviewCard.Where(x => x.Id == exitInterview.ExitCardRef).FirstOrDefault();
                    rec.FormUploaded = 1;
                    dbContext.ExitInterviewCard.Update(rec);
                    await dbContext.SaveChangesAsync();

                    logger.LogInformation($"User:{user.EmployeeId},Verb:POST,Path:Exit form Update Success");
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Exit form Updated,"+modextform });
                }
                else
                {
                    string modextform = "";
                    if (true)
                    {

                        intArr[0] = int.Parse(exitInterview.Fairnessofworkload);
                        intArr[1] = int.Parse(exitInterview.Salary);
                        intArr[2] = int.Parse(exitInterview.WorkingconditionOne);
                        intArr[3] = int.Parse(exitInterview.Toolsprovided);
                        intArr[4] = int.Parse(exitInterview.Trainingreceived);
                        intArr[5] = int.Parse(exitInterview.Rxtioncoworker);
                        intArr[6] = int.Parse(exitInterview.Typeworkperformed);
                        intArr[7] = int.Parse(exitInterview.Supervisonreceived);
                        intArr[8] = int.Parse(exitInterview.Decisionaffected);
                        intArr[9] = int.Parse(exitInterview.Recruitmentprocess);
                        intArr[10] = int.Parse(exitInterview.Employeeorientation);
                        intArr[11] = int.Parse(exitInterview.Trainingopportunity);
                        intArr[12] = int.Parse(exitInterview.Careerdevops);
                        intArr[13] = int.Parse(exitInterview.Employeemorale);
                        intArr[14] = int.Parse(exitInterview.Fairtreatment);
                        intArr[15] = int.Parse(exitInterview.Recognitionofwelldone);
                        intArr[16] = int.Parse(exitInterview.Suportofworklifebal);
                        intArr[17] = int.Parse(exitInterview.Cooperationinoffice);
                        intArr[18] = int.Parse(exitInterview.Communicationmgtemp);
                        intArr[19] = int.Parse(exitInterview.Performancedevplan);
                        intArr[20] = int.Parse(exitInterview.Interestinvemp);
                        intArr[21] = int.Parse(exitInterview.CommitmentCustServ);
                        intArr[22] = int.Parse(exitInterview.ConcernedQualityExcellence);
                        intArr[23] = int.Parse(exitInterview.AdminPolicy);
                        intArr[24] = int.Parse(exitInterview.RecognitionAccomp);
                        intArr[25] = int.Parse(exitInterview.ClearlyCommExpectation);
                        intArr[26] = int.Parse(exitInterview.TreatedFairly);
                        intArr[27] = int.Parse(exitInterview.CoarchedTrainedDev);
                        intArr[28] = int.Parse(exitInterview.ProvidedLeadership);
                        intArr[29] = int.Parse(exitInterview.EncouragedTeamworkCoop);
                        intArr[30] = int.Parse(exitInterview.ResolvedConcerns);
                        intArr[31] = int.Parse(exitInterview.ListeningToSuggetions);
                        intArr[32] = int.Parse(exitInterview.KeptTeamInfo);
                        intArr[33] = int.Parse(exitInterview.SupportedWorkLifeBal);
                        intArr[34] = int.Parse(exitInterview.AppropriateChallengingAssignments);

                        boolArr[0] = exitInterview.Typeofwork == "YES";
                        boolArr[1] = exitInterview.Workingcondition == "YES";
                        boolArr[2] = exitInterview.Payment == "YES";
                        boolArr[3] = exitInterview.Manager == "YES";
                        boolArr[4] = exitInterview.OtherReason == "YES";

                        textArr[0] = exitInterview.Whatulldosummarydous;
                        textArr[1] = exitInterview.TheJobLeaving;
                        textArr[2] = exitInterview.TheOrgoverla;
                        textArr[3] = exitInterview.YourSupervisorMgr;
                        textArr[4] = exitInterview.AnyOtherSuggetionQ;
                        textArr[5] = exitInterview.OtherReasonComment;
                        //WS
                        var resWS = await codeUnitWebService.Client().UpdateExitFormAsync(exitcard.ExitNo, intArr, boolArr, textArr);
                        modextform = resWS.return_value;

                    }

                    //create
                    ExitInterviewForm aux = new ExitInterviewForm
                    {
                        UID = user.Id,
                        Typeofwork = exitInterview.Typeofwork,
                        Workingcondition = exitInterview.Workingcondition,
                        Payment = exitInterview.Payment,
                        Manager = exitInterview.Manager,
                       
                        /*Fairnessofworkload = ((ScaleOne)int.Parse(exitInterview.Fairnessofworkload)).ToString(),
                         Salary = ((ScaleOne)int.Parse(exitInterview.Salary)).ToString(),
                         WorkingconditionOne = ((ScaleOne)int.Parse(exitInterview.WorkingconditionOne)).ToString(),
                         Toolsprovided = ((ScaleOne)int.Parse(exitInterview.Toolsprovided)).ToString(),
                         Trainingreceived = ((ScaleOne)int.Parse(exitInterview.Trainingreceived)).ToString(),
                         Rxtioncoworker = ((ScaleOne)int.Parse(exitInterview.Rxtioncoworker)).ToString(),
                         Typeworkperformed = ((ScaleOne)int.Parse(exitInterview.Typeworkperformed)).ToString(),
                         Supervisonreceived = ((ScaleOne)int.Parse(exitInterview.Supervisonreceived)).ToString(),
                         Decisionaffected = ((ScaleOne)int.Parse(exitInterview.Decisionaffected)).ToString(),
                         Recruitmentprocess = ((ScaleOne)int.Parse(exitInterview.Recruitmentprocess)).ToString(),
                         Employeeorientation = ((ScaleOne)int.Parse(exitInterview.Employeeorientation)).ToString(),
                         Trainingopportunity = ((ScaleOne)int.Parse(exitInterview.Trainingopportunity)).ToString(),
                         Careerdevops = ((ScaleOne)int.Parse(exitInterview.Careerdevops)).ToString(),
                         Employeemorale = ((ScaleOne)int.Parse(exitInterview.Employeemorale)).ToString(),
                         Fairtreatment = ((ScaleOne)int.Parse(exitInterview.Fairtreatment)).ToString(),
                         Recognitionofwelldone = ((ScaleOne)int.Parse(exitInterview.Recognitionofwelldone)).ToString(),
                         Suportofworklifebal = ((ScaleOne)int.Parse(exitInterview.Suportofworklifebal)).ToString(),
                         Cooperationinoffice = ((ScaleOne)int.Parse(exitInterview.Cooperationinoffice)).ToString(),
                         Communicationmgtemp = ((ScaleOne)int.Parse(exitInterview.Communicationmgtemp)).ToString(),
                         Performancedevplan = ((ScaleOne)int.Parse(exitInterview.Performancedevplan)).ToString(),
                         Interestinvemp = ((ScaleOne)int.Parse(exitInterview.Interestinvemp)).ToString(),
                         CommitmentCustServ = ((ScaleOne)int.Parse(exitInterview.CommitmentCustServ)).ToString(),
                         ConcernedQualityExcellence = ((ScaleOne)int.Parse(exitInterview.ConcernedQualityExcellence)).ToString(),
                         AdminPolicy = ((ScaleOne)int.Parse(exitInterview.AdminPolicy)).ToString(),
                         RecognitionAccomp = ((ScaleTwo)int.Parse(exitInterview.RecognitionAccomp)).ToString(),
                         ClearlyCommExpectation = ((ScaleTwo)int.Parse(exitInterview.ClearlyCommExpectation)).ToString(),
                         TreatedFairly = ((ScaleTwo)int.Parse(exitInterview.TreatedFairly)).ToString(),
                         CoarchedTrainedDev = ((ScaleTwo)int.Parse(exitInterview.CoarchedTrainedDev)).ToString(),
                         ProvidedLeadership = ((ScaleTwo)int.Parse(exitInterview.ProvidedLeadership)).ToString(),
                         EncouragedTeamworkCoop = ((ScaleTwo)int.Parse(exitInterview.EncouragedTeamworkCoop)).ToString(),
                         ResolvedConcerns = ((ScaleTwo)int.Parse(exitInterview.ResolvedConcerns)).ToString(),
                         ListeningToSuggetions = ((ScaleTwo)int.Parse(exitInterview.ListeningToSuggetions)).ToString(),
                         KeptTeamInfo = ((ScaleTwo)int.Parse(exitInterview.KeptTeamInfo)).ToString(),
                         SupportedWorkLifeBal = ((ScaleTwo)int.Parse(exitInterview.SupportedWorkLifeBal)).ToString(),
                         AppropriateChallengingAssignments = ((ScaleTwo)int.Parse(exitInterview.AppropriateChallengingAssignments)).ToString(),*/

                        Fairnessofworkload = exitInterview.Fairnessofworkload.ToString(),
                        Salary = exitInterview.Salary.ToString(),
                        WorkingconditionOne = exitInterview.WorkingconditionOne.ToString(),
                        Toolsprovided = exitInterview.Toolsprovided.ToString(),
                        Trainingreceived = exitInterview.Trainingreceived.ToString(),
                        Rxtioncoworker = exitInterview.Rxtioncoworker.ToString(),
                        Typeworkperformed = exitInterview.Typeworkperformed.ToString(),
                        Supervisonreceived = exitInterview.Supervisonreceived.ToString(),
                        Decisionaffected = exitInterview.Decisionaffected.ToString(),
                        Recruitmentprocess = exitInterview.Recruitmentprocess.ToString(),
                        Employeeorientation = exitInterview.Employeeorientation.ToString(),
                        Trainingopportunity = exitInterview.Trainingopportunity.ToString(),
                        Careerdevops = exitInterview.Careerdevops.ToString(),
                        Employeemorale = exitInterview.Employeemorale.ToString(),
                        Fairtreatment = exitInterview.Fairtreatment.ToString(),
                        Recognitionofwelldone = exitInterview.Recognitionofwelldone.ToString(),
                        Suportofworklifebal = exitInterview.Suportofworklifebal.ToString(),
                        Cooperationinoffice = exitInterview.Cooperationinoffice.ToString(),
                        Communicationmgtemp = exitInterview.Communicationmgtemp.ToString(),
                        Performancedevplan = exitInterview.Performancedevplan.ToString(),
                        Interestinvemp = exitInterview.Interestinvemp.ToString(),
                        CommitmentCustServ = exitInterview.CommitmentCustServ.ToString(),
                        ConcernedQualityExcellence = exitInterview.ConcernedQualityExcellence.ToString(),
                        AdminPolicy = exitInterview.AdminPolicy.ToString(),
                        RecognitionAccomp = exitInterview.RecognitionAccomp.ToString(),
                        ClearlyCommExpectation = exitInterview.ClearlyCommExpectation.ToString(),
                        TreatedFairly = exitInterview.TreatedFairly.ToString(),
                        CoarchedTrainedDev = exitInterview.CoarchedTrainedDev.ToString(),
                        ProvidedLeadership = exitInterview.ProvidedLeadership.ToString(),
                        EncouragedTeamworkCoop = exitInterview.EncouragedTeamworkCoop.ToString(),
                        ResolvedConcerns = exitInterview.ResolvedConcerns.ToString(),
                        ListeningToSuggetions = exitInterview.ListeningToSuggetions.ToString(),
                        KeptTeamInfo = exitInterview.KeptTeamInfo.ToString(),
                        SupportedWorkLifeBal = exitInterview.SupportedWorkLifeBal.ToString(),
                        AppropriateChallengingAssignments = exitInterview.AppropriateChallengingAssignments.ToString(),

                        Whatulldosummarydous = exitInterview.Whatulldosummarydous,
                        TheJobLeaving = exitInterview.TheJobLeaving,
                        TheOrgoverla = exitInterview.TheOrgoverla,
                        YourSupervisorMgr = exitInterview.YourSupervisorMgr,
                        AnyOtherSuggetionQ = exitInterview.AnyOtherSuggetionQ,
                        NowDate = exitInterview.NowDate,
                        ExitCardRef = exitInterview.ExitCardRef,
                        OtherReason = exitInterview.OtherReason,
                        OtherReasonComment = exitInterview.OtherReasonComment,

                };

                    dbContext.ExitInterviewForm.Add(aux);
                    var resp = await dbContext.SaveChangesAsync();
                 
                    //Update card
                    var rec= dbContext.ExitInterviewCard.Where(x => x.Id == exitInterview.ExitCardRef).FirstOrDefault();
                    rec.FormUploaded = 1;
                    dbContext.ExitInterviewCard.Update(rec);
                    await dbContext.SaveChangesAsync();

                    logger.LogInformation($"User:{user.EmployeeId},Verb:POST,Path:Exit form Creation Success");
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Exit form Uploaded," + modextform });
                }
               
            }
            catch (Exception x)
            {
                logger.LogError($"User:NAp,Verb:POST,Action:Employee Exit Form Push Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee Exit Form Push err " + x.Message });
            }
        }
    
        //HR Get the Form Data
        [Authorize]
        [HttpGet]
        [Route("hrgetexitform/{ID}")]

        public async Task<IActionResult> HRGetExitForm(string ID)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var formModel = dbContext.ExitInterviewForm.Where(x => x.ExitCardRef == int.Parse(ID)).FirstOrDefault();
                logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Path:Get Employee Exit Form Success");
                return Ok(formModel);
            }
            catch (Exception x)
            {
                logger.LogError($"User:NAp,Verb:GET,Action:Get Employee Exit Form Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee Exit Form err " + x.Message });
            }
        }

    }
}
