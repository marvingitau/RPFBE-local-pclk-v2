using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RPFBE.Auth;
using RPFBE.Model;
using RPFBE.Model.DBEntity;
using RPFBE.Model.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPITest.Models;

namespace RPFBE.Controllers
{

    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext dbContext;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ICodeUnitWebService codeUnitWebService;
        private readonly IMailService mailService;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ICodeUnitWebService codeUnitWebService,
            IMailService mailService
            )
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.dbContext = dbContext;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.codeUnitWebService = codeUnitWebService;
            this.mailService = mailService;
        }
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var user =await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var userRoles = await userManager.GetRolesAsync(user);

            return Ok(user);
        }

        //Get the profile date of usr id

        //Skills
        [Route("getskills")]
        [HttpGet]
        public async Task<IActionResult> GetSkill()
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var res = dbContext.Skills.Where(x => x.UserId == user.Id).ToList();
            return Ok(res);
        }
        [Route("setskills")]
        [HttpPost]
        public async Task<IActionResult> SetSkill([FromBody] List<Skill> Skills)
        {
            List<Skill> skills = new List<Skill>();

            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if(dbContext.Skills.Where(x => x.UserId == user.Id).Count()>0)
            {
                dbContext.Skills.RemoveRange(dbContext.Skills.Where(x => x.UserId == user.Id));
                await dbContext.SaveChangesAsync();
            }

            foreach (var item in Skills)
            {
                var aux = new Skill
                {
                    UserId = user.Id,
                    Title = item.Title,
                    ExperienceYears = ""

                };
                skills.Add(aux);
            }

            await dbContext.Skills.AddRangeAsync(skills);
            await dbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Skill uploaded" });
        }
        
        
        [Route("profile")]
        [HttpGet]
        public async Task<ActionResult> GetProfile()
        {
            //List<Profile> profile = new List<Profile>();

            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (user.ProfileId == 0)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User dont have profile" });
                }
                var usrModel = dbContext.Users.Where(x => x.Id == user.Id).FirstOrDefault(); //.Where(y => y.ProfileId != 0)
                var profileModel = dbContext.Profiles.Where(x=>x.UserId== user.Id).FirstOrDefault();
                var skillList = dbContext.Skills.Where(x => x.UserId == user.Id).ToList();
                var userCV = dbContext.UserCVs.Where(x => x.UserId == user.Id).FirstOrDefault();

                //get bankcode
                List<BankModel> bankModels = new List<BankModel>();
                //list countrie
                List<RegionModel> countryModels = new List<RegionModel>();
                //list county
                List<RegionModel> countyModels = new List<RegionModel>();

                var bankres = await codeUnitWebService.Client().GetBanksAsync();
                dynamic bankserial = JsonConvert.DeserializeObject(bankres.return_value);

                foreach (var bb in bankserial)
                {
                    BankModel bankModel = new BankModel
                    {
                        Value = bb.Code,
                        Label = bb.Name,
                    };
                    bankModels.Add(bankModel);
                }

                //Get Countries
                var countriesNav = await codeUnitWebService.Client().CountriesAsync();
                dynamic countriesSerial = JsonConvert.DeserializeObject(countriesNav.return_value);
                foreach (var bc in countriesSerial)
                {
                    RegionModel countryModel = new RegionModel
                    {
                        Value = bc.Value,
                        Label = bc.Label,
                    };
                    countryModels.Add(countryModel);
                }

                //Get Counties
                var countiesNav = await codeUnitWebService.Client().CountiesAsync();
                dynamic countiesSerial = JsonConvert.DeserializeObject(countiesNav.return_value);
                foreach (var bc in countiesSerial)
                {
                    RegionModel countyModel = new RegionModel
                    {
                        Value = bc.Value,
                        Label = bc.Label,
                    };
                    countyModels.Add(countyModel);
                }



                return Ok(new { usrModel, profileModel, skillList, userCV , bankModels, countryModels, countyModels });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "try failed"+ex.Message });

            }
        }

        //get sub counties codes
        [Route("getsubcounty/{code}")]
        [HttpGet]
        public async Task<IActionResult> GetSubcounty(string code = "0000")
        {
            try
            {
                List<RegionModel> subCountyModels = new List<RegionModel>();
                //bank branch list
                var res = await codeUnitWebService.Client().SubcountiesAsync(code);
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);

                foreach (var bb in resSerial)
                {
                    RegionModel sc = new RegionModel
                    {
                        Value = bb.Value,
                        Label = bb.Label
                    };
                    subCountyModels.Add(sc);

                }

                return Ok(new { subCountyModels });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "Get subcounty failed" + ex.Message });
            }
        }

        //get bank branch codes
        [Route("getbranch/{code}")]
        [HttpGet]
        public async Task<IActionResult> Getbranchbank(string code ="0000")
        {
            try
            {
                List<BankBranchModel> bankBranches = new List<BankBranchModel>();
                //bank branch list
                var res = await codeUnitWebService.Client().GetBranchAsync(code);
                dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);

                foreach (var bbranch in resSerial)
                {
                    BankBranchModel bank = new BankBranchModel
                    {
                        Value = bbranch.Branchcode,
                        Label = bbranch.Branchname,
                    };
                    bankBranches.Add(bank);

                }

                return Ok(new { bankBranches });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "get branch failed" + ex.Message });
            }
        }
        //Admin
        [Route("profile/{id}/{reqno}")]
        [HttpGet]
        public async Task<ActionResult> GetProfile(string id,string reqno)
        {
            //List<Profile> profile = new List<Profile>();

            try
            {
                var user = await userManager.FindByIdAsync(id);
                if (user.ProfileId == 0)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User dont have profile" });
                }
                var userModel = dbContext.Users.Where(x => x.Id == user.Id).Where(y => y.ProfileId != 0).FirstOrDefault();
                var profileModel = dbContext.Profiles.Where(x => x.UserId == user.Id).FirstOrDefault();
                var skillList = dbContext.Skills.Where(x => x.UserId == user.Id).ToList();
                var checkList = dbContext.SpecFiles.Where(x => x.UserId == user.Id && x.JobId == reqno).ToList();
                return Ok(new { userModel,profileModel, skillList, checkList });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "try failed: " + ex.Message });

            }
        }
        [Authorize]
        [Route("setprofile")]
        [HttpPost]
        public async Task<IActionResult> SetProfile([FromBody] Profile profile)
        {

            try
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (user.ProfileId == 0)
                {
                    profile.UserId = user.Id;
                    dbContext.Profiles.Add(profile);
                    await dbContext.SaveChangesAsync();
                    var pid = profile.Id;

                    var usrrec = dbContext.Users.Where(x => x.Id == user.Id).FirstOrDefault();

                    if (usrrec != null)
                    {
                        usrrec.ProfileId = pid;
                        await dbContext.SaveChangesAsync();

                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "User profile created" });
                    }
                    return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User not found" });
                }
                else
                {
                    //Update
                    var current = dbContext.Profiles.First(x => x.Id == user.ProfileId);
                    try
                    {
                        //dbContext.Entry(current).CurrentValues.SetValues(profile);
                        current.Gender = profile.Gender;
                        current.PersonWithDisability = profile.PersonWithDisability;
                        current.FirstName = profile.FirstName;
                        current.SurName = profile.SurName;
                        current.LastName = profile.LastName;
                        current.DOB = profile.DOB;
                        current.Age = profile.Age;
                        current.PostalAddress = profile.PostalAddress;
                        current.PostCode = profile.PostCode;
                        current.City = profile.City;
                        current.Country = profile.Country;
                        current.County = profile.County;
                        current.SubCounty = profile.SubCounty;
                        current.ResidentialAddress = profile.ResidentialAddress;
                        current.MobilePhoneNo = profile.MobilePhoneNo;
                        current.MobilePhoneNoAlt = profile.MobilePhoneNoAlt;
                        current.BirthCertificateNo = profile.BirthCertificateNo;
                        current.NationalIDNo = profile.NationalIDNo;
                        current.HudumaNo = profile.HudumaNo;
                        current.PassPortNo = profile.PassPortNo;
                        current.PinNo = profile.PinNo;
                        current.NHIFNo = profile.NHIFNo;
                        current.NSSFNo = profile.NSSFNo;
                        current.DriverLincenceNo = profile.DriverLincenceNo;
                        current.MaritalStatus = profile.MaritalStatus;
                        current.Citizenship = profile.Citizenship;
                        current.Ethnicgroup = profile.Ethnicgroup;
                        current.Religion = profile.Religion;
                        current.BankCode = profile.BankCode;
                        current.BankName = profile.BankName;
                        current.BankBranchCode = profile.BankBranchCode;
                        current.Experience = profile.Experience;
                        current.BankBranchName = profile.BankBranchName;
                        current.UserId = user.Id;

                        current.WillingtoRelocate = profile.WillingtoRelocate;
                        current.HighestEducation = profile.HighestEducation;
                        current.CurrentSalary = profile.CurrentSalary;
                        current.ExpectedSalary = profile.ExpectedSalary;

                        dbContext.Profiles.Update(current);

                        await dbContext.SaveChangesAsync();
                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "User profile updated" });
                    }
                    catch (Exception)
                    {
                        return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Success", Message = "User profile not updated" });
                    }

                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = ex.Message });

            }
        }
        
        [Route("createapp/{reqNo}/{UID}")]
        [HttpGet]
        public async Task<IActionResult> CreateJobApplication(string reqNo,string UID )
        {
            var employee = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            string JobAppCode = "";
            if (ModelState.IsValid)
            {
                try
                {
                    JobAppCode = codeUnitWebService.Client().PostJobApplicationAsync(reqNo, employee.EmployeeId).Result.return_value;
                    // var jobModel = dbContext.AppliedJobs.First(x => x.JobReqNo == reqNo);
                    var jobModel = dbContext.AppliedJobs.Where(x => x.JobReqNo == reqNo && x.UserId ==UID).FirstOrDefault();

                    jobModel.Viewed = true;
                    jobModel.JobAppplicationNo = JobAppCode;
                    // await dbContext.SaveChangesAsync();
                    dbContext.AppliedJobs.Update(jobModel);
                    await dbContext.SaveChangesAsync();
                    return Ok(JobAppCode);
                }
                catch(Exception x)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Job Application Failed "+x.Message });
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Job Application Failed" });
            }
           
        }

        [Route("modifyapp")]
        [HttpPost]
        public async Task<IActionResult> ModifyJobApplication(Shortlisted shortlisted)
        {
            try
            {
                var user = await userManager.FindByIdAsync(shortlisted.UID);
                Profile current = dbContext.Profiles.First(x => x.Id == user.ProfileId);
                var Client = await userManager.FindByIdAsync(current.UserId);
        
               

                try
                {
                    //check DateTime.ParseExact(LeaveApplicationObj.LeaveStartDate, "MM/dd/yy", null)
                    //Push primary User data
                 

                    //String Array
                    string[] textUserData = new string[35];
                    textUserData[0] = string.IsNullOrEmpty(current.Gender)?"NA": current.Gender;
                    textUserData[1] = string.IsNullOrEmpty(current.PersonWithDisability)?"NA": current.PersonWithDisability;
                    textUserData[2] = "";

                    textUserData[3] = string.IsNullOrEmpty(current.City) ? "NA" : current.City;
                    textUserData[4] = string.IsNullOrEmpty(current.Country) ? "NA" : current.Country;
                    textUserData[5] = string.IsNullOrEmpty(current.County) ? "NA" : current.County;
                    textUserData[6] = string.IsNullOrEmpty(current.SubCounty)?"NA":current.SubCounty;
                    textUserData[7] = string.IsNullOrEmpty(current.ResidentialAddress)?"NA": current.ResidentialAddress;
                    textUserData[8] = string.IsNullOrEmpty(current.MobilePhoneNo)?"NA": current.MobilePhoneNo;

                    textUserData[9] = "";//current.MobilePhoneNoAlt;
                    textUserData[10] = string.IsNullOrEmpty(current.BirthCertificateNo)?"NA": current.BirthCertificateNo;
                    textUserData[11] = string.IsNullOrEmpty(current.HudumaNo)?"NA": current.HudumaNo;
                    textUserData[12] = string.IsNullOrEmpty(current.PassPortNo)?"NA": current.PassPortNo;
                    textUserData[13] = string.IsNullOrEmpty(current.PinNo)?"NA": current.PinNo;
                    textUserData[14] = string.IsNullOrEmpty(current.NHIFNo)?"NA":current.NHIFNo;
                    textUserData[15] = string.IsNullOrEmpty(current.NSSFNo)?"NA": current.NSSFNo;
                    textUserData[16] = string.IsNullOrEmpty(current.DriverLincenceNo)?"NA": current.DriverLincenceNo;

                    textUserData[17] = string.IsNullOrEmpty(current.MaritalStatus)?"NA": current.MaritalStatus;
                    textUserData[18] = string.IsNullOrEmpty(current.Citizenship)?"NA": current.Citizenship;
                    textUserData[19] = string.IsNullOrEmpty(current.Ethnicgroup)?"NA": current.Ethnicgroup;
                    textUserData[20] = string.IsNullOrEmpty(current.Religion)?"NA": current.Religion;
                    textUserData[21] = string.IsNullOrEmpty(current.BankName)?"NA": current.BankName;
                    textUserData[22] = string.IsNullOrEmpty(current.BankBranchName)?"NA": current.BankBranchName;

                    textUserData[23] = string.IsNullOrEmpty(current.Age)?"NA": current.Age;
                    textUserData[24] = string.IsNullOrEmpty(current.PostalAddress)?"NA": current.PostalAddress;
                    textUserData[25] = string.IsNullOrEmpty(current.PostCode)?"NA": current.PostCode;
                    textUserData[26] = string.IsNullOrEmpty(current.NationalIDNo)?"NA": current.NationalIDNo;
                    textUserData[27] = string.IsNullOrEmpty(current.BankCode)?"NA": current.BankCode;
                    textUserData[28] = "";
                    textUserData[29] = string.IsNullOrEmpty(current.BankBranchCode)?"NA": current.BankBranchCode;

                    textUserData[30] = string.IsNullOrEmpty(current.SurName)?"NA": current.SurName;
                    textUserData[31] = string.IsNullOrEmpty(current.FirstName)?"NA": current.FirstName;
                    textUserData[32] = string.IsNullOrEmpty(current.LastName)?"NA": current.LastName;
                    textUserData[33] = string.IsNullOrEmpty(Client.Email)?"NA": Client.Email;
                    textUserData[34] = string.IsNullOrEmpty(user.Pcode)?"NA": user.Pcode;



                    char[] delimiterChars = { '-', 'T' };
                    string text = current.DOB;

                    string[] words = text.Split(delimiterChars);
                    string auxDate = words[1] + "/" + words[2] + "/" + words[0];

                    DateTime datetime = DateTime.ParseExact(auxDate, "MM/dd/yyyy", null);

                    char[] delimiterChars2 = { '-', 'T','.' };
                    string inteviewdatetime = shortlisted.Date;
                    string[] datetimeArr = inteviewdatetime.Split(delimiterChars2);
                    string auxDate2 = datetimeArr[1] + "/" + datetimeArr[2] + "/" + datetimeArr[0];

                    DateTime interviewDate = DateTime.ParseExact(auxDate2, "MM/dd/yyyy", null);

                    //Send Email

                    try
                     {
                        //shortlisted.ToEmail = Client.Email;
                        //shortlisted.UserName = Client.UserName;
                        //await mailService.SendShortlistAsync(shortlisted);
                        //@email

                        var res = await codeUnitWebService.Client().JobApplicationModifiedAsync(shortlisted.JobAppNo, textUserData, datetime, shortlisted.Venue,
                            interviewDate, shortlisted.Time,shortlisted.VirtualLink);
                        if (res.return_value)
                        {
                            var mailRes = await codeUnitWebService.WSMailer().ShortlistedInterviewNoticeAsync(shortlisted.JobAppNo);
                        }
                        return Ok(res.return_value);
                     }
                     catch (Exception x)
                     {
                         return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "Email failed/Modification failed " + x.Message });
                     }
                    
                }
                catch (Exception x)
                {

                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "Job creation Update failed "+x.Message });
                }
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "User  not found" });
            }
           

        }
        //Applicant to Employee
        [Authorize]
        [HttpGet]
        [Route("moveapplicanttoemployee/{JAPNO}")]
        public async Task<IActionResult> MoveApplicantToEmployee(string JAPNO)
        {
            try
            {
                var res = await codeUnitWebService.Client().JobApplicantToEmployeeAsync(JAPNO);
                if(res.return_value != "" && res.return_value != null)
                {
                    var requsitionModel = dbContext.AppliedJobs.Where(x => x.JobAppplicationNo == JAPNO).FirstOrDefault();
                    var user = dbContext.Users.Where(y => y.Id == requsitionModel.UserId).FirstOrDefault();
                    user.EmployeeId = res.return_value;
                    //user.Rank = "NORMAL";
                    dbContext.Users.Update(user);
                    await dbContext.SaveChangesAsync();

                    //if (!await roleManager.RoleExistsAsync("Normal"))
                    //    await roleManager.CreateAsync(new IdentityRole("Normal"));
                   

                    if (await roleManager.RoleExistsAsync("NORMAL"))
                    {
                        await userManager.AddToRoleAsync(user, "NORMAL");

                    }


                    requsitionModel.EmpNo = res.return_value;
                    dbContext.AppliedJobs.Update(requsitionModel);
                    await dbContext.SaveChangesAsync();

                    return Ok(res.return_value);

                }
                else
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "User move Failed" });
                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "User Move Failed  not found: "+x.Message });
            }
        }
    
        //****Normal Staff Profile Section
        //Get Profile if it exists
        [Authorize]
        [HttpGet]
        [Route("getstaffprofile")]
        public async Task<IActionResult> GetStaffProfile()
        {
            try
            {
                var userModel = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var profiletableModel = dbContext.Profiles.Where(x => x.UserId == userModel.Id).FirstOrDefault();
                
                return Ok(new { userModel.UserName, profiletableModel });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "Get Profile Failed: " + x.Message });
            }
        }

        //Update Staff profil
        [Authorize]
        [HttpPost]
        [Route("setstaffprofile")]
        public async Task<IActionResult> SetStaffProfile([FromBody] Profile profile)
        {
            try
            {
                var userModel = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var profileModel = dbContext.Profiles.Where(x => x.UserId == userModel.Id).FirstOrDefault();
                if(profileModel != null)
                {
                    profileModel.FirstName = profile.FirstName;
                    profileModel.SurName = profile.SurName;
                    profileModel.LastName = profile.LastName;
                    profileModel.Gender = profile.Gender;
                    profileModel.PersonWithDisability = profile.PersonWithDisability;
                    profileModel.MaritalStatus = profile.MaritalStatus;
                    profileModel.Religion = profile.Religion;
                    profileModel.Experience = profile.Experience;
                    profileModel.HighestEducation = profile.HighestEducation;
                    profileModel.DOB = profile.DOB;
                    profileModel.Age = profile.Age;

                    dbContext.Update(profileModel);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    profile.UserId = userModel.Id;
                    dbContext.Profiles.Add(profile);
                    await dbContext.SaveChangesAsync();
                }

                userModel.Name = profile.FirstName + " " + profile.SurName + " " + profile.LastName;
                userModel.ProfileId = profile.Id;
                await userManager.UpdateAsync(userModel);

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Set Profile Success " });
            }
            catch (Exception x)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new Response { Status = "Error", Message = "Set Profile Failed: " + x.Message });
            }
        }

}
}
