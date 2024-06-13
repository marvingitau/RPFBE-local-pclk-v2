using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.Performance
{
    public class PerformanceActivityList
    {
        public int Id { get; set; }
        public string CriteriaCode { get; set; }
        public string TargetCode { get; set; }
        public string HeaderNo { get; set; }
        public string Activitycode { get; set; }
        public string Value { get; set; }
        public string ActivityDescription { get; set; }
        public string Label { get; set; }
        public string ActivityWeighting { get; set; }
    }
}
