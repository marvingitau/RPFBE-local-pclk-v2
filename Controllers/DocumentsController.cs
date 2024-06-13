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
using RPFBE.Model.DocumentModels;
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
    public class DocumentsController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<HomeController> logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ICodeUnitWebService codeUnitWebService;
        private readonly IMailService mailService;
        private readonly IOptions<WebserviceCreds> config;

        public DocumentsController(
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

        //Get Employee Documents en filter according to payroll
        [Authorize]
        [HttpGet]
        [Route("employeedocuments")]
        public async Task<IActionResult> EmployeeDocuments()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                List<DocumentListModel> documentListAll = new List<DocumentListModel>();
                List<DocumentListModel> documentList = new List<DocumentListModel>();
                if (user.Payrollcode == null)
                {
                    

                    var uRes = await codeUnitWebService.Client().GetUserPayrollDataAsync(user.EmployeeId);
                    dynamic uResSerial = JsonConvert.DeserializeObject(uRes.return_value);

                    foreach (var item in uResSerial)
                    {
                        UserModel umodel = new UserModel
                        {
                            Payrollcode = item.Payrollcode,
                            Calculationscheme = item.Calculationschema
                        };
                        user.Calculationscheme = item.Calculationschema;
                        user.Payrollcode = item.Payrollcode;
                        dbContext.Users.Update(user);
                        await dbContext.SaveChangesAsync();
                    }

                    var documentRes = await codeUnitWebService.HRWS().GetEmployeeDocumentsListAsync(user.EmployeeId);
                    dynamic docOutSerialIn = JsonConvert.DeserializeObject(documentRes.return_value);
                    foreach (var item in docOutSerialIn)
                    {
                        DocumentListModel dlm = new DocumentListModel
                        {
                            EmployeeNo = item.EmployeeNo,
                            DocumentCode = item.DocumentCode,
                            DocumentName = item.DocumentName,
                            Read = item.Read,
                            DateTimeRead = item.DateTimeRead,
                            Payrollcode = item.Payrollcode,
                            URL = item.URL,

                        };
                        documentListAll.Add(dlm);

                    }
                    documentList = documentListAll.Where(x => x.Payrollcode == user.Payrollcode || x.Payrollcode == "").ToList();
                    return Ok(new { documentList });
                }
                else
                {
                    var documentResOut = await codeUnitWebService.HRWS().GetEmployeeDocumentsListAsync(user.EmployeeId);
                    dynamic docOutSerial = JsonConvert.DeserializeObject(documentResOut.return_value);
                    foreach (var item in docOutSerial)
                    {
                        DocumentListModel dlm = new DocumentListModel
                        {
                            EmployeeNo = item.EmployeeNo,
                            DocumentCode = item.DocumentCode,
                            DocumentName = item.DocumentName,
                            Read = item.Read,
                            DateTimeRead = item.DateTimeRead,
                            Payrollcode = item.Payrollcode,
                            URL = item.URL

                        };
                        documentListAll.Add(dlm);

                    }
                    documentList = documentListAll.Where(x => x.Payrollcode == user.Payrollcode || x.Payrollcode == "").ToList();
                    return Ok(new { documentList });
                }
                
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Documents Fetch failed: " + x.Message });
            }
        }

        //Read Employee Document
        [Authorize]
        [HttpPost]
        [Route("reademployeedocument")]
        public async Task<IActionResult> ReadEmployeeDocument([FromBody] DocumentModel document)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var file = document.Path;

                //byte[] b = System.IO.File.ReadAllBytes(file);
                //return  Convert.ToBase64String(b);

            //// Response...
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

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Document Fetch failed: " + x.Message });
            }
        }

        //Mark as Read Document
        [Authorize]
        [HttpGet]
        [Route("viewedemployeedocument/{EID}/{DID}")]
        public async Task<IActionResult> ViewedEmployeeDocument(string EID,int DID)
        {
            try
            {
                var readStatus = await codeUnitWebService.HRWS().SignEmployeeDocumentsAsync(EID, DID);
                if (readStatus.return_value)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Succes", Message = "Checked the Document D365" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Document Read check failed D365" });
                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Document Read check failed: " + x.Message });
            }
        }

        //Get document list
    
        [Authorize]
        [HttpGet]
        [Route("getdocumentlist")]
        public async Task<IActionResult> GetDocumentList()
        {
            try
            {
                List<HRDocList> docLists = new List<HRDocList>();
                var li = await codeUnitWebService.Client().HRDocsListAsync();
                dynamic liSerial = JsonConvert.DeserializeObject(li.return_value);
                foreach (var item in liSerial)
                {

                    HRDocList hRDoc = new HRDocList
                    {
                        Value = item.Value,
                        Label = item.Label
                    };
                    docLists.Add(hRDoc);
                }

                var selectedDocument = dbContext.DocumentSetting.FirstOrDefault();

                return Ok(new { docLists, selectedDocument });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get document list failed: " + x.Message });
            }
        }
        //Get the set Value in DB ---- DEPRECATED-----
        [Authorize]
        [HttpGet]
        [Route("getsetdocumentlistvalue")]
        public IActionResult GetSetDocListValue()
        {
            try
            {
                var selectedDocument = dbContext.DocumentSetting.First();
                return Ok(new { selectedDocument.ReadMandatory });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get selected document  failed: " + x.Message });
            }
        }

        //setting is read required
        [Authorize]
        [HttpPost]
        [Route("setmandarydoc")]
        public async Task<IActionResult> SetMandatory([FromBody] DocumentSetting documentSetting)
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                documentSetting.LastUser = user.Id;
                var firstRec = dbContext.DocumentSetting.FirstOrDefault();
              
                if (firstRec != null)
                {
                    firstRec.LastUser = user.Id;
                    firstRec.ReadMandatory = documentSetting.ReadMandatory;
                    var res = dbContext.DocumentSetting.Update(firstRec);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    var res = dbContext.DocumentSetting.Add(documentSetting);
                    await dbContext.SaveChangesAsync();
                }
               
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Set Mandatory document, Success: "+ documentSetting.ReadMandatory });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Set Mandatory document  failed: " + x.Message });
            }
        }

        //Check whether Document is Read
        [Authorize]
        [HttpGet]
        [Route("checkifdocumentisread")]
        public async Task<IActionResult> CheckifDocumentisRead()
        {
            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var selectedDocument = dbContext.DocumentSetting.FirstOrDefault();
                if (selectedDocument != null)
                {
                    if (selectedDocument.ReadMandatory != "" && selectedDocument.ReadMandatory != null)
                    {
                        var res = await codeUnitWebService.Client().IsDocReadAsync(user.EmployeeId, selectedDocument.ReadMandatory);
                        return Ok(new { res.return_value });
                    }
                    else
                    {
                        var res = await codeUnitWebService.Client().IsDocReadAsync(user.EmployeeId,"NA");
                        return Ok(new { res.return_value });
                    }
                   
                }
                else
                {
                    //enable employee to operate normally if the HR hasnt setup the no read prevention measures
                    var return_value = true;

                    return Ok(new { return_value });
                }
                
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Check if Document is Read failed: " + x.Message });
            }
        }

        //****************************************************************************************************************************
        //
        //                              CONTRACT & PROBATION DOCUMENTS
        //
        //****************************************************************************************************************************
        [Authorize]
        [HttpGet]
        [Route("getcontractprobationdocs")]
        public async Task<IActionResult> GetContractProbationDocs()
        {
            try
            {
                var usr = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                List<ContractProbationModel> cp = new List<ContractProbationModel>();

                var res = await codeUnitWebService.Client().GetContProbListAsync(usr.EmployeeId);
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);
                if(resSerial != null)
                {
                    foreach (var item in resSerial)
                    {
                        ContractProbationModel cpm = new ContractProbationModel
                        {
                            Lineno = item.Lineno,
                            Docname = item.Docname,
                            Signed = item.Signed,
                            Doctype = item.Doctype,
                            Creationdate = item.Creationdate,
                            Url = item.Url
                        };
                        cp.Add(cpm);
                    }
                    return Ok(new { cp });
                }
                else
                {
                    return Ok(new { cp });
                }
              

               
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get probation/contract document list failed: " + x.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("getcontractprobationdocshr")]
        public async Task<IActionResult> GetContractProbationDocsHR()
        {
            try
            {
                var usr = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                List<ContractProbationModel> cp = new List<ContractProbationModel>();

                var res = await codeUnitWebService.Client().GetContProbListHRAsync();
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);
                if (resSerial != null)
                {
                    foreach (var item in resSerial)
                    {
                        ContractProbationModel cpm = new ContractProbationModel
                        {
                            Lineno = item.Lineno,
                            Docname = item.Docname,
                            Employeeid = item.Employeeid,
                            Employeename = item.Employeename,
                            Signed = item.Signed,
                            Doctype = item.Doctype,
                            Creationdate = item.Creationdate,
                            Url = item.Url

                        };
                        cp.Add(cpm);
                    }

                    return Ok(new { cp });

                }
                else
                {
                    return Ok(new { cp });
                }


            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get probation/contract document list failed: " + x.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("approvecontractprobationdocs/{id}")]
        public async Task<IActionResult> ApproveContractProbationDoc(string id)
        {
            try
            {
                var res = await codeUnitWebService.Client().ContProbListApprovalAsync(Int32.Parse(id));
                if(res.return_value == true)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Approve probation/contract document item success" });
                }
                return StatusCode(StatusCodes.Status501NotImplemented, new Response { Status = "Error", Message = "Approve probation/contract document item failed "});

            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Approve probation/contract document item failed: " + x.Message });
            }
        }

    }
}
