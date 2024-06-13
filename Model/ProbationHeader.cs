using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class ProbationHeader
    {
        public string StaffID { get; set; }
        public string ProbationID { get; set; }
        //Non-Confirmation
        public DateTime ProbationEndDate { get; set; }
        //Confirmation
        public DateTime ProbationDate { get; set; }
        public DateTime ProbationExpire { get; set; }
        //Extend
        public string ExtendDuration { get; set; }
        public DateTime ExtendDate { get; set; }
        public DateTime NextReviewDate { get; set; }
        public string DateFormulae { get; set; }

    }
}
