using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class MonitoringInvokePIP
    {
        //public int Id { get; set; }
        //Header
        public string MonitorNo { get; set; }
        public string PerformanceMonths { get; set; }
        public string ReviewDate { get; set; }
        //Line
        public int Lineno { get; set; }
        //HeaderNo => MonitorNo
        public string Month { get; set; }
        public string SalesTarget { get; set; }
        public string SalesRep { get; set; }

    }
}
