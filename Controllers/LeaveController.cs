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
using RPFBE.Model.LeaveModels;
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
    public class LeaveController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<HomeController> logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ICodeUnitWebService codeUnitWebService;
        private readonly IMailService mailService;
        private readonly IOptions<WebserviceCreds> config;

        public LeaveController(
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
        //Get Leave Type and Balance
        [Authorize]
        [HttpGet]
        [Route("getstaffleavebalance")]
        public async Task<IActionResult> GetStaffLeaveBalance()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var leavetypelist = await codeUnitWebService.Client().EmployeeLeavesAsync(user.EmployeeId);
                List<LeaveTypes> leaveTypeList = new List<LeaveTypes>();

                dynamic leavetypelistSerial = JsonConvert.DeserializeObject(leavetypelist.return_value);

                foreach (var item in leavetypelistSerial)
                {
                    LeaveTypes ltyp = new LeaveTypes
                    {
                        Value = item.Value,
                        Label = item.Label,
                        Leavebalance = item.Leavebalance,
                        Allocationdays = item.Allocationdays,
                        Employeeno = item.Employeeno
                    };
                    leaveTypeList.Add(ltyp);
                }
                return Ok(new { leaveTypeList });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Document Read check failed: " + x.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("getstaffleavebalance/{eid}")]
        public async Task<IActionResult> GetApproveeLeaveBalance(string eid)
        {
            try
            {
                //var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var leavetypelist = await codeUnitWebService.Client().EmployeeLeavesAsync(eid);
                List<LeaveTypes> leaveTypeList = new List<LeaveTypes>();

                dynamic leavetypelistSerial = JsonConvert.DeserializeObject(leavetypelist.return_value);

                foreach (var item in leavetypelistSerial)
                {
                    LeaveTypes ltyp = new LeaveTypes
                    {
                        Value = item.Value,
                        Label = item.Label,
                        Leavebalance = item.Leavebalance,
                        Allocationdays = item.Allocationdays,
                        Employeeno = item.Employeeno
                    };
                    leaveTypeList.Add(ltyp);
                }
                return Ok(new { leaveTypeList });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Document Read check failed: " + x.Message });
            }
        }


        //Leave Application List
        [Authorize]
        [HttpGet]
        [Route("getleaveapplicationlist")]
        public async Task<IActionResult> GetLeaveApplicationList()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var leaveApplication = await codeUnitWebService.HRWS().GetleavesAsync("", user.EmployeeId);
                dynamic leaveApplicationSerial = JsonConvert.DeserializeObject(leaveApplication.return_value);
                List<LeaveApplicationList> leaveApplications = new List<LeaveApplicationList>();

                foreach (var item in leaveApplicationSerial)
                {
                    LeaveApplicationList lap = new LeaveApplicationList
                    {

                        No = item.No,
                        EmployeeNo = item.EmployeeNo,
                        EmployeeName = item.EmployeeName,
                        LeaveType = item.LeaveType,
                        LeaveStartDate = item.LeaveStartDate,
                        LeaveBalance = item.LeaveBalance,
                        DaysApplied = item.DaysApplied,
                        DaysApproved = item.DaysApproved,
                        LeaveEndDate = item.LeaveEndDate,
                        LeaveReturnDate = item.LeaveReturnDate,
                        ReasonForLeave = item.ReasonForLeave,
                        SubstituteEmployeeNo = item.SubstituteEmployeeNo,
                        SubstituteEmployeeName = item.SubstituteEmployeeName,
                        GlobalDimension1Code = item.GlobalDimension1Code,
                        GlobalDimension2Code = item.GlobalDimension2Code,
                        ShortcutDimension3Code = item.ShortcutDimension3Code,
                        ShortcutDimension4Code = item.ShortcutDimension4Code,
                        ShortcutDimension5Code = item.ShortcutDimension5Code,
                        ShortcutDimension6Code = item.ShortcutDimension6Code,
                        ShortcutDimension7Code = item.ShortcutDimension7Code,
                        ShortcutDimension8Code = item.ShortcutDimension8Code,
                        ResponsibilityCenter = item.ResponsibilityCenter,
                        RejectionComments = item.RejectionComments,
                        Status = item.Status,
                    };
                    leaveApplications.Add(lap);
                }
                return Ok(new { leaveApplications });

            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Leave Application List Failed: " + x.Message });
            }
        }

        //Create a New Leave
        [Authorize]
        [HttpGet]
        [Route("createnewleave")]
        public async Task<IActionResult> CreateNewLeave()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                //Create a record
                //var isLeaveOpen = false;
                var isLeaveOpen = await codeUnitWebService.HRWS().CheckOpenLeaveApplicationExistsAsync(user.EmployeeId);
                if (!isLeaveOpen.return_value)
                {
                    var leaveNo = await codeUnitWebService.HRWS().CreateNewLeaveApplicationAPIAsync(user.EmployeeId);
                    //var return_value = "LA00080";
                    if (leaveNo.return_value != "false")
                    {
                        List<EmployeeListModel> employeeListModels = new List<EmployeeListModel>();

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

                        return Ok(new { leaveNo.return_value, employeeListModels });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = $"New  D365 Leave Initialization Failed" });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = $"The Employee {user.EmployeeId} has an Open Leave" });
                }

                //Get employees
                //Get Leave types , is attachment needed?

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Leave Create New Failed: " + x.Message });
            }
        }

        //On select Leave Type
        [Authorize]
        [HttpGet]
        [Route("onselectleavetype/{LNO}/{LTYP}")]
        public async Task<IActionResult> OnSelectLeavetype(string LNO, string LTYP)
        {
            try
            {
                //upload the leave type to portal documents
                var res = await codeUnitWebService.HRWS().InsertLeaveApplicationDocumentsAsync(LNO, LTYP);
                if (res.return_value)
                {
                    //Check of selected leave type is attachment required
                    var isAttachementRequired = await codeUnitWebService.Client().GetLeaveAttachmentStatusAsync(LTYP);
                    //Does Leave has Extra Days -- DEPRECATED -- Moved to onleavesubmit
                    var hasExtradays = await codeUnitWebService.Client().HasLeaveHasExtraDaysAsync(LNO,LTYP);
                    var hasExtraDays = LTYP == config.Value.LeaveType ? true : false;

                    //return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = $"The Leave {LNO} of {LTYP} has been recorded" });
                    return Ok(new { isAttachementRequired.return_value, hasExtraDays });
                }
                else
                {
                    var isAttachementRequired = await codeUnitWebService.Client().GetLeaveAttachmentStatusAsync(LTYP);

                    var hasExtradays = await codeUnitWebService.Client().HasLeaveHasExtraDaysAsync(LNO, LTYP);
                    var hasExtraDays = LTYP == config.Value.LeaveType ? true : false;

                    return Ok(new { isAttachementRequired.return_value, hasExtraDays });
                    //return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = $"The Leave {LNO} of {LTYP} has not been recorded " });
                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "LeaveType Create New Record Failed: " + x.Message });
            }
        }

        //Check if Attachment is required
        [Authorize]
        [HttpGet]
        [Route("checkifattachmentisrequired/{LTYP}")]
        public async Task<IActionResult> IsAttachementRequired(string LTYP)
        {
            try
            {

                //Check of selected leave type is attachment required
                var isAttachementRequired = await codeUnitWebService.Client().GetLeaveAttachmentStatusAsync(LTYP);

                //return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = $"The Leave {LNO} of {LTYP} has been recorded" });
                return Ok(new { isAttachementRequired.return_value });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Check Leave Attachment Failed: " + x.Message });
            }
        }


        //Get End Date
        [Authorize]
        [HttpPost]
        [Route("getleaveendreturndate")]
        public async Task<IActionResult> GetLeaveEndReturnDate([FromBody] LeaveEndDate leaveEndDate)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var endDate = await codeUnitWebService.HRWS().GetLeaveEndDateAsync(user.EmployeeId, leaveEndDate.LeaveType, leaveEndDate.LeaveStartDate, Convert.ToDecimal(leaveEndDate.DaysApplied));
                var returnDate = await codeUnitWebService.HRWS().GetLeaveReturnDateAsync(user.EmployeeId, leaveEndDate.LeaveType, leaveEndDate.LeaveStartDate, Convert.ToDecimal(leaveEndDate.DaysApplied));

                DateTime EndDate = endDate.return_value;
                DateTime ReturnDate = returnDate.return_value;
                var EndD = EndDate.ToString("MM/dd/yyyy");
                var ReturnD = ReturnDate.ToString("MM/dd/yyyy");
                return Ok(new { EndD, ReturnD });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Leave End Return Date Failed: " + x.Message });
            }
        }

        //Upload Leave Attachment
        [Authorize]
        [Route("uploadleaveattachment/{LNO}/{LTYP}")]
        [HttpPost]
        public async Task<IActionResult> UploadLeaveAttachment([FromForm] IFormFile formFile, string LNO, string LTYP)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);


                var subDirectory = "Files/LeaveAttachments";
                var target = Path.Combine(webHostEnvironment.ContentRootPath, subDirectory);
                // string fileName = new String(Path.GetFileNameWithoutExtension(formFile.FileName).Take(10).ToArray()).Replace(' ', '-');
                // DateTime.Now.ToString("yymmssfff")
                string fileName = LNO + "_" + LTYP;
                fileName = fileName + Path.GetExtension(formFile.FileName);
                var path = Path.Combine(target, fileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }

                //Update portal document record
                var updateDocRec = await codeUnitWebService.DOCMGT().ModifySystemFileURLAsync(LNO, LTYP, path);
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Attachment Upload" });


            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = x.Message });
            }
        }

        //View Attachment
        [Authorize]
        [Route("viewleaveattachment/{LNO}")]
        [HttpGet]
        public async Task<IActionResult> ViewLeaveAttachment(string LNO)
        {
            //var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                var path = await codeUnitWebService.HRWS().GenerateSupportingDocumentLinkAsync(LNO);
                string ext = Path.GetExtension(path.return_value); // getting the file extension of uploaded file  

                var file = path.return_value;

                // Response...
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file,
                    Inline = true // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                //Response.Headers.Add("Content-Disposition", cd.ToString());
                //Response.Headers.Add("X-Content-Type-Options", "nosniff");
                return File(System.IO.File.ReadAllBytes(file), "application/pdf");

                //return ext switch
                //{
                //    //".jpeg" => File(System.IO.File.ReadAllBytes(file), "image/jpeg"),
                //    //".jpg" => File(System.IO.File.ReadAllBytes(file), "image/jpg"),
                //    //".png" => File(System.IO.File.ReadAllBytes(file), "image/png"),
                //    ".pdf" => File(System.IO.File.ReadAllBytes(file), "application/pdf"),
                //    _ => Ok(""),
                //};



            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Attachment View failed " + x.Message });
            }
        }

        //Update Leave Card & Send leave for approval
        [Authorize]
        [HttpPost]
        [Route("uploadleaveform")]
        public async Task<IActionResult> UploadLeaveForm([FromBody] LeaveEndDate leaveEnd)
        {
            try
            {
                var updateRes = await codeUnitWebService.HRWS().ModifyLeaveApplicationAsync(leaveEnd.LeaveAppNo, leaveEnd.LeaveType,
                    leaveEnd.LeaveStartDate, Convert.ToDecimal(leaveEnd.DaysApplied), leaveEnd.RelieverRemark, leaveEnd.RelieverNo);
                //check if the leave has a valid workflow
                var isWorkflowEnabled = await codeUnitWebService.HRWS().CheckLeaveApplicationApprovalWorkflowEnabledAsync(leaveEnd.LeaveAppNo);
                if (isWorkflowEnabled.return_value)
                {
                    //send for approvalonselectleavetype

                    try
                    {
                        var isApproved = await codeUnitWebService.HRWS().SendLeaveApplicationApprovalRequestAPIAsync(leaveEnd.LeaveAppNo);
                        if (isApproved.return_value == "true")
                        {
                            //Does Leave has Extra Days
                            var hasExtradays = await codeUnitWebService.Client().HasLeaveHasExtraDaysAsync(leaveEnd.LeaveAppNo, leaveEnd.LeaveType);
                            var hasExtraDays = leaveEnd.LeaveType == config.Value.LeaveType ? true : false;

                            return StatusCode(StatusCodes.Status200OK, new Response {HasExtraDays= hasExtraDays, Status = "Success", Message = "Your leave application was successfully sent for approval. Once approved, you will receive an email containing your leave details." });
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Leave Application Approval Request Failed." });
                        }
                        //return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = $"Your leave application was successfully sent for approval. Once approved, you will receive an email containing your leave details [sysmes:{isApproved.return_value} ]" });

                    }
                    catch (Exception x)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Leave Application Approval Request Failed: " + x.Message });

                    }

                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Leave Application Approval Workflow is Disabled." });
                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Leave Data Upload Failed: " + x.Message });
            }

        }

        //Own leave cancelling
        [Authorize]
        [Route("usercancelleaveapplication/{leaveno}")]
        [HttpGet]
        public async Task<IActionResult> UserCancelLeaveApplication(string leaveno)
        {
            try
            {
                var resp = await codeUnitWebService.HRWS().CancelLeaveApplicationApprovalRequestAsync(leaveno);

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = resp.return_value.ToString() });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Leave Cancellation Failed: " + x.Message });
            }
        }
        //Get Employee List
        [Authorize]
        [HttpGet]
        [Route("getemployeelist")]
        public async Task<IActionResult> GetEmployeeList()
        {
            try
            {
                List<EmployeeListModel> employeeListModels = new List<EmployeeListModel>();

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

                return Ok(new { employeeListModels });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee List Fetch Failed: " + x.Message });
            }
        }




        /*
         * ************************************************************************************************************
         * 
         * 
         *          APPROVAL MGT SECTION (LEAVES)
         * 
         * ************************************************************************************************************
         */

        //Get the approvees
        [Authorize]
        [HttpGet]
        [Route("getapprovee")]
        public async Task<IActionResult> GetApprovees()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                List<ApproveeList> approvees = new List<ApproveeList>();
                var res = await codeUnitWebService.ApprovalMGT().GetApprovalEntriesAPIAsync(user.EmployeeId, "");
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);
                foreach (var item in resSerial)
                {
                    ApproveeList apee = new ApproveeList
                    {
                        EntryNo = item.EntryNo,
                        TableID = item.TableID,
                        DocumentType = item.DocumentType,
                        DocumentNo = item.DocumentNo,
                        Description = item.Description,
                        SequenceNo = item.SequenceNo,
                        ApprovalCode = item.ApprovalCode,
                        SenderID = item.SenderID,
                        ApproverID = item.ApproverID,
                        Status = item.Status,
                        DateTimeSentforApproval = item.DateTimeSentforApproval,
                        Amount = item.Amount,
                        CurrencyCode = item.CurrencyCode,
                        SenderEmployeeNo = item.SenderEmployeeNo,
                        SenderEmployeeName = item.SenderEmployeeName,
                        ApproverEmployeeNo = item.ApproverEmployeeNo,
                        ApproverEmployeeName = item.ApproverEmployeeName,
                    };
                    approvees.Add(apee);
                }

                return Ok(new { approvees });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Aprovee List Fetch Failed: " + x.Message });
            }
        }

        //Get Approvee Leave
        [Authorize]
        [HttpGet]
        [Route("getapproveeleave/{LAPP}")]
        public async Task<IActionResult> GetApproveeLeave(string LAPP)
        {
            try
            {
                var leaveApplication = await codeUnitWebService.HRWS().GetleavesAsync(LAPP, "");
                dynamic leaveApplicationSerial = JsonConvert.DeserializeObject(leaveApplication.return_value);
                List<LeaveApplicationList> leaveApplications = new List<LeaveApplicationList>();

                foreach (var item in leaveApplicationSerial)
                {
                    LeaveApplicationList lap = new LeaveApplicationList
                    {

                        No = item.No,
                        EmployeeNo = item.EmployeeNo,
                        EmployeeName = item.EmployeeName,
                        LeaveType = item.LeaveType,
                        LeaveStartDate = item.LeaveStartDate,
                        LeaveBalance = item.LeaveBalance,
                        DaysApplied = item.DaysApplied,
                        DaysApproved = item.DaysApproved,
                        LeaveEndDate = item.LeaveEndDate,
                        LeaveReturnDate = item.LeaveReturnDate,
                        ReasonForLeave = item.ReasonForLeave,
                        SubstituteEmployeeNo = item.SubstituteEmployeeNo,
                        SubstituteEmployeeName = item.SubstituteEmployeeName,
                        GlobalDimension1Code = item.GlobalDimension1Code,
                        GlobalDimension2Code = item.GlobalDimension2Code,
                        ShortcutDimension3Code = item.ShortcutDimension3Code,
                        ShortcutDimension4Code = item.ShortcutDimension4Code,
                        ShortcutDimension5Code = item.ShortcutDimension5Code,
                        ShortcutDimension6Code = item.ShortcutDimension6Code,
                        ShortcutDimension7Code = item.ShortcutDimension7Code,
                        ShortcutDimension8Code = item.ShortcutDimension8Code,
                        ResponsibilityCenter = item.ResponsibilityCenter,
                        RejectionComments = item.RejectionComments,
                        Status = item.Status,
                    };
                    leaveApplications.Add(lap);
                }
                return Ok(new { leaveApplications });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Aprovee Leave Fetch Failed: " + x.Message });
            }
        }

        //Approve Approvee Leave
        [Authorize]
        [HttpGet]
        [Route("approveapproveeleave/{LNO}")]
        public async Task<IActionResult> ApproveApproveeLeave(string LNO)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var res = await codeUnitWebService.ApprovalMGT().ApproveLeaveApplicationAsync(user.EmployeeId, LNO);
                if (res.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Approve Leave Application, Success: " });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Approve Leave Application Failed" });
                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Approving Leave Application Failed: " + x.Message });
            }
        }

        //Reject Approvee Leave
        [Authorize]
        [HttpPost]
        [Route("rejectapproveeleave/{LNO}")]
        public async Task<IActionResult> RejectApproveeLeave([FromBody] LeaveEndDate leaveRej, string LNO)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var res = await codeUnitWebService.ApprovalMGT().RejectDocumentWithCommentsAsync(user.EmployeeId, LNO, leaveRej.RejectionRemark);
                if (res.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Reject Leave Application, Success: " });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Reject Leave Application Failed" });
                }
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Reject Leave Application Failed: " + x.Message });
            }
        }

        //Get Employees Per Manager
        [Authorize]
        [HttpGet]
        [Route("getemployeepermanager")]
        public async Task<IActionResult> GetMyEmployees()
        {
            try
            {
                List<ManagerEmployees> managerEmployees = new List<ManagerEmployees>();
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var res = await codeUnitWebService.HRWS().GetEmployeesPerManagerAsync(user.EmployeeId);
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);
                foreach (var item in resSerial)
                {
                    ManagerEmployees me = new ManagerEmployees
                    {
                        EmployeeNo = item.EmployeeNo,
                        SupervisorNo = item.SupervisorNo,
                        EmployeeName = item.EmployeeName,
                        FullNameReliever = item.FullNameReliever,
                        EmploymentContractCode = item.EmploymentContractCode,
                        Gender = item.Gender,
                        GlobalDimension1Code = item.GlobalDimension1Code,
                        GlobalDimension2Code = item.GlobalDimension2Code,
                        ShortcutDimension3Code = item.ShortcutDimension3Code,
                        EmployeeCompanyEmail = item.EmployeeCompanyEmail,
                        JobTitle = item.JobTitle,
                    };
                    managerEmployees.Add(me);
                }
                return Ok(new { managerEmployees });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Fetching Manager Employees Failed: " + x.Message });
            }
        }
        //Get Employee Per Manager
        //[Authorize]
        [HttpGet]
        [Route("getemployeepermananger/{EID}")]
        public async Task<IActionResult> GetEmployee(string EID)
        {
            try
            {
                var res = await codeUnitWebService.HRWS().GetOneEmployeeAsync(EID);
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);
                List<ManagerEmployees> employee = new List<ManagerEmployees>();
                foreach (var tem in resSerial)
                {
                    ManagerEmployees me = new ManagerEmployees
                    {
                        EmployeeNo = tem.EmployeeNo,
                        SupervisorNo = tem.SupervisorNo,
                        EmployeeName = tem.EmployeeName,
                        FullNameReliever = tem.FullNameReliever,
                        EmploymentContractCode = tem.EmploymentContractCode,
                        Gender = tem.Gender,
                        GlobalDimension1Code = tem.GlobalDimension1Code,
                        GlobalDimension2Code = tem.GlobalDimension2Code,
                        ShortcutDimension3Code = tem.ShortcutDimension3Code,
                        EmployeeCompanyEmail = tem.EmployeeCompanyEmail,
                        JobTitle = tem.JobTitle,
                    };
                    employee.Add(me);
                }

                //Employee Consumed Leaves
                List<LeaveApplicationList> usedLeaves = new List<LeaveApplicationList>();
                var usedL = await codeUnitWebService.HRWS().GetleavesAsync("", EID);
                dynamic usedLSerial = JsonConvert.DeserializeObject(usedL.return_value);

                foreach (var item in usedLSerial)
                {
                    LeaveApplicationList usdlapp = new LeaveApplicationList
                    {
                        No = item.No,
                        EmployeeNo = item.EmployeeNo,
                        EmployeeName = item.EmployeeName,
                        LeaveType = item.LeaveType,
                        LeaveStartDate = item.LeaveStartDate,
                        LeaveBalance = item.LeaveBalance,
                        DaysApplied = item.DaysApplied,
                        DaysApproved = item.DaysApproved,
                        LeaveEndDate = item.LeaveEndDate,
                        LeaveReturnDate = item.LeaveReturnDate,
                        ReasonForLeave = item.ReasonForLeave,
                        RejectionComments = item.RejectionComments,
                        SubstituteEmployeeNo = item.SubstituteEmployeeNo,
                        SubstituteEmployeeName = item.SubstituteEmployeeName,
                        Status = item.Status,
                    };
                    usedLeaves.Add(usdlapp);
                }

                //Employee Balance Leave
                List<LeaveTypes> leaveBalance = new List<LeaveTypes>();
                var lBal = await codeUnitWebService.HRWS().GetEmployeeLeaveBalancesAsync(EID);
                dynamic lBalSerial = JsonConvert.DeserializeObject(lBal.return_value);
                foreach (var itm in lBalSerial)
                {
                    LeaveTypes lty = new LeaveTypes
                    {
                        Leavebalance = itm.LeaveBalance,
                        Label = itm.LeaveType,
                    };
                    leaveBalance.Add(lty);

                }


                return Ok(new { employee, usedLeaves, leaveBalance });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Fetching Manager Employee Failed: " + x.Message });
            }
        }

        //Extra Days in Exam Days
        //List of Extra Days
        [HttpGet]
        [Route("getextradays/{LeaveNo}")]
        public async Task<IActionResult> GetExtraDays(string LeaveNo)
        {
            try
            {
               // DateTime datetime = DateTime.ParseExact(auxDate, "MM/dd/yyyy", null);

                List<ExtraDaysList> extraDays = new List<ExtraDaysList>();
                var rez = await codeUnitWebService.Client().GetExtraDaysAsync(LeaveNo);
                dynamic rezSeria = JsonConvert.DeserializeObject(rez.return_value);
                foreach (var item in rezSeria)
                {
                    //var stardate = DateTime.ParseExact(item.Startdate, "MM/dd/yyyy", null);
                    //var enddate = DateTime.ParseExact(item.Enddate, "MM/dd/yyyy", null);
                    //var returdate = DateTime.ParseExact(item.Returndate, "MM/dd/yyyy", null);

                    ExtraDaysList edl = new ExtraDaysList
                    {
                        Leaveno = item.Leaveno,
                        Employeeno = item.Employeeno,
                        Startdate = item.Startdate,
                        Days = item.Days,
                        Enddate = item.Enddate,
                        Returndate = item.Returndate,
                        Assignedleaveno = item.Assignedleave
                    };
                    extraDays.Add(edl);

                }
                return Ok(new { extraDays });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Fetching Extra Days Failed: " + x.Message });
            }
        }

        //Add New Day
        [Authorize]
        [HttpPost]
        [Route("addanextraday")]
        public async Task<IActionResult> AddExtraDay([FromBody] ExtraDaySingle day)
        {
            try
            {
                List<ExtraDaysList> newday = new List<ExtraDaysList>();

                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var res = await codeUnitWebService.Client().AddExtraDaysAsync(day.Leaveno, user.EmployeeId, day.Startdate, day.Days);
                //dynamic resSeri = JsonConvert.DeserializeObject(res.return_value);
                //foreach (var item in resSeri)
                //{
                //    ExtraDaysList edl = new ExtraDaysList
                //    {
                //        Leaveno = item.Leaveno,
                //        Employeeno = item.Employeeno,
                //        Startdate = item.Startdate,
                //        Days = item.Days,
                //        Enddate = item.Enddate,
                //        Returndate = item.Returndate,
                //        Assignedleaveno = item.Assignedleave
                //    };
                //    newday.Add(edl);
                //}
                return Ok(new { res.return_value });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Add Extra Day Failed: " + x.Message });
            }
        }
        //Delete A Day
        [Authorize]
        [HttpPost]
        [Route("deleteday")]
        public async Task<IActionResult> DeleteAday([FromBody] ExtraDaySingle extraDay)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var rs = await codeUnitWebService.Client().DeleteExtraDaysAsync(extraDay.Leaveno, extraDay.Startdate, user.EmployeeId);

                if (rs.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Error", Message = "Remove Extra Day, Success " });

                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Remove Extra Day Failed (DeleteExtraDays return false)" });

                }
              
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Remove Extra Day Failed: " + x.Message });
            }
        }
    
        // Leave Status Dashboard
        [Authorize]
        [HttpPost]
        [Route("leavestatusdashboard")]
        public async Task<IEnumerable<LeaveSubmanager>> LeaveStatusDashboard(LeaveDashboard leaveDashboard)
        {
           
            List<LeaveSubmanager> leaveDashList = new List<LeaveSubmanager>();
            try
            {
                var userModel = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                string EID = userModel.EmployeeId;
                var leaveResult = await codeUnitWebService.Client().GetLeaveDashboardAsync(EID, leaveDashboard.StartDate, leaveDashboard.EndDate);
                //process the data ---
                dynamic leaveSerial = JsonConvert.DeserializeObject<List<LeaveSubmanager>>(leaveResult.return_value);
                //Dictionary<LeaveSubmanager, List<LeaveSubmanagerEmployee>> data = new Dictionary<LeaveSubmanager, List<LeaveSubmanagerEmployee>>();
                //List<LeaveSubmanagerEmployee> lList = new List<LeaveSubmanagerEmployee>();

                //foreach (var item in leaveSerial)
                //{
                //    LeaveSubmanager ls = new LeaveSubmanager();
                //    ls.EmployeeId = item.SubmanagerId;
                //    ls.EmployeeName = item.SubmanagerName;
                //    ls.ManagerId = item.ManagerId;
                //    ls.LeaveType = item.LeaveType;
                //    ls.LeaveDays = item.LeaveDays;
                //    ls.LeaveStart = item.LeaveStart;
                //    ls.LeaveEnd = item.LeaveEnd;

                //    if (item.subRows != null)
                //    {

                //        foreach (var itm in item.subRows)
                //        {
                //            LeaveSubmanagerEmployee lse = new LeaveSubmanagerEmployee();
                //            lse.EmployeeId = itm.EmployeeId;
                //            lse.EmployeeName = itm.EmployeeName;
                //            lse.ManagerId = itm.ManagerId;
                //            lse.LeaveType = itm.LeaveType;
                //            lse.LeaveDays = itm.LeaveDays;
                //            lse.LeaveStart = itm.LeaveStart;
                //            lse.LeaveEnd = itm.LeaveEnd;

                //            lList.Add(lse);


                //        }
                //        data.Add(ls, lList);
                //        continue;
                //    }
                //    else
                //    {
                //        data.Add(ls, null);
                //    }

                //}




                //foreach (var item in leaveSerial)
                //{

                //    LeaveSubmanager ls = new LeaveSubmanager();
                //    ls.EmployeeId = item.SubmanagerId;
                //    ls.EmployeeName = item.SubmanagerName;
                //    ls.ManagerId = item.ManagerId;
                //    ls.LeaveType = item.LeaveType;
                //    ls.LeaveDays = item.LeaveDays;
                //    ls.LeaveStart = item.LeaveStart;
                //    ls.LeaveEnd = item.LeaveEnd;


                //    if (item.subRows != null)
                //    {

                //        foreach (var itm in item.subRows)
                //        {
                //            LeaveSubmanagerEmployee lse = new LeaveSubmanagerEmployee();
                //            lse.EmployeeId = itm.EmployeeId;
                //            lse.EmployeeName = itm.EmployeeName;
                //            lse.ManagerId = itm.ManagerId;
                //            lse.LeaveType = itm.LeaveType;
                //            lse.LeaveDays = itm.LeaveDays;
                //            lse.LeaveStart = itm.LeaveStart;
                //            lse.LeaveEnd = itm.LeaveEnd;

                //            lList.Add(lse);
                //        }

                //        ls.SubRows = lList;
                //        leaveDashList.Add(ls);
                //    }
                //    else
                //    {
                //        leaveDashList.Add(ls);
                //    }



                //}

                return leaveSerial;
                
            }
            catch (Exception)
            {
                return leaveDashList;
                //return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Fetch Leave Status Dashboard Failed: " + x.Message });
            }
        }

       
    }


}
