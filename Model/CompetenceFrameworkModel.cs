using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class CompetenceFrameworkModel
    {
        public string Cno { get; set; }
        public string Staffno { get; set; }
        public string Staffname { get; set; }
        public string Supervisorno { get; set; }
        public string Supervisorname { get; set; }
        public string Startdate { get; set; }
        public string Enddate { get; set; }
        public string Behavescoreemp { get; set; }
        public string Behavescoresup { get; set; }
        public string Behavescoreavg { get; set; }
        public string Techscoreemp { get; set; }
        public string Techscoresup { get; set; }
        public string Techscoreavg { get; set; }
        public string Averagescore { get; set; }
        public string Percentagescore { get; set; }
        public string Status { get; set; }
    }
}
