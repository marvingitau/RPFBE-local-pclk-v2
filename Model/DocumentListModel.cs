using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class DocumentListModel
    {

        public int Id { get; set; }
        public string EmployeeNo { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentName { get; set; }
        public string Read { get; set; }
        public string DateTimeRead { get; set; }
        public string Payrollcode { get; set; }
        public string URL { get; set; }
    }
}
