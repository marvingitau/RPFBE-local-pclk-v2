using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class PerformanceLineModel
    {
        public string Monitorno { get; set; }
        public string Performanceparameter { get; set; }
        public string Currentperformance { get; set; }
        public string Month1 { get; set; }
        public string Month2 { get; set; }
        public string Month3 { get; set; }
        public bool Original { get; set; }
    }
}
