using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskMgmt.Context;
using TaskMgmt.Models;

namespace TaskMgmt.Auth
{
    // enum login or register
    public enum LoginOrRegister
    {
        Login = 1,
        Register = 2
    }
    public class Authentication
    {
        // show options for login or register
        public void ShowOptions()
        {
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("Enter your choice: ");
            int choice = Convert.ToInt32(Console.ReadLine());
            if (choice == (int)LoginOrRegister.Login)
            {
                LoginInput();
            }
            else if (choice == (int)LoginOrRegister.Register)
            {
                RegisterInput();
            }
            else
            {
                Console.WriteLine("Invalid choice");
            }
        }
        // input fields for register
        public void RegisterInput()
        {
            Console.WriteLine("Enter username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Enter password: ");
            string password = Console.ReadLine();
            Console.WriteLine("Enter role: ");

            foreach (var roleOption in Enum.GetValues(typeof(Role)))
            {
                Console.WriteLine($"{(int)roleOption}. {roleOption}");
            }

            string selectedRole = Console.ReadLine();
            Register(username, password, selectedRole);
        }

        // input fields for login
        public void LoginInput()
        {
            Console.WriteLine("Choose your role: ");
            foreach (var role in Enum.GetValues(typeof(Role)))
            {
                Console.WriteLine($"{(int)role}. {role}");
            }
            string selectedRole = Console.ReadLine();
            Console.WriteLine("Enter username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Enter password: ");
            string password = Console.ReadLine();
            bool isLogin = Login(username, password);
            if (isLogin)
            {
                Console.WriteLine("Login successful");
            }
            else
            {
                Console.WriteLine("Login failed, uer not found!!");
            }
        }
        // Register method
        public static void Register(string username, string password, string roleName)
        {
            using (var db = new TaskContext())
            {
                var user = new User
                {
                    Username = username,
                    Password = password,
                    Role = (Role)Enum.Parse(typeof(Role), roleName)
                };
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        // Login method
        public static bool Login(string username, string password)
        {
            using (var db = new TaskContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                    if (user.Role == Role.Admin)
                    {
                        AdminMenu(); // Display admin privileges
                    }
                    else
                    {
                        UserMenu(); // Display regular user privileges
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // Admin menu
        public static void AdminMenu()
        {
            Console.WriteLine("1. Create A Project");
            Console.WriteLine("2. Update Project");
            Console.WriteLine("3. Delete Project");
            Console.WriteLine("4. Create Tasks");
            Console.WriteLine("5. View all tasks by status");
            Console.WriteLine("6. Update task");
            Console.WriteLine("7. Delete task");
            Console.WriteLine("8. Delete user");
            Console.WriteLine("9. Logout");
            Console.WriteLine("Enter your choice: ");
            int choice = Convert.ToInt32(Console.ReadLine());
            // switch (choice)
            // {
            //     case 1:
            //         CreateProject();
            //         break;
            //     case 2:
            //         UpdateProject();
            //         break;
            //     case 3:
            //         DeleteProject();
            //         break;
            //     case 4:
            //         ViewAllTasksByStatus();
            //         break;
            //     case 5:
            //         UpdateTask();
            //         break;
            //     case 6:
            //         DeleteTask();
            //         break;
            //     case 7:
            //         DeleteUser();
            //         break;
            //     case 8:
            //         Logout();
            //         break;
            //     default:
            //         Console.WriteLine("Invalid choice");
            //         break;
            // }

        }

        // user menu
        public static void UserMenu()
        {
            Console.WriteLine("1. Create task");
            Console.WriteLine("2. View all tasks");
            Console.WriteLine("3. View all tasks by status");
            Console.WriteLine("4. View all tasks by project");
            Console.WriteLine("5. View all tasks by project and status");
            Console.WriteLine("6. Update task");
            Console.WriteLine("7. Logout");
            Console.WriteLine("Enter your choice: ");
            int choice = Convert.ToInt32(Console.ReadLine());
            // switch (choice)
            // {
            //     case 1:
            //         CreateTask();
            //         break;
            //     case 2:
            //         ViewAllTasks();
            //         break;
            //     case 3:
            //         ViewAllTasksByStatus();
            //         break;
            //     case 4:
            //         ViewAllTasksByProject();
            //         break;
            //     case 5:
            //         ViewAllTasksByProjectAndStatus();
            //         break;
            //     case 6:
            //         UpdateTask();
            //         break;
            //     case 7:
            //         Logout();
            //         break;
            //     default:
            //         Console.WriteLine("Invalid choice");
            //         break;
            // }
        }
    }
}