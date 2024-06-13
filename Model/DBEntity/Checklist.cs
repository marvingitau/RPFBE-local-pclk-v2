using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.DBEntity
{
    public class Checklist
    {
        public int UserId { get; set; }
        public string Qualification { get; set; }
        public string OtherDoc { get; set; }
        public string Experience { get; set; }
        public string JobId { get; set; }


    }
}
