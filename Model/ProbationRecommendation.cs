using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class ProbationRecommendation
    {
        public string EmployeeStrongestPoint { get; set; }
        public string EmployeeWeakestPoint { get; set; }
        public string EmployeeQualifiedForPromo { get; set; }
        public string PromoPosition { get; set; }
        public string PromotableInTheFuture { get; set; }
        public string EffectiveDifferentAssignment { get; set; }
        public string WhichAssignment { get; set; }
        public string AdditionalComment { get; set; }

        public bool confirm { get; set; }
        public bool Extend { get; set; }
        public bool Terminate { get; set; }
        public string HRremark { get; set; }
        public string MDremark { get; set; }
    }
}
