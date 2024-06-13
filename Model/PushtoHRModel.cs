using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class PushtoHRModel
    {
        public string Reqno { get; set; }
        public string Jobno { get; set; }
        public string RequestedEmployees { get; set; }
        public string ClosingDate { get; set; }
        public string HODcomment { get; set; }
        public string UIDcomment { get; set; }
        public string HRcomment { get; set; }
        public string MDcomment { get; set; }
        public string Stage { get; set; }
    }
}
