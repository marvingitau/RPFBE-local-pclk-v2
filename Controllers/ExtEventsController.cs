using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPFBE.Auth;
using RPFBE.Model;
using RPFBE.Service.ExtServs;
using RPFBE.Service.ExtServs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExtEventsController : Controller
    {
        private readonly ILogger<ExtEventsController> logger;
        private readonly ICodeUnitWebService codeUnitWebService;
        private readonly IOptions<WebserviceCreds> config;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        //private readonly IAESc aESc;

        public ExtEventsController(
            ILogger<ExtEventsController> logger,
            ICodeUnitWebService codeUnitWebService,
            IOptions<WebserviceCreds> config,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext
           // IAESc iAESc
            )
        {
            this.logger = logger;
            this.codeUnitWebService = codeUnitWebService;
            this.config = config;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.dbContext = dbContext;
            //this.aESc = iAESc;
        }
        [HttpPost]
        [Route("modifyrec")]
        public async Task<IActionResult> ModifyOpenEOCProbs([FromBody] ExtPayload extpayload)
        {
            try
            {
                //Get all EOC records under HOD and Below interms of progress
                var getPendingEOC = await dbContext.EndofContractProgress.Where(x =>x.EmpID == extpayload.EID && x.ContractStatus < 2).ToListAsync();
                foreach (var item in getPendingEOC)
                {
                    item.HODEid = extpayload.NEWHOD;
                }
                dbContext.EndofContractProgress.UpdateRange(getPendingEOC);
                await dbContext.SaveChangesAsync();

                // var getPendingProb;
                //Get all PROB records under HOD and Below interms of progress
                var getPendingProb = await dbContext.ProbationProgress.Where(x => x.EmpID == extpayload.EID && x.ProbationStatus < 2).ToListAsync();
                foreach (var item in getPendingProb)
                {
                    item.HODEid = extpayload.NEWHOD;
                }
                dbContext.ProbationProgress.UpdateRange(getPendingProb);
                await dbContext.SaveChangesAsync();

                logger.LogInformation($"User:NA,Verb:POST,Action:ModifyRec Success,Message:Records Modified Prob:{getPendingProb.Count()},EOC:{getPendingEOC.Count()}");
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Prob/EOC Modified Successfully" });

            }
            catch (Exception x)
            {
                logger.LogError($"User:NA,Verb:POST,Action:ModifyRec Failed,Message:{x.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Get Courses Failed: " + x.Message });

            }
        }
        
       
    }
}
