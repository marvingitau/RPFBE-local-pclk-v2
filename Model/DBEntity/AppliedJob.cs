using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.DBEntity
{
    public class AppliedJob
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string JobReqNo { get; set; }
        public string JobTitle { get; set; }
        public string Deadline { get; set; }
        public DateTime ApplicationDate { get; set; }
        public bool Viewed { get; set; } = false;
        public string Rejected { get; set; } = "FALSE";
        public string JobAppplicationNo { get; set; }
        public string EmpNo { get; set; }
    }
}
