using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPITest.Models;

namespace RPFBE.Model.Repository
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
        Task SendShortlistAsync(Shortlisted request);
        Task SendEmailPasswordReset(string userEmail, string link);
        Task RequisitionRequestAsync(Requisitionrequest request);
        void SendEmail(string[] mailers,string[] Username,string Monitorno, bool approved = true);
        //void RejectedApplicants(List<RejectedApplicantList> rejects);

    }
}