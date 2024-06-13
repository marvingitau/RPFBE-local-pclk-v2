using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.DBEntity
{
    public class TrainingNeedList
    {
        public int Id { get; set; }
        public string NeedNo { get; set; }
        public string EName { get; set; }
        public string EID { get; set; }
        public int Stage { get; set; }
        public string StageName { get; set; }
        public string HODComment { get; set; }
        public string HRComment { get; set; }
        public string FMDComment { get; set; }
        public string UID { get; set; }
        public string Calender { get; set; }
        public string CalenderLabel { get; set; }
        public string GenesisPoint { get; set; }
    }
}
