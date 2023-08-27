using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskMgmt.Context;
using TaskMgmt.Models;

namespace TaskMgmt.Controllers
{
    public class UserOptions
    {
        // view assigned task - done
        public void ViewTaskAssignedToYou(int loggedInUser)
        {
            using (var db = new TaskContext())
            {
                var tasks = db.Tasks.Where(t => t.AssignedUserId == loggedInUser).ToList();
                foreach (var task in tasks)
                {
                    Console.Write($"Hello, you have been assigned the following tasks: {task.Title} \nChoose an option to Update task status (y/n)");
                    string choice = Console.ReadLine();
                    if (choice == "y")
                    {
                        UpdateTaskStatus(loggedInUser);
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice");
                    }
                }
            }
        }

        // update task status - done
        public void UpdateTaskStatus(int loggedInUser)
        {
            using (var db = new TaskContext())
            {
                // List the tasks assigned to the user
                var tasks = db.Tasks.Where(t => t.AssignedUserId == loggedInUser).ToList();
                foreach (var task in tasks)
                {
                    Console.WriteLine($"ID: {task.Id} - {task.Title} - {task.Status}");
                }

                Console.Write("Enter the ID of the task you want to update: ");
                int taskId = Convert.ToInt32(Console.ReadLine());

                var taskToUpdate = db.Tasks.FirstOrDefault(t => t.Id == taskId && t.AssignedUserId == loggedInUser);

                if (taskToUpdate != null)
                {
                    Console.WriteLine($"Current Status: {taskToUpdate.Status}");
                    Console.WriteLine("Select new status:");
                    foreach (var statusOption in Enum.GetValues(typeof(Status)))
                    {
                        Console.WriteLine($"{(int)statusOption}. {statusOption}");
                    }
                    string selectedStatus = Console.ReadLine();
                    if (Enum.TryParse(selectedStatus, out Status newStatus))
                    {
                        taskToUpdate.Status = newStatus;
                        db.SaveChanges();
                        Console.WriteLine("Task status updated successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Invalid status.");
                    }
                }
                else
                {
                    Console.WriteLine("Task not found or not assigned to you.");
                }
            }
        }

    }
}