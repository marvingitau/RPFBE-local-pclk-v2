using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.DBEntity
{
    public class PerformanceMonitoring
    {
        public int Id { get; set; }
        public string HODId { get; set; }
        public string HRId { get; set; }
        public int Progresscode { get; set; } = 0;
        public string PerformanceId { get; set; }
        public string StaffName { get; set; }
        public string ManagerName { get; set; }
        public string Date { get; set; }
        public string ApprovalStatus { get; set; }

    }
}
