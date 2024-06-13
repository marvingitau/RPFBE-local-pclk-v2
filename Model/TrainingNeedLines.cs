using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class TrainingNeedLines
    {
        public string No { get; set; }
        public string Lineno { get; set; }//in d365 is int
        public string Developmentneed { get; set; }
        public string Interventionrequired { get; set; }
        public string Objective { get; set; }
        public string Trainingprovider { get; set; }
        public string Traininglocation { get; set; }
        public string Trainingschedulefrom { get; set; }
        public string Trainingscheduleto { get; set; }
        public DateTime TrainingschedulefromDate { get; set; }
        public DateTime TrainingscheduletoDate { get; set; }

        public string Estimatedcost { get; set; }//in d365 is decimal
        public decimal EstimatedcostMoney { get; set; }//in d365 is decimal
    }
}
