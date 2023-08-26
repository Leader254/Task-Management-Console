using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskMgmt.Models
{
    // enum Status
    public enum Status
    {
        NotStarted = 1,
        InProgress = 2,
        Completed = 3
    }
    public class Tasks
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int AssignedUserId { get; set; }
        public User AssignedUser { get; set; }

    }
}