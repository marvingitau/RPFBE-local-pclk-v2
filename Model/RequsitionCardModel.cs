using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class RequsitionCardModel
    {
        public string No { get; set; }
        public string Jobno { get; set; }
        public string  Jobtitle { get; set; }
        public string Description { get; set; }
        public string Jobgrade { get; set; }
        public string Maxposition { get; set; }
        public string Occupiedposition { get; set; }
        public string Vacantposition { get; set; }
        public string Requestedemployees { get; set; }
        public string Closingdate { get; set; }
        public string Requisitiontype { get; set; }
        public string Contractcode { get; set; }
        public string Reason { get; set; }
        public string Branchcode { get; set; }
        public string Jobadvertised { get; set; }
        public string Jobadvertiseddropped { get; set; }
        public string Status { get; set; }
        public string Userid { get; set; }
        public string Comments { get; set; }
        public string Documentdate { get; set; }
        public string Desiredstartdate { get; set; }
        public string HOD { get; set; }
        public string HR { get; set; }
        public string MD { get; set; }

        public string Product { get; set; }
        public string Department { get; set; }
        public string RequisitionNature { get; set; } // New or Replacement 
        public string Employeetoreplace { get; set; }
    }
}
