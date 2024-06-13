using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.LeaveModels
{
    public class ApproveeList
    {
        public int Id { get; set; }
        public string EntryNo { get; set; }
        public string TableID { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNo { get; set; }
        public string Description { get; set; }
        public string SequenceNo { get; set; }
        public string ApprovalCode { get; set; }
        public string SenderID { get; set; }
        public string ApproverID { get; set; }
        public string Status { get; set; }
        public string DateTimeSentforApproval { get; set; }
        public string Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string SenderEmployeeNo { get; set; }
        public string SenderEmployeeName { get; set; }
        public string ApproverEmployeeNo { get; set; }
        public string ApproverEmployeeName { get; set; }
    }
}
