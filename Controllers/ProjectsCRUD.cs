using Microsoft.EntityFrameworkCore;
using TaskMgmt.Context;
using TaskMgmt.Models;
using TaskMgmt.Auth;
using TaskMgmt.Utils;

namespace TaskMgmt.Controllers
{
    public class ProjectsCRUD
    {
        // Create Project - Done - Validation
        public void CreateProject()
        {
            using (var db = new TaskContext())
            {
                string name = "";
                while (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("Enter the name of the project:");
                    name = Console.ReadLine().Trim(); // Trim to remove leading and trailing spaces
                    if (string.IsNullOrEmpty(name))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Name cannot be empty!");
                        Console.ResetColor();
                    }
                }

                string description = "";
                while (string.IsNullOrEmpty(description))
                {
                    Console.WriteLine("Enter the description of the project:");
                    description = Console.ReadLine().Trim(); // Trim to remove leading and trailing spaces
                    if (string.IsNullOrEmpty(description))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Description cannot be empty!");
                        Console.ResetColor();
                    }
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

        // Update Project - Done - Validation
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
                // validate the input id to confirm if the input is valid and if the input is valid, check if the project is found and if the project is not found, show a message that project not found
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input. Please enter a valid project id.");
                    Console.ResetColor();
                    // call the method to update project again
                    UpdateProject();
                }
                Project projectToUpdate = projects.FirstOrDefault(p => p.Id == id);
                if (projectToUpdate == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Project not found!");
                    Console.ResetColor();
                    // call the method to update project again
                    UpdateProject();
                }
                Console.Write($"Old name: =={projectToUpdate.Name}==, Please enter the new name of the project:");
                string name = Console.ReadLine();
                if (string.IsNullOrEmpty(name))
                {
                    // leave the name as it is
                    name = projectToUpdate.Name;
                }
                Console.Write($"Old description: =={projectToUpdate.Description}==, Please enter the new description of the project:");
                string description = Console.ReadLine();
                if (string.IsNullOrEmpty(description))
                {
                    // leave the description as it is
                    description = projectToUpdate.Description;
                }
                projectToUpdate.Name = name;
                projectToUpdate.Description = description;
                db.SaveChanges();

            }
        }
        // Delete Project - Done - Validation
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
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input. Please enter a valid project id.");
                    Console.ResetColor();
                    // call the method to delete project again
                    DeleteProject();
                }

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
                        return;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Project cannot be deleted as it has tasks assigned to it!");
                        Console.ResetColor();
                        // call the admin menu again
                        Authentication.AdminMenu();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Project not found!");
                    Console.ResetColor();
                    // call the method to delete project again
                    DeleteProject();
                }
            }
        }
        // Create Task - Done - Validation
        public void CreateTask()
        {
            using (var db = new TaskContext())
            {
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

                int projectId = ValidationUtils.ReadValidInt("Enter the project id under which you want to create the task: ", "Invalid input. Please enter a valid project id.");

                Project projectToAssign = db.Projects.Include(p => p.Tasks).FirstOrDefault(p => p.Id == projectId);
                if (projectToAssign == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Project not found!");
                    Console.ResetColor();
                    CreateTask();
                }

                string title = ValidationUtils.ReadNonEmptyString("Enter the title of the task: ", "Title cannot be empty.");
                string description = ValidationUtils.ReadNonEmptyString("Enter the description of the task: ", "Description cannot be empty.");

                User assignedUser = null;
                while (assignedUser == null)
                {
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
                    int assignedUserId = ValidationUtils.ReadValidInt("Enter the user id to assign the task: ", "Invalid input. Please enter a valid user id.");

                    assignedUser = db.Users.Include(u => u.Task).FirstOrDefault(u => u.Id == assignedUserId);

                    if (assignedUser != null && assignedUser.Task != null && assignedUser.Task.Status != Status.Completed)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("User already has a task assigned! Choose another user.");
                        Console.ResetColor();
                        assignedUser = null; // Reset assignedUser to null to loop again
                    }
                    else if (assignedUser != null && assignedUser.Role == Role.Admin)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Admin cannot be assigned a task! Choose another user.");
                        Console.ResetColor();
                        assignedUser = null; // Reset assignedUser to null to loop again
                    }
                    else
                    {
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

        // View all tasks - Done - Validation
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
                    Console.WriteLine("Invalid choice");
                    // call the admin menu again
                    Authentication.AdminMenu();
                }
            }
        }
        // Update Task - Done - Validation
        public void UpdateTask()
        {
            using (var db = new TaskContext())
            {
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

                int id = ValidationUtils.ReadValidInt("Enter the id of the task you want to update:", "Invalid input. Please enter a valid task id.");

                var taskToUpdate = db.Tasks.Find(id);
                if (taskToUpdate == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Task not found!");
                    UpdateTask();
                }

                if (taskToUpdate.Status == Status.NotStarted || taskToUpdate.Status == Status.InProgress)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Task cannot be updated as it is not completed!");
                    Console.ResetColor();
                    UpdateTask();
                }

                string newTitle = ValidationUtils.ReadNonEmptyString($"Old title: =={taskToUpdate.Title}==, Please enter the new title of the task:", "Title cannot be empty.");
                string newDescription = ValidationUtils.ReadNonEmptyString($"Old description: =={taskToUpdate.Description}==, Please enter the new description of the task:", "Description cannot be empty.");

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

                int assignedUserId = ValidationUtils.ReadValidInt("Enter the user id to assign the task:", "Invalid input. Please enter a valid user id.");
                User assignedUser = db.Users.Find(assignedUserId);
                if (assignedUser == null)
                {
                    assignedUser = taskToUpdate.AssignedUser;
                }

                taskToUpdate.Title = newTitle;
                taskToUpdate.Description = newDescription;
                taskToUpdate.AssignedUser = assignedUser;
                db.SaveChanges();

                const string message = "Task updated successfully!";
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(message);
                Console.ResetColor();
                Authentication.AdminMenu();
            }
        }


        // Delete Task - Done - Validation
        public void DeleteTask()
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

                int id = ValidationUtils.ReadValidInt("Enter the id of the task you want to delete:", "Invalid input. Please enter a valid task id.");
                var taskToDelete = db.Tasks.Find(id);
                if (taskToDelete == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Task not found!");
                    Console.ResetColor();
                    DeleteTask();
                }
                else if (taskToDelete.Status == Status.NotStarted || taskToDelete.Status == Status.InProgress)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Task cannot be deleted as it is not completed!");
                    Console.ResetColor();
                    DeleteTask();
                }
                else
                {
                    db.Tasks.Remove(taskToDelete);
                    db.SaveChanges();
                    const string message = "Task deleted successfully!";
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(message);
                    Console.ResetColor();
                    // call the admin menu again
                    Authentication.AdminMenu();
                }
            }
        }


        // Delete User - Done - Validation
        public void DeleteUser()
        {
            using (var db = new TaskContext())
            {
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

                int userId = ValidationUtils.ReadValidInt("Enter the id of the user you want to delete: ", "Invalid input. Please enter a valid user id.");
                User userToDelete = users.FirstOrDefault(u => u.Id == userId);

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
                        // call the admin menu again
                        Authentication.AdminMenu();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("User cannot be deleted as they have a task assigned to it!");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("User not found!");
                    Console.ResetColor();
                    DeleteUser();
                }
            }
        }


    }
}