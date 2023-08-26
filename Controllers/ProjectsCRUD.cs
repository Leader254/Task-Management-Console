using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskMgmt.Auth;
using TaskMgmt.Context;
using TaskMgmt.Models;

namespace TaskMgmt.Controllers
{
    public class ProjectsCRUD
    {
        // Create Project
        public void CreateProject()
        {
            using (var db = new TaskContext())
            {
                Console.WriteLine("Enter the name of the project:");
                string name = Console.ReadLine();
                Project project = new Project
                {
                    Name = name
                };
                db.Projects.Add(project);
                db.SaveChanges();
            }
        }

        // Update Project
        public void UpdateProject()
        {
            using (var db = new TaskContext())
            {
                Console.WriteLine("Enter the id of the project you want to update:");
                int id = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter the new name of the project:");
                string name = Console.ReadLine();
                Project project = db.Projects.Find(id);
                project.Name = name;
                db.SaveChanges();
            }
        }

        // Delete Project
        public void DeleteProject()
        {
            using (var db = new TaskContext())
            {
                Console.WriteLine("Enter the id of the project you want to delete:");
                int id = Convert.ToInt32(Console.ReadLine());
                Project project = db.Projects.Find(id);
                db.Projects.Remove(project);
                db.SaveChanges();
            }
        }

        // Create Task
        public void CreateTask()
        {
            using (var db = new TaskContext())
            {
                Console.WriteLine("Enter the title of the task:");
                string title = Console.ReadLine();
                Console.WriteLine("Enter the description of the task:");
                string description = Console.ReadLine();
                Console.WriteLine("Enter the project id of the task:");
                int projectId = Convert.ToInt32(Console.ReadLine());

                User assignedUser = null;
                while (assignedUser == null)
                {
                    Console.WriteLine("Enter the assigned user id of the task:");
                    int assignedUserId = Convert.ToInt32(Console.ReadLine());

                    // Check if the user is already assigned a task
                    assignedUser = db.Users.Include(u => u.Task).FirstOrDefault(u => u.Id == assignedUserId);
                    if (assignedUser != null && assignedUser.Task != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("User already has a task assigned! Choose another user.");
                        Console.ResetColor();
                        assignedUser = null; // Reset assignedUser to null to loop again
                    }
                }

                Tasks task = new Tasks
                {
                    Title = title,
                    Description = description,
                    ProjectId = projectId,
                    AssignedUserId = assignedUser.Id
                };
                db.Tasks.Add(task);
                db.SaveChanges();

                const string message = "Task created successfully!";
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(message);
                Console.ResetColor();
                Console.ReadKey();
            }
        }


        // View all tasks by status
        // ViewAllTasksByStatus
        public void ViewAllTasksByStatus()
        {
            using (var db = new TaskContext())
            {
                // loop through the enum values
                foreach (Status status in Enum.GetValues(typeof(Status)))
                {
                    Console.WriteLine($"{(int)status}. {status}");
                }
                Console.WriteLine("Enter the status of the task:");
                string statusInput = Console.ReadLine();
                Status statusEnum = (Status)Enum.Parse(typeof(Status), statusInput);
                List<Tasks> tasks = db.Tasks.Where(t => t.Status == statusEnum).ToList();
                foreach (Tasks task in tasks)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Id: {task.Id}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Title: {task.Title}");
                    Console.WriteLine($"Description: {task.Description}");
                    Console.WriteLine($"Status: {task.Status}");
                    Console.ResetColor();
                    // Console.WriteLine($"Assigned to: {task.AssignedUser.Username}");
                }

            }
        }

        // Update Task
        public void UpdateTask()
        {
            using (var db = new TaskContext())
            {
                Console.WriteLine("Enter the id of the task you want to update:");
                int id = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter the new title of the task:");
                string title = Console.ReadLine();
                Console.WriteLine("Enter the new description of the task:");
                string description = Console.ReadLine();
                Console.WriteLine("Enter the new status of the task:");
                string status = Console.ReadLine();
                Console.WriteLine("Enter the new project id of the task:");
                int projectId = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter the new assigned user id of the task:");
                int assignedUserId = Convert.ToInt32(Console.ReadLine());
                Tasks task = db.Tasks.Find(id);
                task.Title = title;
                task.Description = description;
                task.Status = (Status)Enum.Parse(typeof(Status), status);
                task.ProjectId = projectId;
                task.AssignedUserId = assignedUserId;
                db.SaveChanges();
            }
        }

        // Delete Task
        public void DeleteTask()
        {
            using (var db = new TaskContext())
            {
                Console.WriteLine("Enter the id of the task you want to delete:");
                int id = Convert.ToInt32(Console.ReadLine());
                Tasks task = db.Tasks.Find(id);
                db.Tasks.Remove(task);
                db.SaveChanges();
            }
        }

        // Delete User
        public void DeleteUser()
        {
            using (var db = new TaskContext())
            {
                Console.WriteLine("Enter the id of the user you want to delete:");
                int id = Convert.ToInt32(Console.ReadLine());
                User user = db.Users.Find(id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

    }
}