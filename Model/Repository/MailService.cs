using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using RPFBE.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Models;

namespace RPFBE.Model.Repository
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task SendShortlistAsync(Shortlisted request)
        {
            char[] delimiterChars2 = { '-', 'T', '.' };
            string inteviewdatetime = request.Date;
            string[] datetimeArr = inteviewdatetime.Split(delimiterChars2);
            string auxDate2 = datetimeArr[1] + "/" + datetimeArr[2] + "/" + datetimeArr[0];
            DateTime interviewDate = DateTime.ParseExact(auxDate2, "MM/dd/yyyy", null);

            string FilePath = Directory.GetCurrentDirectory() + "/HTML/template.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            MailText = MailText.Replace("[username]", request.UserName)
                .Replace("[jobname]", request.Jobname)
                .Replace("[interviewdate]", auxDate2)
                .Replace("[interviewtime]", request.Time)
                .Replace("[interviewvenue]", request.Venue)
                .Replace("[virtuallink]", request.VirtualLink)
                .Replace("[Company]", _mailSettings.CompanyName)
                .Replace("[Tel]", _mailSettings.Telephone)
                .Replace("[HRMail]", _mailSettings.HREmail)
                ;
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            email.Subject = $"Welcome {request.UserName}";
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task RequisitionRequestAsync(Requisitionrequest request)
        {
            //char[] delimiterChars2 = { '-', 'T', '.' };
            //string inteviewdatetime = request.Date;
            //string[] datetimeArr = inteviewdatetime.Split(delimiterChars2);
            //string auxDate2 = datetimeArr[1] + "/" + datetimeArr[2] + "/" + datetimeArr[0];
            //DateTime interviewDate = DateTime.ParseExact(auxDate2, "MM/dd/yyyy", null);

            string FilePath = Directory.GetCurrentDirectory() + "/HTML/requisitiontemplate.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            MailText = MailText.Replace("[username]", request.Username)
                .Replace("[RequisionNo]", request.RequisionNo);
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            email.Subject = $"Job Requisition Approval Request";
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }




        public async Task  SendEmailPasswordReset(string userEmail, string link)
        {

            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(userEmail));
            email.Subject = $"Reset Link";
            var builder = new BodyBuilder();
            builder.HtmlBody = $"Copy the following Reset token<b> {link}</b>";
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);

           
        }

        //Probation Mails
        public void SendEmail(SmtpClient smtp, string toEmail,string Username,string Monitorno,bool approved)
        {
            string FilePath;
            if (approved)
            {
                FilePath = Directory.GetCurrentDirectory() + "/HTML/probationmonitoring.html";
            }
            else
            {
                FilePath = Directory.GetCurrentDirectory() + "/HTML/probationmonitoringreject.html";
            }
            
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            MailText = MailText.Replace("[username]", Username)
                .Replace("[MonitorNo]", Monitorno);

            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = $"{_mailSettings.ProbationMailTitle}";
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();


            smtp.Send(email);
        }

        public void SendEmail(string[] mailers,string[] Username,string Monitorno, bool approved = true)
        {
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

            int it = 0;
            if(mailers.Length == Username.Length)
            {
                foreach (string mail in mailers)
                {
                    SendEmail(smtp, mail, Username[it++], Monitorno,approved);
                }
            }
           
           

            smtp.Disconnect(true);
        }

    }
}
