using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class CompetenceLineModel
    {
        public string Cno { get; set; }
        public string Lineno { get; set; }
        public string Type { get; set; }
        public string Competence { get; set; }
        public string Description { get; set; }
        public string Employeeassesment { get; set; }
        public string Employeecomment { get; set; }
        public string Supervisorassesment { get; set; }
        public string Supervisorcomment { get; set; }
        public string GeneralSupervisorcomment { get; set; }
    }
}
