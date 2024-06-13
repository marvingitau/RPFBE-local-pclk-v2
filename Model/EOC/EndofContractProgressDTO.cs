using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.EOC
{
    public class EndofContractProgressDTO
    {
        public int ContractStatus { get; set; } = 0;
        public string ContractNo { get; set; }
        public string BackTrackingReason { get; set; }
    }
}
