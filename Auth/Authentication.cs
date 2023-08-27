using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskMgmt.Context;
using TaskMgmt.Models;
using TaskMgmt.Controllers;

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
        private static int loggedInUser;
        // show options for login or register
        public void ShowOptions()
        {
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.Write("Enter your choice: ");
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
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            Console.Write("Enter role: ");

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
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            bool isLogin = Login(username, password);

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
                    loggedInUser = user.Id;
                    if (user.Role == Role.Admin)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Welcome Back {user.Username}!!");
                        Console.ResetColor();
                        AdminMenu();
                    }
                    else if (user.Role == Role.User)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Welcome Back {user.Username}!!");
                        Console.ResetColor();
                        UserMenu();
                    }
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid username or password");
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
            Console.WriteLine("5. View all tasks");
            Console.WriteLine("6. Update task");
            Console.WriteLine("7. Delete task");
            Console.WriteLine("8. Delete user");
            Console.Write("Enter your choice: ");
            int choice = Convert.ToInt32(Console.ReadLine());

            ProjectsCRUD projectsCRUD = new ProjectsCRUD();
            switch (choice)
            {
                case 1:
                    projectsCRUD.CreateProject();
                    break;
                case 2:
                    projectsCRUD.UpdateProject();
                    break;
                case 3:
                    projectsCRUD.DeleteProject();
                    break;
                case 4:
                    projectsCRUD.CreateTask();
                    break;
                case 5:
                    projectsCRUD.ViewAllTasks();
                    break;
                case 6:
                    projectsCRUD.UpdateTask();
                    break;
                case 7:
                    projectsCRUD.DeleteTask();
                    break;
                case 8:
                    projectsCRUD.DeleteUser();
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }

        }

        // user menu
        public static void UserMenu()
        {
            Console.WriteLine("1. View task assigned to you");
            Console.WriteLine("2. Update task status");
            Console.Write("Enter your choice: ");
            int choice = Convert.ToInt32(Console.ReadLine());

            UserOptions userOptions = new UserOptions();
            switch (choice)
            {
                case 1:
                    userOptions.ViewTaskAssignedToYou();
                    break;
                case 2:
                    userOptions.UpdateTaskStatus();
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
}