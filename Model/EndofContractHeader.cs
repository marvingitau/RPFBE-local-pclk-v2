using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class EndofContractHeader
    {
        public string EocID { get; set; }
        public string StaffNo { get; set; }
        //Non Renewal
        public DateTime TerminationDate { get; set; }
        //Renewal
        public string RenewalTime { get; set; }
        public DateTime ContractedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string NewSalary { get; set; }
        public string DateFormulae { get; set; }



    }
}
