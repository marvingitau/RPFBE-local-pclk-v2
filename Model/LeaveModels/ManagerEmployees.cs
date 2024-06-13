using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.LeaveModels
{
    public class ManagerEmployees
    {
        public int Id { get; set; }
        public string EmployeeNo { get; set; }
        public string SupervisorNo { get; set; }
        public string EmployeeName { get; set; }
        public string FullNameReliever { get; set; }
        public string EmploymentContractCode { get; set; }
        public string Gender { get; set; }
        public string GlobalDimension1Code { get; set; }
        public string GlobalDimension2Code { get; set; }
        public string ShortcutDimension3Code { get; set; }
        public string EmployeeCompanyEmail { get; set; }
        public string JobTitle { get; set; }
       
    }
}
