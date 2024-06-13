using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.LeaveModels
{
    public class ExtraDaysList
    {
        public string Leaveno { get; set; }
        public string Employeeno { get; set; }
        public string Startdate { get; set; }
        public decimal Days { get; set; }
        public string Enddate { get; set; }
        public string Returndate { get; set; }
        public string Assignedleaveno { get; set; }

    }
}
