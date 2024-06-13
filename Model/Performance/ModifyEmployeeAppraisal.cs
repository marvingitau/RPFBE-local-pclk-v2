using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.Performance
{
    public class ModifyEmployeeAppraisal
    {
        public int TargetCode { get; set; }
        public int IndicatorCode { get; set; }
        public int KPICode { get; set; }
        public string HeaderNo { get; set; }
        public decimal AchievedScore { get; set; }
        public string EmployeeComments { get; set; }

    }
}
