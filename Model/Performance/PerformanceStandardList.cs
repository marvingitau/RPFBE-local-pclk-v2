using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.Performance
{
    public class PerformanceStandardList
    {
        public int Id { get; set; }
        public string CriteriaCode { get; set; }
        public string TargetCode { get; set; }
        public string KPIDescription { get; set; }
        public string HeaderNo { get; set; }
        public string StandardCode { get; set; }
        public string StandardDescription { get; set; }
        public string StandardWeighting { get; set; }
        public string Timelines { get; set; }
        public string ActivityDescription { get; set; }
        public string TargetedScore { get; set; }
    }
}
