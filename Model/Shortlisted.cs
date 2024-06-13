using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Models
{
    public class Shortlisted
    {
        public string Jobname { get; set; }
        public string ToEmail { get; set; }
        public string UserName { get; set; }
        public string Date { get; set; }
        public string JobAppNo { get; set; }
        public string UID { get; set; }

        public string Time { get; set; }
        public string Venue { get; set; } = "N/A";
        public string VirtualLink { get; set; } = "N/A";


    }
}
