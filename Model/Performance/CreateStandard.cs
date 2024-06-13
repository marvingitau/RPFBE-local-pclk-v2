using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.Performance
{
    public class CreateStandard
    {
        public int ActivityCode { get; set; }
        public string HeaderNo  { get; set; }
        public string StandardDescription { get; set; }
        public int TargetCode { get; set; }
        public decimal TargetScore  { get; set; }
        public DateTime Timelines { get; set; }
    }
}
