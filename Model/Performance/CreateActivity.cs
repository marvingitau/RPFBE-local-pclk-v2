using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.Performance
{
    public class CreateActivity
    {
        public string HeaderNo { get; set; }
        public int KPICode { get; set; }
        public string ActivityDescription { get; set; }
        public string Remarks { get; set; }
        public int TargetCode { get; set; }
    }
}
