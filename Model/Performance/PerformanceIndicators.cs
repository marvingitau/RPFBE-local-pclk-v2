using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.Performance
{
    public class PerformanceIndicators
    {
        public string Value { get; set; }
        public string Label { get; set; }
        public string Weighting { get; set; }
        public string TargetedScore { get; set; }
        public string AchievedScoreEmployee { get; set; }
        public string WeightedScoreEmployee { get; set; }

    }
}
