using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.Performance
{
    public class PerformanceIndicatorsSupervisor
    {
        public string Value { get; set; }
        public string Label { get; set; }
        public string Weighting { get; set; }
        public string TargetedScore { get; set; }
        public string AchievedScoreSupervisor { get; set; }
        public string WeightedScoreSupervisor { get; set; }
    }
}
