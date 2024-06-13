using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.DBEntity
{
    public class Skill
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string ExperienceYears { get; set; }
    }
}
