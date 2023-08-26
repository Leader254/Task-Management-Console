using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskMgmt.Models
{
    public class Project
    {
        public Project()
        {
            Tasks = new List<Tasks>();
        }
        public int ProjectId { get; set; }
        public string Name { get; set; }

        public List<Tasks> Tasks { get; set; }
    }
}