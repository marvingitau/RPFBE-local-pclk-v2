using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RPFBE.Auth;
using RPFBE.Model;
using RPFBE.Model.DBEntity;
using RPFBE.Model.Repository;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RPFBE.Controllers
{
    //[Authorize(Roles =UserRoles.Admin)]
   
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
     

        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ICodeUnitWebService codeUnitWebService;
        private readonly IMailService mailService;
        private readonly IOptions<WebserviceCreds> config;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;

        public IConfiguration Configuration { get; }

        public HomeController(
             UserManager<ApplicationUser> userManager,
             ApplicationDbContext dbContext,
             ILogger<HomeController> logger,
            IWebHostEnvironment webHostEnvironment,
            ICodeUnitWebService codeUnitWebService,
            IMailService mailService,
            IOptions<WebserviceCreds> config,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration
      )
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            _logger = logger;
            this.webHostEnvironment = webHostEnvironment;
            this.codeUnitWebService = codeUnitWebService;
            this.mailService = mailService;
            this.config = config;
            this.roleManager = roleManager;
            Configuration = configuration;
        }

        [Route("posted-jobs")]
        // public async Task<IActionResult> GetPostedJobs()
        public async Task<IEnumerable<PostedJobModel>> GetPostedJobs()
        {
            List<PostedJobModel> postedJobsList = new List<PostedJobModel>();
            try
            {
                //List<PostedJobModel> postedJobsList = new List<PostedJobModel>();

                var result = await codeUnitWebService.Client().GetPostedJobsAsync();
                dynamic postedJobs = JsonConvert.DeserializeObject<List<PostedJobModel>>(result.return_value);

                foreach (var postedJob in postedJobs)
                {
                    PostedJobModel postedJobModel = new PostedJobModel();
                    postedJobModel.Jobno = postedJob.Jobno;
                    postedJobModel.Jobtitle = postedJob.Jobtitle;
                    postedJobModel.Closingdate = postedJob.Closingdate;
                    postedJobModel.Department = postedJob.Department;
                    postedJobModel.Branch = postedJob.Branch;
                    postedJobModel.Product = postedJob.Product;

                    postedJobsList.Add(postedJobModel);
                }

                //return Content(postedJobs );
                _logger.LogInformation("Viewing Posted Jobs");
                return postedJobs;
            }
            catch (Exception)
            {
                return postedJobsList;
                //return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }

        }

        //Internal Job List
        [Authorize]
        [HttpGet]
        [Route("internal-vacancy")]
        public async Task<IActionResult> InternalVacancy()
        {
            try
            {
                List<PostedJobModel> postedJobsList = new List<PostedJobModel>();

                var result = await codeUnitWebService.Client().GetPostedInternalJobsAsync();
                dynamic postedJobs = JsonConvert.DeserializeObject<List<PostedJobModel>>(result.return_value);

                foreach (var postedJob in postedJobs)
                {
                    PostedJobModel postedJobModel = new PostedJobModel();
                    postedJobModel.Jobno = postedJob.Jobno;
                    postedJobModel.No = postedJob.No;
                    postedJobModel.Jobtitle = postedJob.Jobtitle;
                    postedJobModel.Closingdate = postedJob.Closingdate;
                    postedJobModel.Department = postedJob.Department;
                    postedJobModel.Branch = postedJob.Branch;
                    postedJobModel.Product = postedJob.Product;

                    postedJobsList.Add(postedJobModel);
                }

                //return Content(postedJobs );
                return Ok(new { postedJobsList });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Internal Vacancy Failed: " + x.Message });
            }

        }


        [HttpGet]
        [Route("jobdata/{ReqNo}")]
        public IActionResult GetJobData(string ReqNo)
        {
            List<JobQualificationModel> JobQualifList = new List<JobQualificationModel>();
            List<JobRequirementModel> JobRequireList = new List<JobRequirementModel>();
            List<TasksModel> JobTaskList = new List<TasksModel>();
            List<ChecklistModel> JobCheckList = new List<ChecklistModel>();

            try
            {
                //Job Metadata
                var jobMeta = codeUnitWebService.Client().GetJobMetaAsync(ReqNo).Result.return_value;
                JobMetaModel jobMetaDeserialize = JsonConvert.DeserializeObject<JobMetaModel>(jobMeta);

                try
                {
                    //Qualification
                    var jobQualification = codeUnitWebService.Client().GetPostedJobQualificationsAsync(jobMetaDeserialize.Jobno).Result.return_value;
                    dynamic jobQualificationDeserialize = JsonConvert.DeserializeObject(jobQualification);
                    foreach (var qualif in jobQualificationDeserialize)
                    {
                        JobQualificationModel qualificationModel = new JobQualificationModel
                        {
                            Jobno = qualif.Jobno,
                            Mandantory = qualif.Mandatory,
                            Description = qualif.Description,
                            Qficationcode = qualif.Qficationcode

                        };

                        JobQualifList.Add(qualificationModel);
                    }
                }
                catch (Exception)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Job Qualification deserialize failed" });
                }

                try
                {
                    //Requirements
                    var jobRequirement = codeUnitWebService.Client().GetPostedJobRequirementsAsync(jobMetaDeserialize.Jobno).Result.return_value;
                    dynamic JobReqSerial = JsonConvert.DeserializeObject(jobRequirement);
                    foreach (var require in JobReqSerial)
                    {
                        JobRequirementModel requirementModel = new JobRequirementModel
                        {
                            Jobno = require.Jobno,
                            Mandatory = require.Mandatory,
                            Description = require.Description,
                            Rqmentcode = require.Rqmentcode
                        };
                        JobRequireList.Add(requirementModel);
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Job Requirement deserialize failed" + ex.Message });
                }

                try
                {
                    //Task
                    var jobTask = codeUnitWebService.Client().GetJobTasksAsync(jobMetaDeserialize.Jobno).Result.return_value;
                    dynamic JobTaskSerial = JsonConvert.DeserializeObject(jobTask);

                    foreach (var task in JobTaskSerial)
                    {
                        TasksModel tasksModel = new TasksModel
                        {
                            Jobno = task.Jobno,
                            Description = task.Description,
                            Taskcode = task.Taskcode
                        };
                        JobTaskList.Add(tasksModel);
                    }
                }
                catch (Exception)
                {

                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Job Task deserialize failed" });
                }

                try
                {
                    //Checklist
                    var jobChecklist = codeUnitWebService.Client().GetChecklistAsync(ReqNo).Result.return_value;
                    dynamic jobChecklistSerial = JsonConvert.DeserializeObject(jobChecklist);

                    foreach (var task in jobChecklistSerial)
                    {
                        ChecklistModel checkModel = new ChecklistModel
                        {
                            ReqNo = task.ReqNo,
                            Code = task.DocCode,
                            Description = task.Description
                        };
                        JobCheckList.Add(checkModel);
                    }
                }
                catch (Exception)
                {

                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Job Check List deserialize failed" });
                }




                return Ok(new
                {
                    JobMeta = jobMetaDeserialize,
                    JobQualification = JobQualifList,
                    JobRequirement = JobRequireList,
                    JobTask = JobTaskList,
                    JobCheck = JobCheckList
                });
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Job metadata deserialize failed" });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("applyjob")]
        public async Task<IActionResult> ApplyJob([FromBody] AppliedJob appliedJob)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);


                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"User:{user.Id},Verb:POST,Path:Apply Job Failed");
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Application Failed" });
                }
                appliedJob.UserId = user.Id;
                appliedJob.ApplicationDate = DateTime.Now;
                var profileExist = dbContext.Users.Where(x => x.Id == user.Id && x.ProfileId != 0).Count();
                if (profileExist == 0)
                {
                    _logger.LogInformation($"User:{user.Id},Verb:POST,Path:Please Create your profile first");
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Please Create your profile first" });
                }
                var duplicateCheck = dbContext.AppliedJobs.Where(x => x.UserId == user.Id && x.JobReqNo == appliedJob.JobReqNo ).FirstOrDefault();
                if (duplicateCheck == null)
                {
                    await dbContext.AppliedJobs.AddAsync(appliedJob);
                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation($"User:{user.Id},Verb:POST,Path:Job Applied");
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Job Applied" });

                }

                _logger.LogInformation($"User:{user.Id},Verb:POST,Path:Job Applied Already");
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Job Applied Already" });
            }
            catch (Exception x)
            {
                _logger.LogError($"User:NAp,Verb:POST,Action:Job Application Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Job Application Failed"+x.Message });

            }

        }

        [Authorize]
        [HttpPost]
        [Route("applyinternaljob")]
        public async Task<IActionResult> ApplyInternalJob([FromBody] AppliedJob appliedJob)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);


                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Application Failed" });
                }
                appliedJob.UserId = user.Id;
                appliedJob.ApplicationDate = DateTime.Now;
                var profileExist = dbContext.Users.Where(x => x.Id == user.Id && x.ProfileId != 0).Count();
                if (profileExist == 0)
                {
                    _logger.LogInformation($"User:{user.EmployeeId},Verb:POST,Path:Please Create your profile first");
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Please Create your profile first" });
                }
                var duplicateCheck = dbContext.AppliedJobs.Where(x => x.UserId == user.Id && x.JobReqNo == appliedJob.JobReqNo).FirstOrDefault();
                if (duplicateCheck == null)
                {
                    await dbContext.AppliedJobs.AddAsync(appliedJob);
                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation($"User:{user.EmployeeId},Verb:POST,Path:Job Applied");
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Job Applied" });

                }

                _logger.LogInformation($"User:{user.EmployeeId},Verb:POST,Path:Job Applied Already");
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Job Applied Already" });
            }
            catch (Exception x)
            {
                _logger.LogError($"User:NAp,Verb:POST,Action:Job Application Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Apply Job Failed: "+x.Message });

            }

        }



        //Checklist Documents !(Roles="Admin")
        [Authorize]
        [Route("uploadcheck/{jobNo}")]
        [HttpPost]
        public async Task<IActionResult> SaveImage([FromForm] List<IFormFile> forms, string jobNo)
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
          
            try
            {
                var profileExist = dbContext.Users.Where(x => x.Id == user.Id && x.ProfileId != 0).Count();
                if (profileExist == 0)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Success", Message = "Please Create your profile first" });
                }
                var subDirectory = "Files";
                var result = new List<FileUploadResult>();
                var target = Path.Combine(webHostEnvironment.ContentRootPath, subDirectory);
                var dbCount = dbContext.SpecFiles.Where(x => x.UserId == user.Id && x.JobId == jobNo).Count();
                if (dbCount <= 0)
                {
                    foreach (var file in forms)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName) + Path.GetExtension(file.FileName);
                        //string fileName = new String(Path.GetFileNameWithoutExtension(file.FileName).Take(17).ToArray()).Replace(' ', '-');
                        //fileName = fileName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(file.FileName);
                        var path = Path.Combine(target, fileName);
                        //var stream = new FileStream(path, FileMode.Create);
                        //await file.CopyToAsync(stream);
                        //result.Add(new FileUploadResult() { Name = file.FileName, Length = file.Length });

                        using (FileStream stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            result.Add(new FileUploadResult() { Name = file.FileName, Length = file.Length });
                            JobSpecFile specData = new JobSpecFile
                            {
                                UserId = user.Id,
                                JobId = jobNo,
                                FilePath = path,
                                TagName = fileName,

                            };
                            dbContext.SpecFiles.Add(specData);
                            dbContext.SaveChanges();

                        }

                    }

                    _logger.LogInformation($"User:{user.EmployeeId},Verb:POST,Path:File Uploaded");
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "File Uploaded" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "File Uploaded Already" });
                }
               
            }
            catch (Exception ex)
            {
                _logger.LogError($"User:NAp,Verb:POST,Action:File upload Failed,Message:{ex.Message}");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "File upload failed :"+ex.Message });
                
            }

        }

        //User View Checklist Documents
        [Route("viewchecklist")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobSpecFile>>> ViewChecklist()
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                var dbres = dbContext.SpecFiles.Where(x => x.UserId == user.Id).ToList();
                return dbres;
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "CV View failed" });
            }

        }

        //Admin & use for users so as to delete
        [Route("viewattachment/{FID}/{EID}")]
        [HttpGet]
        public IActionResult ViewAttachment(string FID,string EID)
        {
            //var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                var dbres = dbContext.SpecFiles.Where(x => x.TagName == FID && x.UserId ==EID).First();
                var file = dbres.FilePath;

                /*
                // Response...
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file,
                    Inline = false // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                return File(System.IO.File.ReadAllBytes(file), "application/pdf");
                */
                var stream = new FileStream(file, FileMode.Open);
                return new FileStreamResult(stream, "application/pdf");

                // return Ok(dbres.FilePath);
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "Attachement View failed"+x.Message });
            }
        }



        //Admin View Checklist Documents
        [Route("adminviewchecklist/{UID}")]
        [HttpGet]
        public ActionResult<IEnumerable<JobSpecFile>> AdminViewChecklist(string UID)
        {
            //var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                var dbres = dbContext.SpecFiles.Where(x => x.UserId == UID).ToList();

                return dbres;
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "CV View failed" });
            }

        }

        //User View CV
        [Authorize]
        [Route("viewcv")]
        [HttpGet]
        public async Task<IActionResult> ViewCV()
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                var dbres = dbContext.UserCVs.Where(x => x.UserId == user.Id).FirstOrDefault();

                /*var bytes = await System.IO.File.ReadAllBytesAsync(dbres.FilePath);
                return File(bytes, "application/pdf", Path.GetFileName(dbres.FilePath));
                if (filename == null)
                    return Content("filename not present");

                var path = Path.Combine(
                               Directory.GetCurrentDirectory(),
                               "wwwroot", filename);*/

                //var path = dbres.FilePath;

                //var memory = new MemoryStream();
                //using (var stream = new FileStream(path, FileMode.Open))
                //{
                //    await stream.CopyToAsync(memory);
                //}
                //memory.Position = 0;
                //return File(memory, "application/pdf", Path.GetFileName(path));
                if(dbres != null)
                {

                var file = dbres.FilePath;
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
                else
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "CV Not Found"});
                }




            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "CV View failed" + x.Message });
            }

        }
        //Admin View CV
        [Route("viewcv/{UID}")]
        [HttpGet]
        public IActionResult AdminViewCV(string UID)
        {
            //var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                //var path = System.AppContext.BaseDirectory;
            
                var dbres = dbContext.UserCVs.Where(x => x.UserId == UID).FirstOrDefault();
                if(dbres == null)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "CV Not Uploaded" });
                }
                var file = dbres.FilePath;
              
                // Response...
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file,
                    Inline = true // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                return File(System.IO.File.ReadAllBytes(file), "application/pdf");
                
                /*
                WebClient webclient = new WebClient();
                var byteArr = webclient.DownloadData(file);
                return File(byteArr, "application/pdf");
                */
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "CV View failed" });
            }
        }

        //User Upload the CV
        [Route("uploadcv")]
        [HttpPost]
        public async Task<IActionResult> UploadCV([FromForm] IFormFile formFile)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);


                var subDirectory = "Files/CVs";
                var target = Path.Combine(webHostEnvironment.ContentRootPath, subDirectory);
                string fileName = new String(Path.GetFileNameWithoutExtension(formFile.FileName).Take(10).ToArray()).Replace(' ', '-');
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(formFile.FileName);
                var path = Path.Combine(target, fileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }
                //var stream = new FileStream(path, FileMode.Create);
                //await formFile.CopyToAsync(stream);
                //var respnuk = dbContext.UserCVs.Where(x => x.UserId == user.Id);

                /* 
                 * var host = HttpContext.Request.Host.ToUriComponent();
                 * var url = $"{HttpContext.Request.Scheme}://{host}/{path}";
                 * return Content(url);
                */

                if (dbContext.UserCVs.Where(x => x.UserId == user.Id).Count() > 0)
                {
                    var specificCV = dbContext.UserCVs.Where(x => x.UserId == user.Id).FirstOrDefault();
                    specificCV.FilePath = path;
                    specificCV.TagName = formFile.FileName;
                    await dbContext.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "CV Updated" });

                }
                else
                {
                    UserCV cvData = new UserCV
                    {
                        UserId = user.Id,
                        FilePath = path,
                        TagName = formFile.FileName
                    };
                    dbContext.UserCVs.Add(cvData);
                    dbContext.SaveChanges();
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "CV Uploaded" });
                }

            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = x.Message });
            }
        }


        //HOD Upload justification file
        [Authorize]
        [Route("justificationupload/{reqNo}")]
        [HttpPost]
        public async Task<IActionResult> JustificationFile([FromForm] IFormFile formFile, string reqNo)
        {

            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                var subDirectory = "Files/Reqfiles";
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

                
                if (dbContext.JustificationFiles.Where(x => x.UserId == user.Id && x.ReqNo == reqNo).Count() > 0)
                {
                    var specificFile = dbContext.JustificationFiles.Where(x => x.UserId == user.Id && x.ReqNo == reqNo).FirstOrDefault();
                    specificFile.FilePath = path;
                    specificFile.TagName = formFile.FileName;
                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation($"User:{user.EmployeeId},Verb:POST,Path:Justification File Updated");
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "File Updated" });

                }
                else
                {
                    JustificationFile specData = new JustificationFile
                    {
                        UserId = user.Id,
                        ReqNo = reqNo,
                        FilePath = path,
                        TagName = fileName,

                    };
                    dbContext.JustificationFiles.Add(specData);
                    dbContext.SaveChanges();

                    _logger.LogInformation($"User:{user.EmployeeId},Verb:POST,Path:Justification File upload");
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "File Uploaded" });
                }

            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var verb = Request.HttpContext.Request.Method;
                _logger.LogError($"User:{user.EmployeeId},Verb:{verb},Action:Justification File upload Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = x.Message });
            }


        }

        //Admin View Justification
        [Route("justificationfile/{reqID}")]
        [HttpGet]
        public IActionResult ViewJustificationFile(string reqID)
        {
            //var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                //var path = System.AppContext.BaseDirectory;

                var dbres = dbContext.JustificationFiles.Where(x => x.ReqNo == reqID).FirstOrDefault();
                if (dbres == null)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "File Not Uploaded" });
                }
                var file = dbres.FilePath;

                // Response...
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file,
                    Inline = true // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                return File(System.IO.File.ReadAllBytes(file), "application/pdf");

                /*
                WebClient webclient = new WebClient();
                var byteArr = webclient.DownloadData(file);
                return File(byteArr, "application/pdf");
                */
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "File View failed" });
            }
        }





        [Authorize]
        [Route("appliedjobs")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppliedJob>>> AppliedJobs()
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            return dbContext.AppliedJobs.Where(y => y.UserId == user.Id).Where(x => x.UserId == user.Id).ToList();
        }
        [Authorize]
        [Route("applicants")]
        [HttpGet]
        public ActionResult GetApplicants()
        {
            try
            {
                var query = dbContext.AppliedJobs
                .Join(
                dbContext.Users,
                apps => apps.UserId,
                user => user.Id,
                (apps, user) => new
                {
                UserId = user.Id,
                Viewed = apps.Viewed,
                Title = apps.JobTitle,
                Name = user.Name,
                AppDate = apps.ApplicationDate,
                ReqNo = apps.JobReqNo,
                Reject = apps.Rejected

                }
                ).Where(x => x.Viewed != true).ToList();

                return Ok(query);
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Applicants fetche failed " + x.Message });
            }
     
        }
        [Authorize]
        [Route("approvedapplicants")]
        [HttpGet]
        public ActionResult GetApprovedApplicants()
        {
            var query = dbContext.AppliedJobs
            .Join(
                dbContext.Users,
                apps => apps.UserId,
                user => user.Id,
                (apps, user) => new
                {
                    UserId = user.Id,
                    Viewed = apps.Viewed,
                    Title = apps.JobTitle,
                    Name = user.Name,
                    AppDate = apps.ApplicationDate,
                    ReqNo = apps.JobReqNo,
                    AppNo = apps.JobAppplicationNo,
                    EmpNo = apps.EmpNo,
                    Reject = apps.Rejected
                }
                ).Where(x => x.Viewed == true ).ToList();//&& x.EmpNo != ""

            return Ok(query);
        }

        //Reject Pending Applicants
        [Authorize]
        [HttpGet]
        [Route("rejectpendingapplicant")]
        public async Task<IActionResult> RejectPendingApplicant()
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var verb = Request.HttpContext.Request.Method;
            try
            {
                var rejects = dbContext.AppliedJobs
                .Join( dbContext.Users,
                    apps => apps.UserId,
                    user => user.Id,
                    (apps, user) => new {
                        Email = user.Email,
                        Username = user.UserName,
                        Viewed = apps.Viewed,
                        Rejected = apps.Rejected,
                        JobTitle = apps.JobTitle
                      
                    }
                 ).Where(x => x.Viewed == false && x.Rejected == "FALSE").ToList();


                //Emails Section
                foreach (var item in rejects)
                {
                    //if(item.Email != null)
                    //{
                    await codeUnitWebService.WSMailer().RejectedRegretAlertAsync(item.Email, item.Username, item.JobTitle);
                   // }
                }
                var res = dbContext.AppliedJobs.Where(x => x.Viewed == false && x.Rejected =="FALSE").ToList();
                foreach (var item in res)
                {
                    item.Rejected = "TRUE";
                }
                dbContext.UpdateRange(res);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation($"User:{user.EmployeeId},Verb:{verb},Path:Reject Pending Applicant(s) [{String.Join(",", rejects.Select(x=>x.Email) )}] Success");

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Reject Pending Applicant Success" });


            }
            catch (Exception x)
            {
                _logger.LogWarning($"User:{user.EmployeeId},Verb:{verb},Path:Reject Pending Applicant(s) Failed:"+x.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Reject Pending Applicant(s) failed: " + x.Message });
            }
        }

        //View Single Applicant i.e the CV
        //[Authorize]
        [Route("applicant/{id}")]
        [HttpGet]
        public ActionResult GetApplicant(string id)
        {
            var usr = dbContext.Users.Find(id);
            if (usr.ProfileId != 0)
            {
                var profile = dbContext.Profiles.Where(x => x.UserId == usr.Id);
                return Ok(new { profile, usr });
            }
            return Ok(usr);
        }

        // Create ERP PostJobApplication
        // Needs the Requision and Employee Code
        [Route("postjob/{reqNo}")]
        [HttpGet]
        public async Task<IActionResult> PushJobApp(string reqNo)
        {

            var employee = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var jobAppId = codeUnitWebService.Client().PostJobApplicationAsync(reqNo, employee.EmployeeId);
            return Ok(jobAppId);
        }

        //Admin Dashboad
        [Authorize]
        [Route("adminstats")]
        [HttpGet]

        public IActionResult adminstats()
        {
            try
            {
                var viewedCount = dbContext.AppliedJobs.Where(x => x.Viewed != true).Count();
                var pendingCount = dbContext.AppliedJobs.Where(x => x.Viewed == true).Count();

                return Ok(new { viewedCount, pendingCount });
            }
            catch (Exception)
            {
                var viewedCount = 0;
                var pendingCount = 0;
                return Ok(new { viewedCount, pendingCount });

            }
        }

        [Authorize]
        [Route("userstats")]
        [HttpGet]

        public async Task<IActionResult> userstats()
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            try
            {
                var viewedCount = dbContext.AppliedJobs.Where(x => x.UserId == user.Id && x.Viewed == true).Count();
                var pendingCount = dbContext.AppliedJobs.Where(x => x.UserId == user.Id && x.Viewed != true).Count();

                return Ok(new { viewedCount, pendingCount });
            }
            catch (Exception)
            {
                var viewedCount = 0;
                var pendingCount = 0;
                return Ok(new { viewedCount, pendingCount });

            }
        }

        [Route("env")]
        [HttpGet]
        public IActionResult GetDefault()
        {
            //string wwwPath = this.webHostEnvironment.WebRootPath;
            //string contentPath = this.webHostEnvironment.ContentRootPath;
            //return Ok(new { wwwPath, contentPath });
            WebClient webclient = new WebClient();
            var byteArr = webclient.DownloadData(@"A:\CODES\VS\RecruitmentPortalFolder\RPFBE\Files/CVs\Marvingita224125779.pdf");
                //await wc.DownloadDataTaskAsync(fileURL);
            return File(byteArr, "application/pdf");

           // var stream = new FileStream(@"A:\CODES\VS\RecruitmentPortalFolder\RPFBE\Files/CVs\Marvingita224125779.pdf", FileMode.Open);
            //return new FileStreamResult(stream, "application/pdf");
        }

        [Route("getcv/{UID}")]
        [HttpGet]
        public IActionResult GetGlobalCV(string UID)
        {
            try
            {
                //var path = System.AppContext.BaseDirectory;
                var dbres = dbContext.UserCVs.Where(x => x.UserId == UID).First();
                var file = dbres.FilePath;

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
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "CV View failed" });
            }
        }

        //Admin
        [Route("getspec/{tag}/{UID}")]
        [HttpGet]
        public IActionResult GetGlobalSpec(string UID,string tag)
        {
            try
            {
                //var path = System.AppContext.BaseDirectory;
                var dbres = dbContext.SpecFiles.Where(x => x.TagName == tag && x.UserId == UID).First();
                var file = dbres.FilePath;

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

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "Supporting Doc View failed: "+x.Message });
            }
        }


        //Get Excel doc
        //Admin
        [Authorize]
        [Route("getexcel/{title}")]
        [HttpGet]
        public IActionResult getExcel(string title)
        {
            try
            {
                //var results = dbContext.AppliedJobs.Where(x => x.JobTitle == title).ToList();
                List<AppliedJob> appliedJobs= dbContext.AppliedJobs.ToList();
                List<Profile> profiles = dbContext.Profiles.ToList();
                List<JobSpecFile> jobSpecFiles = dbContext.SpecFiles.ToList();

                var query = from appjob in appliedJobs
                            join prof in profiles on appjob.UserId equals prof.UserId into tbl1
                            //from t1 in tbl1.ToList()
                            join jspec in jobSpecFiles on appjob.UserId equals jspec.UserId into tbl2
                            //from t2 in tbl2.ToList()
                            select new { appliedJobs = appjob, profiles = tbl1, jobSpecFiles = tbl2 };
                var results = query.Where(x => x.appliedJobs.JobTitle == title && x.appliedJobs.Viewed == false).ToList();

                //return Ok(results);
                /*
                var builder = new StringBuilder();
                builder.AppendLine("Job,UID,Resume");

                foreach (var result in results)
                {
                   //  = HYPERLINK("{HttpContext.Request.Host.ToUriComponent()}", "" / api / home / getcv / "")
                    builder.AppendLine($"{result.JobTitle},{result.UserId},=HYPERLINK({HttpContext.Request.Host.ToUriComponent()}/api/home/getcv/{result.UserId})");
                }

                return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "users.csv");

                //return Ok("dd")
                */

                
                //Excel Document
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Candidates");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "Job";
                    worksheet.Cell(currentRow, 2).Value = "First Name";
                    worksheet.Cell(currentRow, 3).Value = "Last Name";
                    worksheet.Cell(currentRow, 4).Value = "Application Date";
                    worksheet.Cell(currentRow, 5).Value = "Date of Birth";
                    worksheet.Cell(currentRow, 6).Value = "Country";
                    worksheet.Cell(currentRow, 7).Value = "Expected Salary";
                    worksheet.Cell(currentRow, 8).Value = "Current Salary";
                    worksheet.Cell(currentRow, 9).Value = "Highest Education Level";
                    worksheet.Cell(currentRow, 10).Value = "Willing to relocate";
                    worksheet.Cell(currentRow, 11).Value = "Gender";
                    worksheet.Cell(currentRow, 12).Value = "Disabled";
                    worksheet.Cell(currentRow, 13).Value = "Professional Experience (Y)";
                    worksheet.Cell(currentRow, 14).Value = "Resume";
                    worksheet.Cell(currentRow, 15).Value = "Other Qualification";

                    // From worksheet
                    //var rngTable = workbook.Range("1A:1K");
                    //rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    //rngTable.Style.Font.Bold = true;
                    //rngTable.Style.Fill.BackgroundColor = XLColor.Aqua;

                    foreach (var r in results)
                    {
                        char[] delimiterChars = { '-', 'T' };
                        string text = r.profiles.FirstOrDefault().DOB;

                        string[] words = text.Split(delimiterChars);
                        string auxDate = words[1] + "/" + words[2] + "/" + words[0];

                        DateTime datetime = DateTime.ParseExact(auxDate, "MM/dd/yyyy", null);

                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = r.appliedJobs.JobTitle;
                        worksheet.Cell(currentRow, 2).Value = r.profiles.FirstOrDefault() != null ? r.profiles.FirstOrDefault().FirstName : "";
                        worksheet.Cell(currentRow, 3).Value = r.profiles.FirstOrDefault() != null ? r.profiles.FirstOrDefault().LastName : "";
                        worksheet.Cell(currentRow, 4).Value = r.appliedJobs.ApplicationDate;
                        worksheet.Cell(currentRow, 5).Value = auxDate;
                        worksheet.Cell(currentRow, 6).Value = r.profiles.FirstOrDefault() != null ? r.profiles.FirstOrDefault().Country : "";
                        worksheet.Cell(currentRow, 7).Value = r.profiles.FirstOrDefault() != null ? r.profiles.FirstOrDefault().ExpectedSalary: "";
                        worksheet.Cell(currentRow, 8).Value = r.profiles.FirstOrDefault() != null ? r.profiles.FirstOrDefault().CurrentSalary: "";
                        worksheet.Cell(currentRow, 9).Value = r.profiles.FirstOrDefault() != null ? r.profiles.FirstOrDefault().HighestEducation: "";
                        worksheet.Cell(currentRow, 10).Value = r.profiles.FirstOrDefault() != null ? r.profiles.FirstOrDefault().WillingtoRelocate : "";
                        worksheet.Cell(currentRow, 11).Value = r.profiles.FirstOrDefault() != null ? r.profiles.FirstOrDefault().Gender : "";
                        worksheet.Cell(currentRow, 12).Value = r.profiles.FirstOrDefault() != null ? r.profiles.FirstOrDefault().PersonWithDisability : "";
                        worksheet.Cell(currentRow, 13).Value = r.profiles.FirstOrDefault() != null ? r.profiles.FirstOrDefault().Experience : "";
                        worksheet.Cell(currentRow, 14).Value = "Resume";
                        //worksheet.Cell(currentRow, 14).Hyperlink = new XLHyperlink($"{HttpContext.Request.Host.ToUriComponent()}/api/home/getcv/{r.appliedJobs.UserId}", "Click to Open CV!");
                        worksheet.Cell(currentRow, 14).Hyperlink = new XLHyperlink($"{config.Value.ExcelHostUrl}/home/getcv/{r.appliedJobs.UserId}", $"{config.Value.ExcelHostUrl}/home/getcv/{r.appliedJobs.UserId}");
                        foreach(var spec in r.jobSpecFiles)
                        {
                            worksheet.Cell(currentRow, 15).Value = spec.TagName;
                            //worksheet.Cell(currentRow, 15).Hyperlink = new XLHyperlink($"{HttpContext.Request.Host.ToUriComponent()}/api/home/getspec/{spec.TagName}", "Click to Open");
                           // worksheet.Cell(currentRow, 15).Hyperlink = new XLHyperlink($"{config.Value.ExcelHostUrl}/home/getspec/{spec.TagName}", "Click to Open");
                            worksheet.Cell(currentRow, 15).Hyperlink = new XLHyperlink($"{config.Value.ExcelHostUrl}/home/getspec/{spec.TagName}/{spec.UserId}", $"{config.Value.ExcelHostUrl}/home/getspec/{spec.TagName}/{spec.UserId}");
                            currentRow++;
                        }
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "users.xlsx");
                    }
                }
                
                
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "Excel download failed "+x.Message });
            }
           
        }
        /*
      * *************************************************************************************************************
      * 
      * 
      * 
      *                      PASSWORD RESET LOGIC FOR IDENTTY FRAMEWORK.
      * 
      * 
      * 
      **************************************************************************************************************** 
      */
        [HttpGet]
        [Route("reset/{email}")]
        //Get token
        public async Task<IActionResult> RequestPasswordResetLink(string email)
        {
            try
            {
                var user = dbContext.Users.Where(x => x.Email == email).FirstOrDefault();
                string host = HttpContext.Request.Host.ToUriComponent();
                string protocol = HttpContext.Request.Scheme;

                if (user != null)
                {
                    var code = await userManager.GeneratePasswordResetTokenAsync(user);

                    var link = $"{code}";
                    //var link = $"{protocol}/{host}/forgot?id={code}";

                    try
                    {
                        // await mailService.SendEmailPasswordReset(user.Email, link);
                        await codeUnitWebService.WSMailer().SendEmailPasswordResetAsync(user.Email, link);

                        _logger.LogInformation($"User:{user.EmployeeId},Verb:POST,Path:Password Token sent");
                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Token Mailed" });
                    }
                    catch (Exception x)
                    {
                        _logger.LogError($"User:NAp,Verb:GET,Action:Reset Link email failed,Message:{x.Message}");
                        return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "Reset Link email failed: " + x.Message });
                    }
                    //logger.LogInformation($"An password reset email was sent to {user.Email}");
                }
                else
                {
                    _logger.LogError($"User:NAp,Verb:GET,Action:User Not Found");
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "User not found" });
                }
            }
            catch (Exception x)
            {
                _logger.LogError($"User:NAp,Verb:GET,Action:Password Reset Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "User not found"+x.Message });

            }
          

        }

        //Use Token
        [HttpPost]
        [Route("forgotten")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] ForgottenModel forgottenModel)
        {
            try
            {
                var user = dbContext.Users.Where(x => x.Email == forgottenModel.Email).FirstOrDefault();
                var resetPassResult = await userManager.ResetPasswordAsync(user, forgottenModel.Token, forgottenModel.Password);

                var isEmployeeExist = await codeUnitWebService.EmployeeAccount().EmployeeExistsAsync(user.EmployeeId);
                if (isEmployeeExist.return_value)
                {
                    var isTokenSet = await codeUnitWebService.EmployeeAccount().ResetEmployeePortalPasswordAsync(user.EmployeeId, Cryptography.Hash(forgottenModel.Password));
                    if (resetPassResult.Succeeded && isTokenSet.return_value)
                    {
                        _logger.LogInformation($"User:{user.Id},Verb:POST,Path:Passord reset Success");
                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Passord reset Success" });
                    }
                    else
                    {
                        _logger.LogWarning($"User:{user.Id},Verb:POST,Path:Password reset failed ");
                        return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "Password reset failed " });

                    }
                }
                else
                {
                    if (resetPassResult.Succeeded)
                    {
                        _logger.LogInformation($"User:{user.Id},Verb:POST,Path:Passord reset Success Employee Doesnt Exist");
                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Passord reset Success" });
                    }
                    else
                    {
                        _logger.LogWarning($"User:{user.Id},Verb:POST,Path:Password reset failed Employee Doesnt Exist");
                        return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "Password reset failed " });

                    }
                }

            }
            catch (Exception x)
            {
                _logger.LogError($"User:NAp,Verb:GET,Action:Password Reset Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "Password reset failed "+x.Message });

            }
        }


        // HOD upload Monitoring Support Documents

        [Authorize]
        [Route("hoduploadmonitoringdocs/{ID}")]
        [HttpPost]
        public async Task<IActionResult> HODUploadMonitoringDocs([FromForm] IFormFile formFile, string ID)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);


                var subDirectory = "Files/Monitoring";
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
                    await dbContext.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Monitoring Doc Updated" });

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
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Monitoring Doc Uploaded" });
                }

            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = x.Message });
            }
        }


        //HR
        [Route("getmonitoring/{PID}")]
        [HttpGet]
        public IActionResult GetMonitoringDoc(string PID)
        {
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

                return File(System.IO.File.ReadAllBytes(file), "application/pdf");
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Supporting Doc View failed: "+x.Message });
            }
        }

        //HR Activate Exit interview
        [Authorize]
        [HttpGet]
        [Route("createexitinterview")]

        public async Task<IActionResult> CreateExitInterview()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                List<EmployeeListModel> employeeListModels = new List<EmployeeListModel>();
                List<EmployeeListModel> separationGrounds = new List<EmployeeListModel>();

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


                var resGround = await codeUnitWebService.Client().GetGroundsForSeparationAsync();
                dynamic resGroundSerial = JsonConvert.DeserializeObject(resGround.return_value);


                foreach (var gr in resGroundSerial)
                {
                    EmployeeListModel e2 = new EmployeeListModel
                    {
                        Value = gr.Value,
                        Label = gr.Label,
                    };
                    separationGrounds.Add(e2);

                }
                _logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Path:Get Exit Form Source Data Sucess");
                return Ok(new { employeeListModels, separationGrounds });


            }
            catch (Exception x)
            {
                _logger.LogError($"User:NAp,Verb:GET,Action:Exit source data missing,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Exit source data missing:"+x.Message });
            }
        }

        //Post Exit Interview Card/ Push to Employee
        [Authorize]
        [HttpPost]
        [Route("postexitinterview")]

        public async Task<IActionResult> PostExitInterview([FromBody] ExitInterviewCard interviewCard) 
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);


                List<ExitInterviewCard> responseCard =new  List<ExitInterviewCard>();

                var res = await codeUnitWebService.Client().CreateExitInterviewAsync(
                    interviewCard.EID,
                    interviewCard.InterviewDate,
                    interviewCard.Interviewer,
                    interviewCard.SeparationGround,
                    interviewCard.OtherReason,
                    interviewCard.SeparationDate,
                    interviewCard.Reemploy
                    );

                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);
                var latestEntryModel = dbContext.ExitInterviewCard.Where(x => x.EID == interviewCard.EID).FirstOrDefault();

                foreach (var item in resSerial)
                {
                    ExitInterviewCard interviewCard1 = new ExitInterviewCard
                    {
                        ExitNo = item.ExitNo,
                        EmployeeName = item.EmployeeName,
                        JobTitle = item.JobTitle,
                        Division = item.DivisionUnit,
                        StartDateWithOrganization = item.StartDateWithOrganization,
                        PositionStartDate = item.PositionStartDate,
                        SeparationDate = item.SeparationDate,
                        LengthOfService = item.LengthOfService,
                        OtherPositionsHeld = item.OtherPositionsHeld,

                    };

                    responseCard.Add(interviewCard1);

                    if (latestEntryModel != null)
                    {
                        //update
                        latestEntryModel.ExitNo = interviewCard1.ExitNo;
                        latestEntryModel.EmployeeName = interviewCard1.EmployeeName;
                        latestEntryModel.JobTitle = interviewCard1.JobTitle;
                        latestEntryModel.Division = interviewCard1.Division;
                        latestEntryModel.StartDateWithOrganization = interviewCard1.StartDateWithOrganization;
                        latestEntryModel.PositionStartDate = interviewCard1.PositionStartDate;
                        latestEntryModel.SeparationDate = interviewCard1.SeparationDate;
                        latestEntryModel.LengthOfService = interviewCard1.LengthOfService;
                        latestEntryModel.OtherPositionsHeld = interviewCard1.OtherPositionsHeld;
                        latestEntryModel.FormUploaded = 0;


                        dbContext.ExitInterviewCard.Update(latestEntryModel);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        interviewCard.ExitNo = interviewCard1.ExitNo;
                        interviewCard.EmployeeName = interviewCard1.EmployeeName;
                        interviewCard.JobTitle = interviewCard1.JobTitle;
                        interviewCard.Division = interviewCard1.Division;
                        interviewCard.StartDateWithOrganization = interviewCard1.StartDateWithOrganization;
                        interviewCard.PositionStartDate = interviewCard1.PositionStartDate;
                        interviewCard.SeparationDate = interviewCard1.SeparationDate;
                        interviewCard.LengthOfService = interviewCard1.LengthOfService;
                        interviewCard.OtherPositionsHeld = interviewCard1.OtherPositionsHeld;
                        interviewCard.UID = user.Id;

                        dbContext.ExitInterviewCard.Add(interviewCard);
                        await dbContext.SaveChangesAsync();
                    }

                }
                //Mail Employee.
                var sendEmpMail = await codeUnitWebService.WSMailer().ExitInterviewAsync(interviewCard.EID);

                _logger.LogInformation($"User:{user.EmployeeId},Verb:POST,Path:Exit data upload Sucess");
                return Ok(new { interviewCard });
            }
            catch (Exception x)
            {
                _logger.LogError($"User:NAp,Verb:GET,Action:Exit source data missing,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Exit data upload failed: " +x.Message });
            }
        }


        //Get the Count of Exit interview with Status 1 [filed by employee]

        [Authorize]
        [HttpGet]
        [Route("getpushedexitinterview")]
        public IActionResult GetPushedExitInterview()
        {
            try
            {
                //get the status 1
                var exitCards = dbContext.ExitInterviewCard.Where(x => x.FormUploaded >= 1).ToList();
                return Ok(exitCards);
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Exit data upload failed" + x.Message });
            }
        }

        //HR Approve the Interview Form
        [Authorize]
        [HttpGet]
        [Route("hrapproveexitform/{PK}")]

        public async Task<IActionResult> HRApproveExitForm(string PK)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var res = await codeUnitWebService.Client().ApproveInterviewFormAsync(PK);
                if(res.return_value == "TRUE")
                {
                    var rec = dbContext.ExitInterviewCard.Where(x => x.ExitNo == PK).FirstOrDefault();
                    rec.FormUploaded = 2;
                    dbContext.ExitInterviewCard.Update(rec);
                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Path:Exit Form Approved");

                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Exit Form/Card Updated" });
                }
                else
                {
                    _logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Path:Exit Form Approval Failed");
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Exit Form/Card Update Failed" });
                }
            }
            catch (Exception x)
            {
                _logger.LogError($"User:NAp,Verb:GET,Action:Exit Form Approval Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Exit data upload failed" + x.Message });
            }
        }

        //HR Get the list of Users
        [Authorize]
        [HttpGet]
        [Route("hrallusers")]

        public IActionResult HRGetAllUsers()
        {
            try
            {
                var allUsers = dbContext.Users.ToList();
                return Ok(new { allUsers });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = " All Users failed" + x.Message });
            }
        }

        //HR Get the list of Job Seekers
        [Authorize]
        [HttpGet]
        [Route("hralljobseekers")]

        public IActionResult HRGetAllJobseeker()
        {
            try
            {
                var allUsers = dbContext.Users.Where(x => x.EmployeeId == null).ToList();
                return Ok(new { allUsers });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = " All Jobseekers failed" + x.Message });
            }
        }
        //HR delete all Jobseekers
        [Authorize]
        [HttpGet]
        [Route("hrdeletealljobseekers")]
        public async Task<IActionResult> DeleteAllJobseekers()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var jseekrs = dbContext.Users.Where(x => x.EmployeeId == null).ToList();
                dbContext.Users.RemoveRange(jseekrs);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Path:HR Delete All Job Seekers Success");
                return Ok("Job seekers Deleted");

            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                _logger.LogError($"User:{user.EmployeeId},Verb:GET,Action:HR Delete All Job Seekers Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Jobseekers Deletion failed" + x.Message });
            }
        }

        //HR User Roles
        [HttpGet]
        [Route("getuserroles")]
        public async Task<IActionResult> Getuserroles()
        {
            try
            {
                List<BankModel> userRoles = new List<BankModel>();
                var res =await  codeUnitWebService.Client().GetUserRolesAsync();
                dynamic resS = JsonConvert.DeserializeObject(res.return_value);

                foreach (var ur in resS)
                {
                    BankModel ess = new BankModel
                    {
                        Label = ur.Label,
                        Value = ur.Value,
                    };
                    userRoles.Add(ess);

                }
                return Ok(new { userRoles });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Role failed" + x.Message });
            }
        }


        //Update Roles
        [Authorize]
        [Route("updaterole/{uid}/{val}")]
        [HttpGet]

        public async Task<IActionResult> HRUpdateProfile(string uid, string val)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var usr = dbContext.Users.Where(x => x.EmployeeId == uid).First();

                //var roless = await userManager.GetRolesAsync(usr);
                if (await userManager.IsInRoleAsync(usr, "NORMAL"))
                {

                    await userManager.RemoveFromRoleAsync(usr, "NORMAL");
                    if (!await roleManager.RoleExistsAsync(val))
                        await roleManager.CreateAsync(new IdentityRole(val));

                    await userManager.AddToRoleAsync(usr, val);
                }

                var rolls = await userManager.GetRolesAsync(usr);
                foreach (var item in rolls)
                {
                    await userManager.RemoveFromRoleAsync(usr, item);
                }
               
                if (!await roleManager.RoleExistsAsync(val)){
                    await roleManager.CreateAsync(new IdentityRole(val));
                    await userManager.AddToRoleAsync(usr, val);
                }
                else
                {
                    await userManager.AddToRoleAsync(usr, val);
                }

             
                usr.Rank = val;
                dbContext.Users.Update(usr);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation($"User:{user.EmployeeId},Verb:GET,Path:User Role Updated to {val}");

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "User Role Updated" });
            }
            catch (Exception x)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                _logger.LogError($"User:{user.EmployeeId},Verb:GET,Action:HR User Role Update failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Role Update failed" + x.Message });
            }
        }


        //HR create Clearance
        [Authorize]
        [HttpGet]
        [Route("hrcreateclearance")]
        public async Task<IActionResult> HRCreateClearance()
        {
            try
            {
                //Employees 
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

                List<BankModel> userRoles = new List<BankModel>();
                var res = await codeUnitWebService.Client().GetUserRolesAsync();
                dynamic resS = JsonConvert.DeserializeObject(res.return_value);


                foreach (var ur in resS)
                {
                    BankModel ess = new BankModel
                    {
                        Label = ur.Label,
                        Value = ur.Value,
                    };
                    userRoles.Add(ess);

                }

                return Ok(new { employeeListModels, userRoles });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Emp Clearance Create Failed: " + x.Message });
            }
        }

        //HR Post Clearance
        [Authorize]
        [HttpPost]
        [Route("hrstoreclearance")]
        public async Task<IActionResult> HRStoreClearance([FromBody] EmployeeClearance employeeClearance)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                //char[] delimiterChars = { '-', 'T' };
                //string text = employeeClearance.LastEmployeeDate;

                //string[] words = text.Split(delimiterChars);
                //string auxDate = words[1] + "/" + words[2] + "/" + words[0];

                //DateTime datetime = DateTime.ParseExact(auxDate, "MM/dd/yyyy", null);

                //No Workflow

                var pFlag = 1;
                var sFlag = 1;
                if (employeeClearance.SelectedRole == "HOD")
                {
                    pFlag = 1;
                    sFlag = 1;
                }
                   
                if (employeeClearance.SelectedRole == "HOD-ADMIN")
                {
                    pFlag = 1;//2;
                    sFlag = 2;
                    //Since this is Employee Own Dept,prefill the own dept so HR can approve when the Rest of dept hods approves
                    //employeeClearance.HODApproved = "TRUE";
                    //employeeClearance.HODApprovedName = user.Name;
                    //employeeClearance.HODApprovedUID = user.Id;

                }
                   

                if (employeeClearance.SelectedRole == "HOD-IT")
                {
                    pFlag = 1;// 3;
                    sFlag = 3;
                    //Since this is Employee Own Dept,prefill the own dept so HR can approve when the Rest of dept hods approves
                    //employeeClearance.HODApproved = "TRUE";
                    //employeeClearance.HODApprovedName = user.Name;
                    //employeeClearance.HODApprovedUID = user.Id;
                }
                   

                if (employeeClearance.SelectedRole == "HOD-HR")
                {
                    pFlag = 1;// 4;
                    sFlag = 4;
                    //Since this is Employee Own Dept,prefill the own dept so HR can approve when the Rest of dept hods approves
                    //employeeClearance.HODApproved = "TRUE";
                    //employeeClearance.HODApprovedName = user.Name;
                    //employeeClearance.HODApprovedUID = user.Id;
                }

                if (employeeClearance.SelectedRole == "HOD-FIN")
                {
                    pFlag = 1;// 5;
                    sFlag = 5;
                    //Since this is Employee Own Dept,prefill the own dept so HR can approve when the Rest of dept hods approves
                    //employeeClearance.HODApproved = "TRUE";
                    //employeeClearance.HODApprovedName = user.Name;
                    //employeeClearance.HODApprovedUID = user.Id;
                }


                employeeClearance.UID = user.Id;
                employeeClearance.ProgressFlag = pFlag;
                employeeClearance.ProgressStartFlag = sFlag;

                var evt = dbContext.EmployeeClearance.Add(employeeClearance);
                await dbContext.SaveChangesAsync();

                var rId = employeeClearance.Id;
                List<EmployeeClearance> employeeClearancesList = new List<EmployeeClearance>();
                var empClearanceUpdate =await codeUnitWebService.Client().CreateClearanceAsync(employeeClearance.EmpID, employeeClearance.LastEmployeeDate);
                dynamic empSerial = JsonConvert.DeserializeObject(empClearanceUpdate.return_value);

                foreach (var ec in empSerial)
                {
                    EmployeeClearance eC = new EmployeeClearance
                    {
                        ClearanceNo = ec.Clearanceno,
                        EmpID = ec.Empno,
                        EmpName = ec.Empname
                    };

                    var latestRecModel = dbContext.EmployeeClearance.Where(r => r.Id == rId).FirstOrDefault();

                    latestRecModel.ClearanceNo = ec.Clearanceno;
                    latestRecModel.EmpID = ec.Empno;
                    latestRecModel.EmpName = ec.Empname;

                    dbContext.EmployeeClearance.Update(latestRecModel);
                    await dbContext.SaveChangesAsync();

                    employeeClearancesList.Add(latestRecModel);
                }
                return Ok(new { employeeClearancesList });

                
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee Clearance Post Failed: " + x.Message });
            }
        }

        //HR Clearance List
        [Authorize]
        [HttpGet]
        [Route("hrgetclearancelist")]
        public IActionResult HRGetClearanceList()
        {
            try
            {
                var clearanceList = dbContext.EmployeeClearance.ToList();
                return Ok(new { clearanceList });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee Clearance List Failed: " + x.Message });
            }
        }


        //HOD Update
        [Authorize]
        [HttpGet]
        [Route("hodupdateclearancerecord/{PK}")]

        public async Task<IActionResult> HODUpdateClearanceLine(int PK)
        {
            try
            {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var resUp = dbContext.EmployeeClearance.Where(x => x.Id == PK).FirstOrDefault();

               
                if (resUp.ProgressFlag == resUp.ProgressStartFlag)
                {
                    //resUp.ProgressFlag = 2;
                    resUp.HODApproved = "TRUE";
                    resUp.HODApprovedUID = user.Id;
                    resUp.HODApprovedName = user.Name;

                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();

                    ///Mail the HR
                    ///

                    return Ok(resUp.ProgressFlag);
                }
                else
                {

                    // resUp.ProgressFlag = resUp.ProgressStartFlag == 2 ? 3 : 2;  @free of workflow
                    resUp.HODApproved = "TRUE";
                    resUp.HODApprovedUID = user.Id;
                    resUp.HODApprovedName = user.Name;

                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();

                    return Ok(resUp.ProgressFlag);
                }


            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD Clearance List Update Failed: " + x.Message });
            }
        }

        //HOD-ADMIN Update
        [Authorize]
        [HttpGet]
        [Route("hodadminupdateclearancerecord/{PK}")]

        public async Task<IActionResult> HODADMINUpdateClearanceLine(int PK)
        {
            try
            {
                var user =await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var resUp = dbContext.EmployeeClearance.Where(x => x.Id == PK).FirstOrDefault();


                /*if (resUp.ProgressFlag == resUp.ProgressStartFlag)
                {
                    //resUp.ProgressFlag = 2;
                    resUp.HODAdminApproved = "TRUE";
                    resUp.HODAdminApprovedName = user.Name;
                    resUp.HODAdminApprovedUID = user.Id;

                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();

                    return Ok(resUp.ProgressFlag);
                }
                else
                {
                    resUp.HODAdminApproved = "TRUE";
                    resUp.HODAdminApprovedName = user.Name;
                    resUp.HODAdminApprovedUID = user.Id;
                    //resUp.ProgressFlag = resUp.ProgressStartFlag == 3 ? 4 : 3; //
                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();

                    return Ok(resUp.ProgressFlag);
                }*/


                // If Start origin is the Administration Dept 
                // Approve both HODAdmin & HOD in the ESS db.
                if ( resUp.ProgressStartFlag == 2)
                {
                    resUp.HODAdminApproved = "TRUE";
                    resUp.HODAdminApprovedName = user.Name;
                    resUp.HODAdminApprovedUID = user.Id;

                    resUp.HODApproved = "TRUE";
                    resUp.HODApprovedName = user.Name;
                    resUp.HODApprovedUID = user.Id;

                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();

                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "HOD-ADMIN Clearance List Update, Succes " });

                }
                else
                {
                    resUp.HODAdminApproved = "TRUE";
                    resUp.HODAdminApprovedName = user.Name;
                    resUp.HODAdminApprovedUID = user.Id;

                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();

                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "HOD-ADMIN Clearance List Update, Succes " });

                }


            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD-ADMIN Clearance List Update Failed: " + x.Message });
            }
        }

        //HOD-ICT Update
        [Authorize]
        [HttpGet]
        [Route("hodictupdateclearancerecord/{PK}")]

        public async Task<IActionResult> HODICTUpdateClearanceLine(int PK)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var resUp = dbContext.EmployeeClearance.Where(x => x.Id == PK).FirstOrDefault();

                /*if (resUp.ProgressFlag == resUp.ProgressStartFlag)
                {
                    //resUp.ProgressFlag = 2;
                    resUp.HODITApproved = "TRUE";
                    resUp.HODITApprovedName = user.Name;
                    resUp.HODITApprovedUID = user.Id;

                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();

                    return Ok(resUp.ProgressFlag);
                }
                else
                {

                    //resUp.ProgressFlag = resUp.ProgressStartFlag == 4 ? 5 : 4; //
                    resUp.HODITApproved = "TRUE";
                    resUp.HODITApprovedName = user.Name;
                    resUp.HODITApprovedUID = user.Id;

                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();

                    return Ok(resUp.ProgressFlag);
                }*/

                // If Start origin is the Administration Dept 
                // Approve both HODICT & HOD in the ESS db.
                if (resUp.ProgressStartFlag == 3)
                {
                    resUp.HODITApproved = "TRUE";
                    resUp.HODITApprovedName = user.Name;
                    resUp.HODITApprovedUID = user.Id;

                    resUp.HODApproved = "TRUE";
                    resUp.HODApprovedName = user.Name;
                    resUp.HODApprovedUID = user.Id;

                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();

                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "HOD-ICT Clearance List Update, Succes " });

                }
                else
                {
                    resUp.HODITApproved = "TRUE";
                    resUp.HODITApprovedName = user.Name;
                    resUp.HODITApprovedUID = user.Id;

                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();

                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "HOD-ICT Clearance List Update, Succes " });

                }



            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD-ICT Clearance List Update Failed: " + x.Message });
            }
        }

        //HOD-HR Update
        [Authorize]
        [HttpGet]
        [Route("hodhrupdateclearancerecord/{PK}")]

        public async Task<IActionResult> HODHRUpdateClearanceLine(int PK)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var resUp = dbContext.EmployeeClearance.Where(x => x.Id == PK).FirstOrDefault();


                if (resUp.ProgressFlag == resUp.ProgressStartFlag)
                {
                    //resUp.ProgressFlag = 2;
                    resUp.HODHRApproved = "TRUE";
                    resUp.HODHRApprovedName = user.Name;
                    resUp.HODHRApprovedUID = user.Id;

                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();

                    //@email
                    //Mail HR


                    return Ok(resUp.ProgressFlag);
                }
                else
                {

                    // resUp.ProgressFlag = resUp.ProgressStartFlag == 5 ? 6 : 5; //
                    resUp.HODHRApproved = "TRUE";
                    resUp.HODHRApprovedName = user.Name;
                    resUp.HODHRApprovedUID = user.Id;

                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();

                    return Ok(resUp.ProgressFlag);
                }


            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD-HR Clearance List Push Failed: " + x.Message });
            }
        }

        //HOD-FIN Update; Push to HR
        [Authorize]
        [HttpGet]
        [Route("hodfinupdateclearancerecord/{PK}")]

        public async Task<IActionResult> HODFINUpdateClearanceLine(int PK)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var resUp = dbContext.EmployeeClearance.Where(x => x.Id == PK).FirstOrDefault();
                if (resUp.ProgressFlag == resUp.ProgressStartFlag)
                {
                    //resUp.ProgressFlag = 2;
                    resUp.HODFINApproved = "TRUE";
                    resUp.HODFINApprovedName = user.Name;
                    resUp.HODFINApprovedUID = user.Id;

                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();

                    return Ok(resUp.ProgressFlag);
                }
                else
                {

                    //resUp.ProgressFlag = resUp.ProgressStartFlag == 6 ? 7 : 6; //
                    resUp.HODFINApproved = "TRUE";
                    resUp.HODFINApprovedName = user.Name;
                    resUp.HODFINApprovedUID = user.Id;

                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();

                    return Ok(resUp.ProgressFlag);
                }


            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD-HR Clearance List Update Failed: " + x.Message });
            }
        }

        //HR Pending Approval
        [Authorize]
        [HttpGet]
        [Route("hrpendingclearancerecord/{PK}")]

        public async Task<IActionResult> HRPendingClearanceLine(int PK)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var resUp = dbContext.EmployeeClearance.Where(x => x.Id == PK).FirstOrDefault();
                //Approve in D365
                var apprveRes = await codeUnitWebService.Client().PendingClearanceAsync(resUp.ClearanceNo);
                if (resUp.ProgressFlag == resUp.ProgressStartFlag)
                {
                    //resUp.ProgressFlag = 2;
                    resUp.HRApproved = "FALSE";
                    resUp.HRApprovedName = user.Name;
                    resUp.HRApprovedUID = user.Id;

                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();

                    //Mail Client
                    // Fun(StaffNo)

                    return Ok(apprveRes.return_value);
                }
                else
                {

                    //resUp.ProgressFlag = resUp.ProgressStartFlag == 7 ? 8 : 7; //
                    resUp.HRApproved = "FALSE";
                    resUp.HRApprovedName = user.Name;
                    resUp.HRApprovedUID = user.Id;

                    dbContext.EmployeeClearance.Update(resUp);
                    await dbContext.SaveChangesAsync();


                    return Ok(apprveRes.return_value);
                }


            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HR Clearance List Update Failed: " + x.Message });
            }
        }

        //HR Clearance Approval an Generation of Final Dues
        [Authorize]
        [HttpGet]
        [Route("hrapprovingclearancerecord/{PK}")]
        public async Task<IActionResult> HRApprovingClearanceLine(int PK)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var resUp = dbContext.EmployeeClearance.Where(x => x.Id == PK).FirstOrDefault();
                //Approve in D365
                //Mail Client
                // Fun(StaffNo)
                var apprveRes = await codeUnitWebService.Client().ApproveClearanceAsync(resUp.ClearanceNo);
                //resUp.ProgressFlag = 2;
                resUp.HRApproved = "TRUE";
                resUp.HRApprovedName = user.Name;
                resUp.HRApprovedUID = user.Id;
                dbContext.EmployeeClearance.Update(resUp);
                await dbContext.SaveChangesAsync();

              


                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "HR Clearance Approval Success " });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HR Clearance Approval Failed: " + x.Message });
            }
        }





        //HOD-FIN Get Respective Lists
        [Authorize]
        [HttpGet]
        [Route("hodfingetrespectivelist")]
        public IActionResult HODFINGetRespectiveList()
        {
            try
            {
                //@from 5
                var clisthod = dbContext.EmployeeClearance.Where(x => x.ProgressFlag == 1 && x.ClearanceNo != null && x.ClearanceNo != "").ToList();
                return Ok(new { clisthod });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD-HR Clearance List Failed: " + x.Message });
            }
        }

        //HOD-HR Get Respective Lists
        [Authorize]
        [HttpGet]
        [Route("hodhrgetrespectivelist")]
        public IActionResult HODHRGetRespectiveList()
        {
            try
            {
                //@from 4
                var clisthod = dbContext.EmployeeClearance.Where(x => x.ProgressFlag == 1 && x.ClearanceNo != null && x.ClearanceNo != "").ToList();
                return Ok(new { clisthod });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD-HR Clearance List Failed: " + x.Message });
            }
        }


        //HOD-IT Get Respective Lists
        [Authorize]
        [HttpGet]
        [Route("hoditgetrespectivelist")]
        public IActionResult HODITGetRespectiveList()
        {
            try
            {
                //@from 3
                var clisthod = dbContext.EmployeeClearance.Where(x => x.ProgressFlag == 1 && x.ClearanceNo != null && x.ClearanceNo != "").ToList();
                return Ok(new { clisthod });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD-HR Clearance List Failed: " + x.Message });
            }
        }

        //HOD Get Respective Lists
        [Authorize]
        [HttpGet]
        [Route("hodgetrespectivelist")]
        public IActionResult HODGetRespectiveList()
        {
            try
            {
                //No workflow do a second flag for HOD activity from 2
                var clisthod = dbContext.EmployeeClearance.Where(x => x.ProgressFlag == 1 && x.ProgressStartFlag == 1 && x.ClearanceNo != null && x.ClearanceNo != "").ToList();
                return Ok(new { clisthod });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD Clearance List Failed: " + x.Message });
            }
        }



        //HOD-FIN Card Data
        [Authorize]
        [HttpGet]
        [Route("hodfingetcardclearancedata/{PK}")]
        public async Task<IActionResult> HODFINGetCardClearance(string PK)
        {
            try
            {
                List<ClearanceList> clearanceFullFormAdmin = new List<ClearanceList>();
                var resEMP = await codeUnitWebService.Client().GetClearancefullformFINANCEAsync(PK);
                dynamic resSerial = JsonConvert.DeserializeObject(resEMP.return_value);
                foreach (var cl in resSerial)
                {
                    ClearanceList cf = new ClearanceList
                    {
                        //Id = cl.Clearanceno,
                        Clearanceno = cl.Clearanceno,
                        Lineno = cl.Lineno,
                        Dept = cl.Dept,
                        Items = cl.Items,
                        Clearance = cl.Clearance,
                        Remarks = cl.Remarks,
                        Kalue = cl.Value,
                        Clearedby = cl.Clearedby,
                        Designation = cl.Designation,

                        StaffLoan = cl.Staffloan,
                        OtherLoan = cl.Otherloan,
                        JitSavings = cl.Jitsavings,
                        AccountantOne = cl.Accountantone,
                        NameOne= cl.Accountantonename,
                        AccountantTwo = cl.Accountanttwo,
                        NameTwo = cl.Accountanttwoname,
                        FinanceManager = cl.Finanancemanager,
                        FinanceManagerName = cl.Finanancemanagername,
                        FinanceDirector = cl.Financedirector,
                        FinanceDirectorName = cl.Financedirectorname,


                    };

                    clearanceFullFormAdmin.Add(cf);
                }



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

                return Ok(new { clearanceFullFormAdmin, employeeListModels });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD Clearance Card Failed: " + x.Message });
            }
        }


        //HOD-FIN Push the Dataline
        [Authorize]
        [HttpPost]
        [Route("hodfinpushclearancelines")]
        public async Task<IActionResult> HODFINPushClearanceLine([FromBody] ClearanceList clearanceList)
        {
            try
            {
                var resClearance = await codeUnitWebService.Client().InsertClearanceLineFinanceAsync(
                    clearanceList.Clearanceno, clearanceList.Clearance,decimal.Parse(clearanceList.StaffLoan),
                    decimal.Parse(clearanceList.OtherLoan), clearanceList.AccountantOne, clearanceList.AccountantTwo,
                    clearanceList.FinanceManager, clearanceList.FinanceDirector,clearanceList.Dept, decimal.Parse(clearanceList.JitSavings)
                    );

                return Ok(resClearance.return_value);
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD Finance Clearance Line Failed: " + x.Message });
            }
        }

        //HOD-FIN Modify Line
        [Authorize]
        [HttpPost]
        [Route("hodfinmodifyclearanceline")]
        public async Task<IActionResult> HODFINModifyClearance([FromBody] ClearanceList clearanceList)
        {
            try
            {
                var resModif = await codeUnitWebService.Client().ModifyClearanceLineFinanceAsync(int.Parse(clearanceList.Lineno),
                    clearanceList.Clearance, decimal.Parse(clearanceList.StaffLoan), decimal.Parse(clearanceList.OtherLoan), clearanceList.AccountantOne,
                    clearanceList.AccountantTwo, clearanceList.FinanceManager, clearanceList.FinanceDirector, decimal.Parse(clearanceList.JitSavings));

                return Ok(resModif.return_value);
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD Clearance Modify Line Failed: " + x.Message });
            }
        }

 

      




        //HOD-HR Card Data
        [Authorize]
        [HttpGet]
        [Route("hodhrgetcardclearancedata/{PK}")]
        public async Task<IActionResult> HODHRGetCardClearance(string PK)
        {
            try
            {
                List<ClearanceList> clearanceFullFormAdmin = new List<ClearanceList>();
                var resEMP = await codeUnitWebService.Client().GetClearancefullformHRAsync(PK);
                dynamic resSerial = JsonConvert.DeserializeObject(resEMP.return_value);
                foreach (var cl in resSerial)
                {
                    ClearanceList cf = new ClearanceList
                    {
                        //Id = cl.Clearanceno,
                        Clearanceno = cl.Clearanceno,
                        Lineno = cl.Lineno,
                        Dept = cl.Dept,
                        Items = cl.Items,
                        Clearance = cl.Clearance,
                        Remarks = cl.Remarks,
                        Kalue = cl.Value,
                        Clearedby = cl.Clearedby,
                        Designation = cl.Designation,

                        Year = cl.Year,
                        AnnualLeaveDays = cl.Annualleavedays,
                        AnnualDaysLess=cl.Annualdaysless,
                        BalDays=cl.Baldays,


                    };

                    clearanceFullFormAdmin.Add(cf);
                }

                return Ok(new { clearanceFullFormAdmin });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD Clearance Card Failed: " + x.Message });
            }
        }

        //HOD-IT Card Data
        [Authorize]
        [HttpGet]
        [Route("hoditgetcardclearancedata/{PK}")]
        public async Task<IActionResult> HODITGetCardClearance(string PK)
        {
            try
            {
                List<ClearanceList> clearanceFullFormAdmin = new List<ClearanceList>();
                var resEMP = await codeUnitWebService.Client().GetClearancefullformICTAsync(PK);
                dynamic resSerial = JsonConvert.DeserializeObject(resEMP.return_value);
                foreach (var cl in resSerial)
                {
                    ClearanceList cf = new ClearanceList
                    {
                        //Id = cl.Clearanceno,
                        Clearanceno = cl.Clearanceno,
                        Lineno = cl.Lineno,
                        Dept = cl.Dept,
                        Items = cl.Items,
                        Clearance = cl.Clearance,
                        Remarks = cl.Remarks,
                        Kalue = cl.Value,
                        Clearedby = cl.Clearedby,
                        Designation = cl.Designation
                    };

                    clearanceFullFormAdmin.Add(cf);
                }

                return Ok(new { clearanceFullFormAdmin });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD Clearance Card Failed: " + x.Message });
            }
        }




        //HOD-ADMIN Get Respective Lists
        [Authorize]
        [HttpGet]
        [Route("hodadmingetrespectivelist")]
        public IActionResult HODADMINGetRespectiveList()
        {
            try
            {
                //@from 2
                var clisthod = dbContext.EmployeeClearance.Where(x => x.ProgressFlag == 1).ToList();
                return Ok(new { clisthod });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD-ADMIN Clearance List Failed: " + x.Message });
            }
        }

        //HOD-ADMIN Card Data
        [Authorize]
        [HttpGet]
        [Route("hodadmingetcardclearancedata/{PK}")]
        public async Task<IActionResult> HODAdminGetCardClearance(string PK)
        {
            try
            {
                List<ClearanceList> clearanceFullFormAdmin = new List<ClearanceList>();
                var resEMP = await codeUnitWebService.Client().GetClearancefullformADMINAsync(PK);
                dynamic resSerial = JsonConvert.DeserializeObject(resEMP.return_value);
                foreach (var cl in resSerial)
                {
                    ClearanceList cf = new ClearanceList
                    {
                        //Id = cl.Clearanceno,
                        Clearanceno = cl.Clearanceno,
                        Lineno = cl.Lineno,
                        Dept = cl.Dept,
                        Items = cl.Items,
                        Clearance = cl.Clearance,
                        Remarks = cl.Remarks,
                        Kalue = cl.Value,
                        Clearedby = cl.Clearedby,
                        Designation = cl.Designation
                    };

                    clearanceFullFormAdmin.Add(cf);
                }

                List<ClearanceList> clearanceFullFormEmployee = new List<ClearanceList>();
                var approvedCount = dbContext.EmployeeClearance.Where(x => x.HODApproved != "TRUE" && x.ClearanceNo==PK).Count();
                if (approvedCount > 0)
                {
                    var xx = await codeUnitWebService.Client().GetClearancefullformEmployeeAsync(PK);
                    dynamic xxSer = JsonConvert.DeserializeObject(xx.return_value);
                    foreach (var cl in xxSer)
                    {
                        ClearanceList cf = new ClearanceList
                        {
                            //Id = cl.Clearanceno,
                            Clearanceno = cl.Clearanceno,
                            Lineno = cl.Lineno,
                            Dept = cl.Dept,
                            Items = cl.Items,
                            Clearance = cl.Clearance,
                            Remarks = cl.Remarks,
                            Kalue = cl.Value,
                            Clearedby = cl.Clearedby,
                            Designation = cl.Designation
                        };

                        clearanceFullFormEmployee.Add(cf);
                    }
                }
               


                return Ok(new { clearanceFullFormAdmin, clearanceFullFormEmployee });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD Clearance Card Failed: " + x.Message });
            }
        }


        //HOD Card Data
        [Authorize]
        [HttpGet]
        [Route("hodgetcardclearancedata/{PK}")]
        public async Task<IActionResult> HODGetCardClearance(string PK)
        {
            try
            {
                List<ClearanceList> clearanceFullFormEmployee = new List<ClearanceList>();
                var resEMP = await codeUnitWebService.Client().GetClearancefullformEmployeeAsync(PK);
                dynamic resSerial = JsonConvert.DeserializeObject(resEMP.return_value);
                foreach (var cl in resSerial)
                {
                    ClearanceList cf = new ClearanceList
                    {
                        //Id = cl.Clearanceno,
                        Clearanceno = cl.Clearanceno,
                        Lineno = cl.Lineno,
                        Dept = cl.Dept,
                        Items = cl.Items,
                        Clearance = cl.Clearance,
                        Remarks = cl.Remarks,
                        Kalue = cl.Value,
                        Clearedby = cl.Clearedby,
                        Designation = cl.Designation
                    };

                    clearanceFullFormEmployee.Add(cf);
                }

                return Ok(new { clearanceFullFormEmployee });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD Clearance Card Failed: " + x.Message });
            }
        }

        //HOD Push the Dataline
        [Authorize]
        [HttpPost]
        [Route("hodpushclearancelines")]
        public async Task<IActionResult> HODPushClearanceLine([FromBody] ClearanceList clearanceList)
        {
            try
            {
                var resClearance = await codeUnitWebService.Client().InsertClearanceLinesAsync(
                    clearanceList.Items,
                    int.Parse(clearanceList.Kalue),
                    clearanceList.Remarks,
                    clearanceList.Dept,
                    clearanceList.Clearance,
                    clearanceList.Clearanceno
                    );

                return Ok(resClearance.return_value);
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD Clearance Line Failed: " + x.Message });
            }
        }

        //HOD-HR Push/Create the Dataline
        [Authorize]
        [HttpPost]
        [Route("hodhrpushclearancelines")]
        public async Task<IActionResult> HODHRPushClearanceLine([FromBody] ClearanceList clearanceList)
        {
            try
            {
                var resClearance = await codeUnitWebService.Client().InsertClearanceLinesHRAsync(
                    clearanceList.Year,
                   decimal.Parse(clearanceList.AnnualLeaveDays),
                    decimal.Parse(clearanceList.AnnualDaysLess),
                    decimal.Parse(clearanceList.BalDays),
                    clearanceList.Remarks,
                    clearanceList.Clearance,
                    clearanceList.Dept,
                    clearanceList.Clearanceno
                    );

                return Ok(resClearance.return_value);
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD-HR Clearance Line Failed: " + x.Message });
            }
        }

        //HOD-HR Update the Dataline
        [Authorize]
        [HttpPost]
        [Route("hodhrupdateclearanceline")]
        public async Task<IActionResult> HODHRUpdateClearanceLine([FromBody] ClearanceList clearanceList)
        {
            try
            {
                var resClearance = await codeUnitWebService.Client().ModifyClearanceLinesHRAsync(
                clearanceList.Year,
                decimal.Parse(clearanceList.AnnualLeaveDays),
                decimal.Parse(clearanceList.AnnualDaysLess),
                decimal.Parse(clearanceList.BalDays),
                decimal.Parse(clearanceList.Kalue),
                clearanceList.Remarks,
                int.Parse(clearanceList.Lineno),
                clearanceList.Clearance
                );
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "HOD-HR Clearance  Line Update, Success " });
                //return Ok(resClearance.return_value);
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD-HR Clearance Line Failed: " + x.Message });
            }
        }



        //Delete line
        [Authorize]
        [HttpGet]
        [Route("hoddeleteline/{lineno}")]
        public async Task<IActionResult> HODDeleteLine(int lineno)
        {
            try
            {
                var res = await codeUnitWebService.Client().DeleteClearanceLineAsync(lineno);
                return Ok(res.return_value);
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD Clearance Delete Line Failed: " + x.Message });
            }
        }

        //Modify Line
        [Authorize]
        [HttpPost]
        [Route("hodmodifyclearanceline")]
        public async Task<IActionResult> HODModifyClearance([FromBody] ClearanceList clearanceList)
        {
            try
            {
                var resModif = await codeUnitWebService.Client().ModifyClearanceLinesAsync(
                    int.Parse(clearanceList.Lineno), clearanceList.Items, int.Parse(clearanceList.Kalue), clearanceList.Remarks,
                    clearanceList.Clearance
                    );

                return Ok(resModif.return_value);
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "HOD Clearance Modify Line Failed: " + x.Message });
            }
        }



        //HR Get Clearnace Card All Data
        [Authorize]
        [HttpGet]
        [Route("getclearancelist/{CID}")]

        public async Task<IActionResult> GetClearanceList(string CID)
        {
            try
            {

                //For HR to approve all HOD must have approved
                var hrApproveIndicator = dbContext.EmployeeClearance.Where(x => x.ClearanceNo == CID && x.HODApproved == "TRUE"
                && x.HODAdminApproved == "TRUE" && x.HODITApproved == "TRUE" && x.HODHRApproved == "TRUE" && x.HODFINApproved == "TRUE"
                && x.HRApproved =="FALSE"
                ).Count();


                List<ClearanceList> clearanceFullFormEmployee = new List<ClearanceList>();
                var resEMP = await codeUnitWebService.Client().GetClearancefullformEmployeeAsync(CID);
                dynamic resSerial = JsonConvert.DeserializeObject(resEMP.return_value);
                foreach (var cl in resSerial)
                {
                    ClearanceList cf = new ClearanceList
                    {
                        Clearanceno = cl.Clearanceno,
                        Lineno = cl.Lineno,
                        Dept = cl.Dept,
                        Items = cl.Items,
                        Clearance = cl.Clearance,
                        Remarks = cl.Remarks,
                        Value = cl.Value,
                        Clearedby = cl.Clearedby,
                        Designation= cl.Designation
                    };

                    clearanceFullFormEmployee.Add(cf);
                }

                List<ClearanceList> clearanceFullFormICT = new List<ClearanceList>();
                var resICT = await codeUnitWebService.Client().GetClearancefullformICTAsync(CID);

                dynamic resSerialICT = JsonConvert.DeserializeObject(resICT.return_value);
                foreach (var cll in resSerialICT)
                {
                    ClearanceList cf = new ClearanceList
                    {
                        Clearanceno = cll.Clearanceno,
                        Lineno = cll.Lineno,
                        Dept = cll.Dept,
                        Items = cll.Items,
                        Clearance = cll.Clearance,
                        Remarks = cll.Remarks,
                        Value = cll.Value,
                        Clearedby = cll.Clearedby,
                        Designation = cll.Designation
                    };

                    clearanceFullFormICT.Add(cf);
                }

                List<ClearanceList> clearanceFullFormFIN = new List<ClearanceList>();
                var resFIN = await codeUnitWebService.Client().GetClearancefullformFINANCEAsync(CID);

                dynamic resSerialFIN = JsonConvert.DeserializeObject(resFIN.return_value);
                foreach (var cll in resSerialFIN)
                {
                    ClearanceList cf = new ClearanceList
                    {
                        Clearanceno = cll.Clearanceno,
                        Lineno = cll.Lineno,
                        Dept = cll.Dept,
                        Items = cll.Items,
                        Clearance = cll.Clearance,
                        Remarks = cll.Remarks,
                        Value = cll.Value,
                        Clearedby = cll.Clearedby,
                        Designation = cll.Designation,

                        StaffLoan = cll.Staffloan,
                        OtherLoan = cll.Otherloan,
                        JitSavings = cll.Jitsavings,
                        AccountantOne = cll.Accountantone,
                        NameOne = cll.Accountantonename,
                        AccountantTwo = cll.Accountanttwo,
                        NameTwo = cll.Accountanttwoname,
                        FinanceManager = cll.Finanancemanager,
                        FinanceManagerName = cll.Finanancemanagername,
                        FinanceDirector = cll.Financedirector,
                        FinanceDirectorName = cll.Financedirectorname

                    };

                    clearanceFullFormFIN.Add(cf);
                }

                List<ClearanceList> clearanceFullFormHR = new List<ClearanceList>();
                var resHR = await codeUnitWebService.Client().GetClearancefullformHRAsync(CID);

                dynamic resSerialHR = JsonConvert.DeserializeObject(resHR.return_value);
                foreach (var cllq in resSerialHR)
                {
                    ClearanceList cf = new ClearanceList
                    {
                        Clearanceno = cllq.Clearanceno,
                        Lineno = cllq.Lineno,
                        Dept = cllq.Dept,
                        Items = cllq.Items,
                        Clearance = cllq.Clearance,
                        Remarks = cllq.Remarks,
                        Value = cllq.Value,
                        Clearedby = cllq.Clearedby,
                        Designation = cllq.Designation,

                        Year = cllq.Year,
                        AnnualLeaveDays = cllq.Annualleavedays,
                        AnnualDaysLess = cllq.Annualdaysless,
                        BalDays = cllq.Baldays,

                    };

                    clearanceFullFormHR.Add(cf);
                }

                List<ClearanceList> clearanceFullFormADMIN = new List<ClearanceList>();
                var resADMIN = await codeUnitWebService.Client().GetClearancefullformADMINAsync(CID);

                dynamic resSerialADMIN = JsonConvert.DeserializeObject(resADMIN.return_value);
                foreach (var kcllq in resSerialADMIN)
                {
                    ClearanceList cf = new ClearanceList
                    {
                        Clearanceno = kcllq.Clearanceno,
                        Lineno = kcllq.Lineno,
                        Dept = kcllq.Dept,
                        Items = kcllq.Items,
                        Clearance = kcllq.Clearance,
                        Remarks = kcllq.Remarks,
                        Value = kcllq.Value,
                        Clearedby = kcllq.Clearedby,
                        Designation = kcllq.Designation,

                      

                    };

                    clearanceFullFormADMIN.Add(cf);
                }




                return Ok(new { clearanceFullFormEmployee, clearanceFullFormICT, clearanceFullFormFIN, clearanceFullFormHR, clearanceFullFormADMIN, hrApproveIndicator });

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Clearance Card Data Failed: " + x.Message });
            }
        }
        
        [HttpGet]
        [Route("users")]
        public IActionResult GetUsers()
        {
            try
            {
                //😀 😃 😄 😁 😆
                var users = dbContext.Users.Count();
                var verb = Request.HttpContext.Request.Method;
                return Ok(new { users });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Login Err: " + x.Message });

            }
        }

    }
}
