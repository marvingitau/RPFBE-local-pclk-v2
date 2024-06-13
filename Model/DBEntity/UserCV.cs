using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.DBEntity
{
    public class UserCV
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FilePath { get; set; }
        public string TagName { get; set; }
    }
}
