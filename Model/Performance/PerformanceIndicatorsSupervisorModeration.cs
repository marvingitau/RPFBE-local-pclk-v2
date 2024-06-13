using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.Performance
{
    public class PerformanceIndicatorsSupervisorModeration
    {
        public string HeaderNo { get; set; }
        public string Value { get; set; } //KPIcode
        public string Label { get; set; } //Description
        public string ObjectiveWeightage { get; set; }
        public string TargetedScore { get; set; }
        public string AchievedScoreSupervisor { get; set; }
        public string WeightedResultsSupervisor { get; set; }
        public string AchievedScoreEmployee { get; set; }
        public string WeightedResultsEmployee { get; set; }
        public string OverallAchievedScore { get; set; }
        public string OverallWeightedResults { get; set; }
    }
}
