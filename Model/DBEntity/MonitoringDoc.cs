using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.DBEntity
{
    public class MonitoringDoc
    {
        public int Id { get; set; }
        public string UID { get; set; }
        public string MonitoringID { get; set; }
        public string Filename { get; set; }
        public string Filepath { get; set; }
        public ICollection<MonitoringDocView> MonitoringDocView { get; set; }
    }
}
