using Microsoft.EntityFrameworkCore;
using TaskMgmt.Context;
using TaskMgmt.Models;

namespace TaskMgmt.Controllers
{
    public class ProjectsCRUD
    {
        // Create Project - Done
        public void CreateProject()
        {
            using (var db = new TaskContext())
            {
                Console.WriteLine("Enter the name of the project:");
                string name = Console.ReadLine();
                if (string.IsNullOrEmpty(name))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Name cannot be empty!");
                    Console.ResetColor();
                    CreateProject();
                    return;
                }
                Console.WriteLine("Enter the description of the project:");
                string description = Console.ReadLine();
                if (string.IsNullOrEmpty(description))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Description cannot be empty!");
                    Console.ResetColor();
                    CreateProject();
                    return;
                }
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

        // Update Project - Done
        public void UpdateProject()
        {
            using (var db = new TaskContext())
            {
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
                Console.WriteLine("Enter the id of the project you want to update:");
                int id = Convert.ToInt32(Console.ReadLine());
                try
                {
                    var project = db.Projects.Find(id);
                    Console.Write($"Old name: =={project.Name}==, Please enter the new name of the project:");
                    string name = Console.ReadLine();
                    if (string.IsNullOrEmpty(name))
                    {
                        // leave the name as it is
                        name = project.Name;
                    }
                    Console.Write($"Old description: =={project.Description}==, Please enter the new description of the project:");
                    string description = Console.ReadLine();
                    if (string.IsNullOrEmpty(description))
                    {
                        // leave the description as it is
                        description = project.Description;
                    }
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
        // Delete Project - Done
        public void DeleteProject()
        {
            using (var db = new TaskContext())
            {
                List<Project> projects = db.Projects.Include(p => p.Tasks).ToList();
                foreach (Project project in projects)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Id: {project.Id}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Name: {project.Name}");
                    Console.WriteLine($"Description: {project.Description}");
                    Console.WriteLine($"Tasks: {project.Tasks.Count}, Status: {project.Tasks.FirstOrDefault()?.Status}");
                    Console.ResetColor();
                }

                Console.WriteLine("Enter the id of the project you want to delete:");
                int id = Convert.ToInt32(Console.ReadLine());
                Project projectToDelete = projects.FirstOrDefault(p => p.Id == id);

                if (projectToDelete != null)
                {
                    if (projectToDelete.Tasks.Count == 0)
                    {
                        db.Projects.Remove(projectToDelete);
                        db.SaveChanges();
                        const string message = "Project deleted successfully!";
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(message);
                        Console.ResetColor();
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Project cannot be deleted as it has tasks assigned to it!");
                        Console.ResetColor();
                        Console.ReadKey();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Project not found!");
                    Console.ResetColor();
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
                List<Project> projects = db.Projects.Include(p => p.Tasks).ToList();
                foreach (Project project in projects)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Id: {project.Id}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Project Name: {project.Name}");
                    Console.WriteLine($"Description: {project.Description}");
                    foreach (Tasks single in project.Tasks)
                    {
                        Console.WriteLine($"{single.Id}. Task: {single.Title}, Status: {single.Status}");
                    }
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
                            Console.WriteLine($"Task: {user.Task.Title}, Status: {user.Task.Status}");
                        }
                        Console.ResetColor();
                    }
                    Console.WriteLine("Enter the user id to assign the task:");
                    int assignedUserId = Convert.ToInt32(Console.ReadLine());
                    assignedUser = db.Users.Include(u => u.Task).FirstOrDefault(u => u.Id == assignedUserId);
                    if (assignedUser != null && assignedUser.Task != null && assignedUser.Task.Status != Status.Completed)
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
                        // if the user task status is completed, assign the task to the user
                        if (assignedUser != null && assignedUser.Task != null && assignedUser.Task.Status == Status.Completed)
                        {
                            assignedUser.Task = null;
                        }

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
            }
        }

        // View all tasks - Done
        public void ViewAllTasks()
        {
            using (var db = new TaskContext())
            {
                List<Tasks> tasks = db.Tasks.Include(t => t.Project).Include(t => t.AssignedUser).ToList();
                foreach (Tasks task in tasks)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Id: {task.Id}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Title: {task.Title}");
                    Console.WriteLine($"Description: {task.Description}");
                    Console.WriteLine($"Assigned to: {task.AssignedUser.Username}");
                    Console.WriteLine($"Status: {task.Status}");
                    Console.ResetColor();
                }
                // call the method to view all tasks by status
                Console.Write("Do you want to delete a task? (y/n)");
                string choice = Console.ReadLine();
                if (choice == "y" || choice == "Y" || choice == "yes" || choice == "Yes")
                {
                    DeleteTask();
                }
                else
                {
                    return;
                }
            }
        }
        // Update Task
        public void UpdateTask()
        {
            using (var db = new TaskContext())
            {
                // View all task and allow user to choose which task to update
                List<Tasks> tasks = db.Tasks.Include(p => p.AssignedUser).ToList();
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
                Console.WriteLine("Enter the id of the task you want to update:");
                int id = Convert.ToInt32(Console.ReadLine());
                try
                {
                    // if the status is completed or in progress, do not allow the user to update the task
                    var task = db.Tasks.Find(id);
                    if (task.Status == Status.Completed || task.Status == Status.InProgress)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Task cannot be updated as it is in progress or completed!");
                        Console.ResetColor();
                        Console.ReadKey();
                        return;
                    }
                    Console.Write($"Old title: =={task.Title}==, Please enter the new title of the task:");
                    string title = Console.ReadLine();
                    if (string.IsNullOrEmpty(title))
                    {
                        // leave the title as it is
                        title = task.Title;
                    }
                    Console.Write($"Old description: =={task.Description}==, Please enter the new description of the task:");
                    string description = Console.ReadLine();
                    if (string.IsNullOrEmpty(description))
                    {
                        // leave the description as it is
                        description = task.Description;
                    }
                    Console.WriteLine($"Person assigned: =={task.AssignedUser.Username}==, Please enter the new person to assign the task:");
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
                    int assignedUserId = Convert.ToInt32(Console.ReadLine());
                    User assignedUser = db.Users.Find(assignedUserId);
                    if (assignedUser == null)
                    {
                        // leave the assigned user as it is
                        assignedUser = task.AssignedUser;
                    }
                    task.Title = title;
                    task.Description = description;
                    task.AssignedUser = assignedUser;
                    db.SaveChanges();

                    string message = "Task updated successfully!";
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

        // Delete Task - Done
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
                // if id is not valid, show a message that task is not found
                if (db.Tasks.Find(id) == null || db.Tasks.Find(id).Status == Status.InProgress)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Task not found or task is in progress!");
                    Console.ResetColor();
                    return;
                }
                var taskToDelete = db.Tasks.Find(id);
                // if task is not in progress, delete the task
                db.Tasks.Remove(taskToDelete);
                db.SaveChanges();
                const string message = "Task deleted successfully!";
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(message);
                Console.ResetColor();
                Console.ReadKey();
            }
        }

        // Delete User - Done
        public void DeleteUser()
        {
            using (var db = new TaskContext())
            {
                // View all users and allow user to choose which user to delete and if the user has a task assigned, do not allow the user to delete the user
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
                Console.Write("Enter the id of the user you want to delete:");
                int id = Convert.ToInt32(Console.ReadLine());
                User userToDelete = users.FirstOrDefault(u => u.Id == id);

                if (userToDelete != null)
                {
                    if (userToDelete.Task == null)
                    {
                        db.Users.Remove(userToDelete);
                        db.SaveChanges();
                        const string message = "User deleted successfully!";
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(message);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("User cannot be deleted as they have task assigned to it!");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("User not found!");
                    Console.ResetColor();
                }

            }
        }

    }
}