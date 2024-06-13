using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class GrievanceRanksRemark
    {
        public int Id { get; set; }
        public string HRrem { get; set; } = "";
        public string HRref { get; set; } = "";

        public string HODrem { get; set; } = "";
        public string HODref { get; set; } = "";

        public string HOSrem { get; set; } = "";
        public string HOSref { get; set; } = "";

        public string MDrem { get; set; } = "";
        public string MDref { get; set; } = "";

        public string Suprem { get; set; } = "";
        public string Supref { get; set; } = "";

        public string HeadHRrem { get; set; } = "";
        public string HeadHRref { get; set; } = "";
    }
}
