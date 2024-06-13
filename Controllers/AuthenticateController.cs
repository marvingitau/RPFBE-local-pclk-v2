using AdminAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RPFBE.Auth;
using RPFBE.Model;
using RPFBE.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
//using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Controllers
{
    //[EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly ICodeUnitWebService codeUnitWebService;
        private readonly IOptions<MailSettings> mailSettings;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger<AuthenticateController> logger;
        private readonly ApplicationDbContext dbContext;

        public AuthenticateController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ICodeUnitWebService codeUnitWebService,
            IOptions<MailSettings> mailSettings,
             IWebHostEnvironment webHostEnvironment,
             ILogger<AuthenticateController> logger,
               ApplicationDbContext dbContext
            )
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.codeUnitWebService = codeUnitWebService;
            this.mailSettings = mailSettings;
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }


        private string EncodeToken(string token)
        {
            //string strRes = token.Insert(50, "B");
            int pPstn = token.IndexOf('.') + 3;
            return token.Insert(pPstn, "7"); ;
            //return token;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if(user != null)
                {
                    string scrablr = "Ss"; //append a string to to the start of the token ,scrabler
                    var isEmplyeeActive = await codeUnitWebService.EmployeeAccount().EmployeeAccountIsActiveAsync(user.EmployeeId); // D365 check status

                    if (isEmplyeeActive.return_value || user.EmployeeId == null)
                    {
                        if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
                        {
                            var userRoles = await userManager.GetRolesAsync(user);

                            var authClaims = new List<Claim>
                            {
                                new Claim("Name", user.UserName),
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            };

                            int clainSkipper = 0;
                            foreach (var userRole in userRoles)
                            {
                                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                                if (clainSkipper == 0)
                                {
                                    authClaims.Add(new Claim("Role", userRole));
                                }
                                clainSkipper++;
                            }

                            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

                            //var signInCreds = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature);
                            var signInCreds = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

                            var token = new JwtSecurityToken(
                                issuer: configuration["JWT:ValidIssuer"],
                                audience: configuration["JWT:ValidAudience"],
                                expires: DateTime.UtcNow.AddHours(3),
                                claims: authClaims,
                                signingCredentials: signInCreds
                                );

                            var Name = user.UserName;
                            logger.LogInformation($"User:{user.EmployeeId},Verb:POST,Path:Staff Login Success");
                            return Ok(new
                            {
                                idToken = EncodeToken(new JwtSecurityTokenHandler().WriteToken(token).Insert(0, scrablr)),
                                //expiresIn = token.ValidTo.TimeOfDay.TotalMilliseconds,
                                expireDate = token.ValidTo.ToLocalTime(),
                            });
                        }
                        return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = "INVALID_USER" });
                    }
                    else
                    {
                        logger.LogWarning($"User:{user.EmployeeId},Verb:POST,Action: Staff Account is Disabled");
                        return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = "USER_DISABLED" });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = "INVALID_USER" });
                }
          


            }
            catch (Exception x)
            {
                logger.LogError($"User:{model.Email},Verb:POST,Action:Staff Login Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Login Err: " + x.Message });

            }

            //return Unauthorized();
        }


        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            try
            {
                //Check Staff Staff above all else.
                var isEmplyeeActive = await codeUnitWebService.EmployeeAccount().EmployeeAccountIsActiveAsync(model.EmployeeId); // D365 check status
                if (isEmplyeeActive.return_value)
                {
                    //Erase Jseeker record
                    var jseekrExists = dbContext.Users.Where(x => x.EmployeeId == model.EmployeeId && x.Rank == null).Count(); //count only jobseeker
                    if (jseekrExists > 0)
                    {
                        var jseekr = dbContext.Users.Where(x => x.EmployeeId == model.EmployeeId && x.Rank == null).First();
                        dbContext.Users.Remove(jseekr);
                        await dbContext.SaveChangesAsync();
                        logger.LogInformation($"User:{model.EmployeeId},Verb:Delete,Path:Delete the Job Seeker Account with the same Employee No");
                    }
                    // return Ok(model.EmployeeId);
                    var res = await codeUnitWebService.EmployeeAccount().LoginEmployeeCoreAsync(model.EmployeeId, Cryptography.Hash(model.Password), model.Email);
                    dynamic resSerial = JsonConvert.DeserializeObject(res.return_value);

                    LoginEmpCoreModel loginEmpCore = new LoginEmpCoreModel
                    {
                        Status = resSerial.Status,
                        Rank = resSerial.Rank
                    };


                    if (loginEmpCore.Status)
                    {
                        try
                        {

                            var userExists = await userManager.FindByNameAsync(model.Username);
                            if (userExists != null)
                            {
                                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
                            }

                            ApplicationUser user = new ApplicationUser()
                            {
                                Email = model.Email,
                                SecurityStamp = Guid.NewGuid().ToString(),
                                UserName = model.Username,
                                EmployeeId = model.EmployeeId,
                                Name = model.Username,
                                Rank = loginEmpCore.Rank
                            };
                            var result = await userManager.CreateAsync(user, model.Password);
                            //var errs = result.Errors.Select(x => "Code: " + x.Code + " Description: " + x.Description).ToArray();

                            //Catch Identity Errors
                            List<IdentityError> errorList = result.Errors.ToList();
                            var errors = string.Join(", ", errorList.Select(e => e.Description));

                            if (!result.Succeeded)
                            {
                                //return Ok(new { errs });
                                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = errors });

                            }
                            if (!await roleManager.RoleExistsAsync(loginEmpCore.Rank.ToUpper()))
                                await roleManager.CreateAsync(new IdentityRole(loginEmpCore.Rank.ToUpper()));
                            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                            if (await roleManager.RoleExistsAsync(loginEmpCore.Rank.ToUpper()))
                            {
                                await userManager.AddToRoleAsync(user, loginEmpCore.Rank.ToUpper());

                            }
                            logger.LogInformation($"User:{user.EmployeeId},Verb:POST,Path:Staff Creation Success");
                            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
                        }
                        catch (Exception x)
                        {
                            logger.LogError($"User:{model.EmployeeId},Verb:POST,Action:Staff Creation Failed,Message:{x.Message}");
                            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed Exception! :" + x.Data });
                        }

                    }
                    else
                    {
                        logger.LogError($"User:NA,Verb:POST,Action:Staff Creation Failed,Message:Invalid User:{model.EmployeeId}");
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "INVALID_USER_D365" });
                    }

                }
                else
                {
                    logger.LogWarning($"User:{model.EmployeeId},Verb:POST,Action: Staff Account is Disabled");
                    return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = "USER_DISABLED" });
                }


            }
            catch (Exception x)
            {
                logger.LogError($"Staff:{model.EmployeeId},Verb:POST,Action:Staff Registration Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Staff Registration Failed: " + x.Message });

            }

        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                //var userExists = await userManager.FindByNameAsync(model.Username);
                //if (userExists != null)
                //    return StatusCode(StatusCodes.Status208AlreadyReported, new Response { Status = "Error", Message = "USER_EXIST" });

                ApplicationUser user = new ApplicationUser()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username,
                    Name = model.Name,
                    Pcode = Cryptography.Hash(model.Password)
                };
                var result = await userManager.CreateAsync(user, model.Password);
                //Catch Identity Errors
                List<IdentityError> errorList = result.Errors.ToList();
                var errors = string.Join(", ", errorList.Select(e => e.Description));

                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status208AlreadyReported, new Response { Status = "Error", Message = "User creation failed: " + errors });

                logger.LogInformation($"User:{user.Name},Verb:POST,Path:Job Seeker Creation Success");
                return Ok(new Response { Status = "Success", Message = "User created successfully!" });
            }
            catch (Exception x)
            {
                logger.LogError($"Staff:{model.Name},Verb:POST,Action:Job Seeker Registration Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "CREATION_FAILED", ExtMessage = x.Message });
            }

        }

        [HttpPost]
        [Route("sendpasswordresetlink")]
        public async Task<ActionResult> SendPasswordResetLink([FromBody] ResetEmail register)
        {
            //Math or Char capcha
            if (register.Approved)
            {
                return Ok("");
            }
            //End Math or Char capcha

            if (ModelState.IsValid)
            {
                try
                {
                    string customerNo = register.EmployeeId;
                    var customerEmailAddress = await codeUnitWebService.EmployeeAccount().GetEmployeeEmailAddressAsync(customerNo);


                    //If customer does not exist
                    var employeeExist = await codeUnitWebService.EmployeeAccount().EmployeeExistsAsync(customerNo);
                    if (employeeExist.return_value)
                    {
                        var employeeActive = await codeUnitWebService.EmployeeAccount().EmployeeAccountIsActiveAsync(customerNo);
                        if (employeeActive.return_value)
                        {
                            if (customerEmailAddress.return_value != "")
                            {
                                //Generate Password Reset Token
                                Random rnd = new Random();
                                int prefix = rnd.Next(10000, 1000000);
                                int surfix = rnd.Next(10000, 1000000);
                                string passwordResetToken = Cryptography.Hash(prefix.ToString() + customerNo + surfix.ToString());
                                //Save the password reset token
                                var savedToken = await codeUnitWebService.EmployeeAccount().SetPasswordResetTokenAsync(customerNo, passwordResetToken);
                                //Create Email Body
                                var callbackUrl = $"localhost:3000/sendpasswordreset/ResetEmployeePassword/{customerNo}/{passwordResetToken }";
                                //var callbackUrl = Url.Action("ResetEmployeePassword", "Account", new { CustomerNo = customerNo, PasswordResetToken = passwordResetToken }, mailSettings.Value.PasswordResetProtocol);
                                var linkHref = "<a href='" + callbackUrl + "' class='btn btn-primary'><strong>Create New Password</strong></a>";

                                string emailBody = "<p>You recently requested to create your password for your " + mailSettings.Value.CompanyName + " employee account no. " + customerNo + ". Below is a token you'll use, to set the password.</p>";
                                emailBody += "<p>" + passwordResetToken + "</p>";
                                emailBody += "<p><b><i>Note that this token will expire after 24hrs</i></b></p>";
                                //End Create Email Body

                                //File Links Test
                                try
                                {
                                    var subDirectory = "Files/LeaveAttachments";
                                    var target = Path.Combine(webHostEnvironment.ContentRootPath, subDirectory);

                                    // string fileName = @"C:\Temp\MaheshTX.txt";

                                    // Set a variable to the Documents path.
                                    //string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                                    // Write the specified text asynchronously to a new file named "WriteTextAsync.txt".
                                    using (StreamWriter outputFile = new StreamWriter(Path.Combine(target, "WriteTextAsync.txt")))
                                    {
                                        await outputFile.WriteAsync(emailBody);
                                    }



                                }
                                catch (Exception x)
                                {
                                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "File Err: " + x.Message });

                                }

                                //Send Email
                                var sendEmail = await codeUnitWebService.EmployeeAccount().SendPasswordResetLinkAsync(customerNo, emailBody);
                                if (sendEmail.return_value)
                                {
                                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Reset Email Sent" });
                                }
                                else
                                {
                                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Send Email Failed" });
                                }

                            }
                            else
                            {
                                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee Email is Missing" });
                            }
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee Account Not Active" });
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee Not Found" });

                    }


                }
                catch (Exception x)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Reset Password Email Failed: " + x.Message });

                }
            }
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Invalid Input" });

            }
        }

        [HttpPost]
        [Route("setpassword")]
        public async Task<IActionResult> SetPassword([FromBody] ResetEmail newpassword)
        {
            try
            {
                //Check if the Employee Exist
                var isEmployeeExist = await codeUnitWebService.EmployeeAccount().EmployeeExistsAsync(newpassword.EmployeeId);
                if (isEmployeeExist.return_value)
                {
                    //Check if Employee is Active
                    var isEmployeeActive = await codeUnitWebService.EmployeeAccount().EmployeeAccountIsActiveAsync(newpassword.EmployeeId);
                    if (isEmployeeActive.return_value)
                    {
                        //Check if Token is Equal
                        var isTokenEqual = await codeUnitWebService.EmployeeAccount().GetPasswordResetTokenAsync(newpassword.EmployeeId);
                        if (isTokenEqual.return_value.Equals(newpassword.Token))
                        {
                            //Check if Token is expired
                            var isTokenExpired = await codeUnitWebService.EmployeeAccount().IsPasswordResetTokenExpiredAsync(newpassword.EmployeeId, newpassword.Token);
                            if (!isTokenExpired.return_value)
                            {
                                //Set Passcode
                                var isTokenSet = await codeUnitWebService.EmployeeAccount().ResetEmployeePortalPasswordAsync(newpassword.EmployeeId, Cryptography.Hash(newpassword.Password));
                                if (isTokenSet.return_value)
                                {
                                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Password Set Success" });

                                }
                                else
                                {
                                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Password Set Failed" });

                                }
                            }
                            else
                            {
                                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Expired Token" });

                            }
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Invalid Token" });

                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Account is Inactive" });

                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee is Missing" });

                }
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "SetPassword Failed: " + x.Message });

            }
        }
        //Check Docs Read status
        [HttpGet]
        [Route("isreaddoc/{EID}")]
        public async Task<IActionResult> IsReadDoc(string EID)
        {
            try
            {
                //Get mandatory docs
                var res = await codeUnitWebService.Client().IsDocReadAsync(EID, "");
                return Ok(new { res.return_value });
            }
            catch (Exception x)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Read Status Failed: " + x.Message });

            }
        }

        [Authorize(Roles = UserRoles.HR + "," + UserRoles.HEADHR)]
        [HttpGet]
        [Route("deleteuser/{EID}")]
        public async Task<IActionResult> DeleteUser(string EID)
        {
            try
            {
                var uzer = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

                var user = await userManager.FindByIdAsync(EID);
                if (user != null && user.UserName != "ADMIN")
                {
                    IdentityResult result = await userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"User:{uzer.EmployeeId},Verb:GET,Path:User-{EID} Deletion Success");
                        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "User Deletion Success " });
                    }
                    else
                    {
                        logger.LogWarning($"User:{uzer.EmployeeId},Verb:GET,Path:User-{EID} Deletion Failed");
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Deletion Failed" });
                    }
                }
                logger.LogError($"User:{uzer.EmployeeId},Verb:GET,Action:User-{EID} Deletion Failed");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Not Found: " });

            }
            catch (Exception x)
            {
                var uzer = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                logger.LogError($"User:{uzer.EmployeeId},Verb:GET,Action:User-{EID} Deletion Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Deletion Failed: " + x.Message });
            }
        }

    }
}
