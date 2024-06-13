using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class ClearanceList
    {
        public int Id { get; set; }
        public string Clearanceno { get; set; }
        public string Lineno { get; set; }
        public string Dept { get; set; }
        public string Items { get; set; }
        public string Clearance { get; set; }
        public string Remarks { get; set; }
        public string Value { get; set; }
        public string Kalue { get; set; }
        public string Clearedby { get; set; }
        public string Designation { get; set; }

        //HOD-HR Fields
        public string Year { get; set; }
        public string AnnualLeaveDays { get; set; }
        public string AnnualDaysLess { get; set; }
        public string BalDays { get; set; }

        //HOD-FIN Fields
        public string StaffLoan { get; set; }
        public string OtherLoan { get; set; }
        public string JitSavings { get; set; }
        public string AccountantOne { get; set; }
        public string NameOne { get; set; }
        public string AccountantTwo { get; set; }
        public string NameTwo { get; set; }
        public string FinanceManager { get; set; }
        public string FinanceManagerName { get; set; }
        public string FinanceDirector { get; set; }
        public string FinanceDirectorName { get; set; }
    }
}
