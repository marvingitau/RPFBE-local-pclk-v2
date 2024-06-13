using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.DBEntity
{
    public class DocumentSetting
    {
        public int Id { get; set; }
        public string ReadMandatory { get; set; }
        public string LastUser { get; set; }
    }
}
