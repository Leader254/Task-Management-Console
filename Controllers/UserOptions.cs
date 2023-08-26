using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskMgmt.Context;

namespace TaskMgmt.Controllers
{
    public class UserOptions
    {
        public void ViewTaskAssignedToYou()
        {
            using (var db = new TaskContext())
            {
                // get the user id 
                var tasks = db.Tasks.Where(t => t.AssignedUserId == 2).ToList();
                foreach (var task in tasks)
                {
                    Console.WriteLine(task.Title);
                }
            }
        }
        public void UpdateTaskStatus()
        {
            Console.WriteLine("UpdateTaskStatus");
        }
    }
}