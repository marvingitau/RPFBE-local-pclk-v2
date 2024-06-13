using Microsoft.AspNetCore.Identity;
using RPFBE.Model.DBEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Auth
{
    public class ApplicationUser:IdentityUser
    {
        [DefaultValue(0)]
        public int ProfileId { get; set; } = 0;
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Rank { get; set; }
        public string Pcode { get; set; }
        public string Payrollcode { get; set; }
        public string Calculationscheme { get; set; }
        
    }
}
