using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.LMS
{
    public class SeletectedCoursesDTO
    {
        public int LineNo { get; set; }
        public Selectable Course { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public string AppraisalNo { get; set; }
        public string CourseScore { get; set; }
        public string Origin { get; set; }
    }
}
