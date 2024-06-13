using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Settings
{
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        public string CompanyName { get; set; }
        public string Telephone { get; set; }
        public string HREmail { get; set; }

        //probation Constacts
        public string ProbationMailTitle { get; set; }
        public string PasswordResetProtocol { get; set; }




    }
}
