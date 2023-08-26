using Microsoft.EntityFrameworkCore;
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
                Console.WriteLine("Enter the description of the project:");
                string description = Console.ReadLine();
                Project project = new Project
                {
                    Name = name,
                    Description = description
                };
                db.Projects.Add(project);
                db.SaveChanges();
                const string message = "Project created successfully!";
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        // Update Project
        public void UpdateProject()
        {
            using (var db = new TaskContext())
            {
                Console.WriteLine("Enter the id of the project you want to update:");
                int id = Convert.ToInt32(Console.ReadLine());
                try
                {
                    var project = db.Projects.Find(id);
                    Console.Write($"Old name: {project.Name}, Please enter the new name of the project:");
                    string name = Console.ReadLine();
                    Console.Write($"Old description: {project.Description}, Please enter the new description of the project:");
                    string description = Console.ReadLine();
                    project.Name = name;
                    project.Description = description;
                    db.SaveChanges();

                    string message = "Project updated successfully!";
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(message);
                    Console.ResetColor();

                }
                catch (Exception)
                {
                    throw;
                }
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
                // are you sure you want to delete the project?
                Console.WriteLine("Are you sure you want to delete the project? (y/n)");
                string choice = Console.ReadLine();
                if (choice == "y || Y || yes || Yes")
                {
                    // delete the project
                    db.Projects.Remove(project);
                    db.SaveChanges();
                    // you have successfully deleted the project
                    const string message = "Project deleted successfully!";
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(message);
                    Console.ResetColor();
                    Console.ReadKey();
                }
                else
                {
                    // do not delete the project
                    Console.WriteLine("Project not deleted!");
                    Console.ReadKey();
                }
            }
        }

        // Create Task
        public void CreateTask()
        {
            using (var db = new TaskContext())
            {
                // get all the projects from the database to choose under which project the task should be created
                List<Project> projects = db.Projects.ToList();
                foreach (Project project in projects)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Id: {project.Id}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Name: {project.Name}");
                    Console.WriteLine($"Description: {project.Description}");
                    Console.ResetColor();
                }
                Console.WriteLine("Enter the project id under which you want to create the task:");
                int projectId = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter the title of the task:");
                string title = Console.ReadLine();
                Console.WriteLine("Enter the description of the task:");
                string description = Console.ReadLine();

                User assignedUser = null;
                while (assignedUser == null)
                {
                    // get all the users from the database to choose which user to assign the task with their status if they have a task assigned or not
                    List<User> users = db.Users.Include(u => u.Task).ToList();
                    foreach (User user in users)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Id: {user.Id}");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Username: {user.Username}");
                        Console.WriteLine($"Role: {user.Role}");
                        if (user.Task != null)
                        {
                            Console.WriteLine($"Task: {user.Task.Title}");
                        }
                        Console.ResetColor();
                    }
                    Console.WriteLine("Enter the user id to assign the task:");
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
                    // if the user has a role of admin, show a message that admin cannot be assigned a task
                    else if (assignedUser != null && assignedUser.Role == Role.Admin)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Admin cannot be assigned a task! Choose another user.");
                        Console.ResetColor();
                        assignedUser = null; // Reset assignedUser to null to loop again
                    }
                    else
                    {
                        // if the user is not assigned a task, assign the task to the user
                        assignedUser = db.Users.Find(assignedUserId);
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


        // View all tasks
        public void ViewAllTasks()
        {
            using (var db = new TaskContext())
            {
                List<Tasks> tasks = db.Tasks.ToList();
                foreach (Tasks task in tasks)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Id: {task.Id}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Title: {task.Title}");
                    Console.WriteLine($"Description: {task.Description}");
                    Console.WriteLine($"Status: {task.Status}");
                    Console.ResetColor();
                }
                // call the method to view all tasks by status
                Console.WriteLine("Do you want to view all tasks by status? (y/n)");
                string choice = Console.ReadLine();
                if (choice == "y")
                {
                    ViewAllTasksByStatus();
                }
            }
        }
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
                try
                {
                    var task = db.Tasks.Find(id);
                    Console.Write($"Old title: {task.Title}, Please enter the new title of the task:");
                    string title = Console.ReadLine();
                    Console.Write($"Old description: {task.Description}, Please enter the new description of the task:");
                    string description = Console.ReadLine();
                    task.Title = title;
                    task.Description = description;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        // Delete Task
        public void DeleteTask()
        {
            using (var db = new TaskContext())
            {
                // View all task and allow user to choose which task to delete
                List<Tasks> tasks = db.Tasks.ToList();
                foreach (Tasks task in tasks)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Id: {task.Id}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Title: {task.Title}");
                    Console.WriteLine($"Description: {task.Description}");
                    Console.WriteLine($"Status: {task.Status}");
                    Console.ResetColor();
                }
                Console.WriteLine("Enter the id of the task you want to delete:");
                int id = Convert.ToInt32(Console.ReadLine());
                Tasks taskToDelete = db.Tasks.Find(id);
                db.Tasks.Remove(taskToDelete);
                db.SaveChanges();
                // you have successfully deleted the task
                const string message = "Task deleted successfully!";
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(message);
                Console.ResetColor();
                Console.ReadKey();
            }
        }

        // Delete User
        public void DeleteUser()
        {
            using (var db = new TaskContext())
            {
                // View all users and allow user to choose which user to delete
                List<User> users = db.Users.ToList();
                foreach (User user in users)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Id: {user.Id}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Username: {user.Username}");
                    Console.WriteLine($"Role: {user.Role}");
                    Console.ResetColor();
                }
                Console.WriteLine("Enter the id of the user you want to delete:");
                int id = Convert.ToInt32(Console.ReadLine());
                User userToDelete = db.Users.Find(id);
                db.Users.Remove(userToDelete);
                db.SaveChanges();
                const string message = "User deleted successfully!";
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(message);
                Console.ResetColor();
                Console.ReadKey();
            }
        }

    }
}