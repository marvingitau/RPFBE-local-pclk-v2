using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.DBEntity
{
    public class MonitoringDocView
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        public string Viewed { get; set; } = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        public MonitoringDoc MonitoringDoc { get; set; }
    }
}
