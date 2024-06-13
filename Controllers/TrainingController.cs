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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RPFBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<HomeController> logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ICodeUnitWebService codeUnitWebService;
        private readonly IMailService mailService;
        private readonly IOptions<WebserviceCreds> config;
        private readonly RoleManager<IdentityRole> roleManager;

        public TrainingController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext,
        ILogger<HomeController> logger,
        IWebHostEnvironment webHostEnvironment,
        ICodeUnitWebService codeUnitWebService,
        IMailService mailService,
        IOptions<WebserviceCreds> config,
        RoleManager<IdentityRole> roleManager
        )
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.logger = logger;
            this.webHostEnvironment = webHostEnvironment;
            this.codeUnitWebService = codeUnitWebService;
            this.mailService = mailService;
            this.config = config;
            this.roleManager = roleManager;
        }

        //Get calender years
        [Authorize]
        [HttpGet]
        [Route("getcalenderyears")]
        public async Task<IActionResult> GetCalendarYears()
        {
            try
            {
                List<CalenderYears> years = new List<CalenderYears>();
                var result = await codeUnitWebService.Client().GetCalenderYearAsync();
                dynamic resSerial = JsonConvert.DeserializeObject(result.return_value);
                foreach (var item in resSerial)
                {
                    CalenderYears calender = new CalenderYears
                    {
                        Label = item.Label,
                        Value = item.Value
                    };
                    years.Add(calender);
                }
                return Ok(new { years });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get calender Years Failed: "+x.Message });
            }
        }
        //Initialize training record
        //Returns PK
        [Authorize]
        [HttpGet]
        [Route("initializetraining")]
        public async Task<IActionResult> InitializeTrainingRecord()
        {
            try
            {
                string recid= "";//TNN0033
                string GenPoint = "";
                var res = await codeUnitWebService.Client().CreateTrainingRecordAsync();
                recid = res.return_value;
                if (recid != null || recid != "")
                {
                    List<EmployeeListModel> employeeList = new List<EmployeeListModel>();
                    var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                    if (user.Rank == "Normal" || user.Rank == "NOS")
                    {
                        GenPoint = "0";
                    }
                    else if (user.Rank == "HOD-ADMIN" || user.Rank == "HOD-HR"|| user.Rank == "HOD-IT"|| user.Rank == "HOD-FIN"|| user.Rank == "HOD")
                    {
                        GenPoint = "1";
                    }
                    else if (user.Rank == "HR")
                    {
                        GenPoint = "2";
                    }
                    else if (user.Rank == "MD"|| user.Rank == "FD")
                    {
                        GenPoint = "3";
                    }
                    else
                    {
                        GenPoint = "0";
                    }

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
                    //generate auxilliary db rec
                    TrainingNeedList trainingNeed = new TrainingNeedList
                    {
                        UID = user.Id,
                        NeedNo = recid,
                        GenesisPoint= GenPoint,
                    };
                    await dbContext.TrainingNeedList.AddAsync(trainingNeed);
                    await dbContext.SaveChangesAsync();

                    return Ok(new { recid , employeeList });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Training record creation failed: Record No is null or empty " });

                }
             
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Training record creation failed: "+x.Message });
            }
        }

        //Modify the blank record
        [Authorize]
        [HttpPost]
        [Route("modifytrainingrecordheader")]
        public async Task<IActionResult>ModifyTrainingRecordHeader([FromBody] TrainingNeedHeader trainingNeedHeader)
        {
            try
            {
                //update nav
                var res = await codeUnitWebService.Client().UpdateTrainingNeedHeaderAsync(trainingNeedHeader.EID, trainingNeedHeader.Calender, trainingNeedHeader.PK);
                if(res.return_value == "true")
                {
                    //update aux db
                    var rec = dbContext.TrainingNeedList.Where(x => x.NeedNo == trainingNeedHeader.PK).First();
                    rec.EName = trainingNeedHeader.Name;
                    rec.EID = trainingNeedHeader.EID;
                    rec.Calender = trainingNeedHeader.Calender;
                    rec.CalenderLabel = trainingNeedHeader.CalenderLabel;
                    //rec.Stage = 0;
                    dbContext.TrainingNeedList.Update(rec);
                    await dbContext.SaveChangesAsync();

                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "General Data Updated" });

                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Updadating Training Need Header failed "  });

                }


            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Posting Training record failed: " + x.Message });
            }
        }

        //Get Training Need List
        [Authorize]
        [HttpGet]
        [Route("gettrainingneedlist/{LID}")]
        public async Task<IActionResult> GetTrainingNeedList(string LID)
        {
            try
            {
                List<TrainingNeedLines> trainingNeedLines = new List<TrainingNeedLines>();
                var listRes = await codeUnitWebService.Client().GetTrainingNeedLinesAsync(LID);
                dynamic listResSerial = JsonConvert.DeserializeObject(listRes.return_value);
                foreach (var item in listResSerial)
                {
                    TrainingNeedLines tnl = new TrainingNeedLines
                    {
                        No = item.No,
                        Lineno = item.Lineno,
                        Developmentneed = item.Developmentneed,
                        Interventionrequired = item.Interventionrequired,
                        Objective = item.Objective,
                        Trainingprovider = item.Trainingprovider,
                        Traininglocation = item.Traininglocation, 
                        //Trainingschedulefrom = "11/03/22",
                        //Trainingscheduleto = "12/03/22",
                        Trainingschedulefrom = item.Trainingschedulefrom, 
                        Trainingscheduleto = item.Trainingscheduleto,
                        //Estimatedcost = int.Parse(item.Estimatedcost, NumberStyles.AllowThousands),
                        //Estimatedcost = Double.Parse(item.Estimatedcost, System.Globalization.NumberStyles.Currency),
                        Estimatedcost = item.Estimatedcost,
                    };
                    trainingNeedLines.Add(tnl);

                }
                //For an exising record get the passed empNo & Name from aux DB
                var rec = dbContext.TrainingNeedList.Where(x => x.NeedNo == LID).First();
                EmployeeListModel employee = new EmployeeListModel
                {
                    Value = rec.EID,
                    Label = rec.EName
                };

                EmployeeListModel clender = new EmployeeListModel
                {
                    Value = rec.Calender,
                    Label = rec.CalenderLabel
                };

                //Comments from Auxilliary DB

                TrainingNeedList comments = new TrainingNeedList
                {
                    HODComment = rec.HODComment,
                    HRComment = rec.HRComment,
                    FMDComment = rec.FMDComment,
                    GenesisPoint = rec.GenesisPoint,
                };

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

       

                return Ok(new { trainingNeedLines, employee, clender,employeeList, comments });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Fetching Training Need Lines failed: " + x.Message });
            }
        }
        //Create Training Need line
        [Authorize]
        [HttpPost]
        [Route("createtrainingneedline")]
        public async Task<IActionResult> CreateTrainingNeedLine([FromBody] TrainingNeedLines trainingNeedLine)
        {
            try
            {
                var res = await codeUnitWebService.Client().CreateTrainingNeedLineAsync(trainingNeedLine.No, trainingNeedLine.Developmentneed, trainingNeedLine.Interventionrequired, trainingNeedLine.Objective, trainingNeedLine.Trainingprovider, trainingNeedLine.Traininglocation, trainingNeedLine.TrainingschedulefromDate, trainingNeedLine.TrainingscheduletoDate, decimal.Parse(trainingNeedLine.Estimatedcost));
                if (res.return_value != "")
                {
                    return Ok(new { res.return_value });
                    //return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Training Line Updated" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Training Line Created failed " });

                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Creating Training Need Lines failed: " + x.Message });
            }
        }

        //Modify training need line
        [Authorize]
        [HttpPost]
        [Route("modifytrainingneedline")]
        public async Task<IActionResult> ModifyTrainingNeedLine([FromBody] TrainingNeedLines trainingNeedLine)
        {
            try
            {
                var res = await codeUnitWebService.Client().ModifyTrainingNeedLineAsync(int.Parse(trainingNeedLine.Lineno), trainingNeedLine.Developmentneed, trainingNeedLine.Interventionrequired, trainingNeedLine.Objective, trainingNeedLine.Trainingprovider, trainingNeedLine.Traininglocation, trainingNeedLine.TrainingschedulefromDate, trainingNeedLine.TrainingscheduletoDate, decimal.Parse(trainingNeedLine.Estimatedcost));
                if (res.return_value == "true")
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Training Line Updated" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Training Line Update failed " });

                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Modifying Training Need Lines failed: " + x.Message });
            }
        }

        //Delete training Need Line
        [Authorize]
        [HttpPost]
        [Route("deletetrainingneedline")]
        public async Task<IActionResult> DeleteTrainingNeedLine([FromBody] TrainingNeedLines trainingNeedLine)
        {
            try
            {
                var res = await codeUnitWebService.Client().DeleteTrainingNeedLineAsync(trainingNeedLine.No, int.Parse(trainingNeedLine.Lineno));
                if (res.return_value=="true")
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Training Line Deleted" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Training Line Deleted failed " });

                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Deleting Training Need Lines failed: " + x.Message });
            }
        }

        //Get Training Need Records [Normal Employee]
        [Authorize]
        [HttpGet]
        [Route("gettrainingneeds")]
        public async Task<IActionResult> GetTrainingNeeds()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var records = dbContext.TrainingNeedList.Where(x => x.UID == user.Id && x.Stage == 0 ).ToList();
                return Ok(new { records });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Training Needs failed: " + x.Message });
            }
        }

        //Get Training Need Records HOD
        [Authorize]
        [HttpGet]
        [Route("gettrainingneedshod")]
        public async Task<IActionResult> GetTrainingNeedsHOD()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var records = dbContext.TrainingNeedList.Where(x => x.UID == user.Id || x.GenesisPoint == "1" || x.Stage == 1).ToList();
                return Ok(new { records });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Training Needs failed: " + x.Message });
            }
        }


        //Get Training Need Records HR
        [Authorize]
        [HttpGet]
        [Route("gettrainingneedshr")]
        public async Task<IActionResult> GetTrainingNeedsHR()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var records = dbContext.TrainingNeedList.Where(x => x.UID == user.Id || x.GenesisPoint == "2" || x.Stage == 2).ToList();
                return Ok(new { records });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Training Needs failed: " + x.Message });
            }
        }

        //Get Training Need Records MD
        [Authorize]
        [HttpGet]
        [Route("gettrainingneedsmd")]
        public async Task<IActionResult> GetTrainingNeedsMD()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var records = dbContext.TrainingNeedList.Where(x => x.UID == user.Id || x.GenesisPoint == "3" || x.Stage == 3).ToList();
                return Ok(new { records });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Training Needs failed: " + x.Message });
            }
        }



        //Get individual record card
        [Authorize]
        [HttpGet]
        [Route("gettrainingneedrecord/{TID}")]
        public async Task<IActionResult> GetTrainingNeedRecord(string TID)
        {
            try
            {
                List<EmployeeListModel> employeeList = new List<EmployeeListModel>();
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var generalData = dbContext.TrainingNeedList.Where(x => x.NeedNo == TID).FirstOrDefault();


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

                return Ok(new { generalData, employeeList });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Training Need failed: " + x.Message });
            }
        }

        //Push Training Need Record to HOD from the creator(Staff)
        [Authorize]
        [HttpGet]
        [Route("pushtrainingneedfromstafftohod/{NID}")]
        public async Task<IActionResult>PushTrainingNeedFromStaffToHOD(string NID)
        {
            try
            {
                var usr =await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var rec = dbContext.TrainingNeedList.Where(x => x.NeedNo == NID).First();
                rec.Stage = 1;
                rec.StageName = "StaffToHOD";

                //Mail HOD
                //-------
                var res = await codeUnitWebService.WSMailer().TrainingNeedAlertAsync(usr.EmployeeId, "HOD", NID);
                if (res.return_value == "true")
                {
                    dbContext.TrainingNeedList.Update(rec);
                    await dbContext.SaveChangesAsync();

                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Moving Training Need from Staff to HOD, Sucess" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Moving Training Need from Staff to HOD Mailing Failed"});

                }



            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Moving Training Need from Staff to HOD failed: " + x.Message });
            }
        }


        /*******************************************************************************************************
         * 
         *                                   HOD LEVEL FUNCTIONALITY
         * 
         * 
         *****************************************************************************************************/
        //HOD Push to HR
        [Authorize]
        [HttpPost]
        [Route("pushtrainingneedfromhodtohr")]
        public async Task<IActionResult> PushTrainingNeedFromHODToHR([FromBody] TrainingNeedList trainingNeed)
        {
            try
            {
                var rec = dbContext.TrainingNeedList.Where(x => x.NeedNo == trainingNeed.NeedNo).First();
                rec.Stage = 2;
                rec.StageName = "HODToHR";
                rec.HODComment = trainingNeed.HODComment;
             

                dbContext.TrainingNeedList.Update(rec);
                await dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Moving Training Need from HOD to HR, Sucess" });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Moving Training Need from  HOD to HR failed: " + x.Message });
            }
        }
        //HOD Push to MFD
        [Authorize]
        [HttpPost]
        [Route("pushtrainingneedfromhodtomdf")]
        public async Task<IActionResult> PushTrainingNeedFromHODToMFD([FromBody] TrainingNeedList trainingNeed)
        {
            try
            {
                var rec = dbContext.TrainingNeedList.Where(x => x.NeedNo == trainingNeed.NeedNo).First();
                rec.Stage = 3;
                rec.StageName = "HODToMFD";
                rec.HODComment = trainingNeed.HODComment;


                dbContext.TrainingNeedList.Update(rec);
                await dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Moving Training Need from HOD to MD/FD, Sucess" });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Moving Training Need from  HOD to MD/FD failed: " + x.Message });
            }
        }

        // Approve if the Genesis is HR
        [Authorize]
        [HttpPost]
        [Route("pushtrainingneedfromhodtoapprove")]
        public async Task<IActionResult> PushTrainingNeedFromHODToApprove([FromBody] TrainingNeedList trainingNeed)
        {
            try
            {
                var rec = dbContext.TrainingNeedList.Where(x => x.NeedNo == trainingNeed.NeedNo).First();
                rec.Stage = 5;
                rec.StageName = "HODToApprove";
                rec.HODComment = trainingNeed.HODComment;


                dbContext.TrainingNeedList.Update(rec);
                await dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Training Need Approved, Sucess" });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Training Need Approval failed: " + x.Message });
            }
        }





        /*******************************************************************************************************
         * 
         *                                   HR LEVEL FUNCTIONALITY
         * 
         * 
         *****************************************************************************************************/
        //HR Push to MD/FD 
        [Authorize]
        [HttpPost]
        [Route("pushtrainingneedfromhrtomfd")]
        public async Task<IActionResult> PushTrainingNeedFromHRToMFD([FromBody] TrainingNeedList trainingNeed)
        {
            try
            {
                var rec = dbContext.TrainingNeedList.Where(x => x.NeedNo == trainingNeed.NeedNo).First();
                rec.Stage = 3;
                rec.StageName = "HRToMFD";
                rec.HRComment = trainingNeed.HRComment;


                dbContext.TrainingNeedList.Update(rec);
                await dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Moving Training Need from HR to MD/FD, Sucess" });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Moving Training Need from  HR to MD/FD failed: " + x.Message });
            }
        }
        //HR Push to HOD
        [Authorize]
        [HttpPost]
        [Route("pushtrainingneedfromhrtohod")]
        public async Task<IActionResult> PushTrainingNeedFromHRToHOD([FromBody] TrainingNeedList trainingNeed)
        {
            try
            {
                var rec = dbContext.TrainingNeedList.Where(x => x.NeedNo == trainingNeed.NeedNo).First();
                rec.Stage = 1;
                rec.StageName = "HRToHOD";
                rec.HRComment = trainingNeed.HRComment;


                dbContext.TrainingNeedList.Update(rec);
                await dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Moving Training Need from HR to HOD, Sucess" });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Moving Training Need from  HR to HOD failed: " + x.Message });
            }
        }

        // Approve if the Genesis is MFD
        [Authorize]
        [HttpPost]
        [Route("pushtrainingneedfromhrtoapprove")]
        public async Task<IActionResult> PushTrainingNeedFromHRToApprove([FromBody] TrainingNeedList trainingNeed)
        {
            try
            {
                var rec = dbContext.TrainingNeedList.Where(x => x.NeedNo == trainingNeed.NeedNo).First();
                rec.Stage = 5;
                rec.StageName = "HRToApprove";
                rec.HRComment = trainingNeed.HRComment;


                dbContext.TrainingNeedList.Update(rec);
                await dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Training Need Approved, Sucess" });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Training Need Approval failed: " + x.Message });
            }
        }

        /*******************************************************************************************************
        * 
        *                                   MFD LEVEL FUNCTIONALITY
        * 
        * 
        *****************************************************************************************************/
        // MD/FD Push to HR
        [Authorize]
        [HttpPost]
        [Route("pushtrainingneedfrommfdtohr")]
        public async Task<IActionResult> PushTrainingNeedFromMFDToHR([FromBody] TrainingNeedList trainingNeed)
        {
            try
            {
                var rec = dbContext.TrainingNeedList.Where(x => x.NeedNo == trainingNeed.NeedNo).First();
                rec.Stage = 4;
                rec.StageName = "MFDToHR";
                rec.FMDComment = trainingNeed.FMDComment;


                dbContext.TrainingNeedList.Update(rec);
                await dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Moving Training Need from  MD/FD to HR, Sucess" });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Moving Training Need from  MD/FD to HR failed: " + x.Message });
            }
        }

        // MD/FD Push to HOD :- When Genesis is MD/FD and when its HR
        [Authorize]
        [HttpPost]
        [Route("pushtrainingneedfrommfdtohod")]
        public async Task<IActionResult> PushTrainingNeedFromMFDToHOD([FromBody] TrainingNeedList trainingNeed)
        {
            try
            {
                var rec = dbContext.TrainingNeedList.Where(x => x.NeedNo == trainingNeed.NeedNo).First();
                rec.Stage = 1;
                rec.StageName = "MFDToHOD";
                rec.FMDComment = trainingNeed.FMDComment;


                dbContext.TrainingNeedList.Update(rec);
                await dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Moving Training Need from  MD/FD to HOD, Sucess" });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Moving Training Need from  MD/FD to HOD failed: " + x.Message });
            }
        }

        // Approve if the Genesis is HOD
        [Authorize]
        [HttpPost]
        [Route("pushtrainingneedfrommdtoapprove")]
        public async Task<IActionResult> PushTrainingNeedFromMDToApprove([FromBody] TrainingNeedList trainingNeed)
        {
            try
            {
                var rec = dbContext.TrainingNeedList.Where(x => x.NeedNo == trainingNeed.NeedNo).First();
                rec.Stage = 5;
                rec.StageName = "HODToApprove";
                rec.FMDComment = trainingNeed.FMDComment;


                dbContext.TrainingNeedList.Update(rec);
                await dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Training Need Approved" });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Training Need Approval failed: " + x.Message });
            }
        }









    }
}
