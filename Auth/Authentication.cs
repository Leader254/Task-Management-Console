using System;
using System.Linq;
using TaskMgmt.Context;
using TaskMgmt.Models;
using TaskMgmt.Controllers;
using TaskMgmt.Utils;

namespace TaskMgmt.Auth
{
    public class Authentication
    {
        private enum LoginOrRegister
        {
            Login = 1,
            Register = 2
        }
        private static int loggedInUser;

        public void ShowOptions()
        {
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.Write("Enter your choice: ");
            int choice = ValidationUtils.ReadValidInt("Enter your choice: ", "Invalid choice");

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
                ShowOptions();
            }
        }

        public void RegisterInput()
        {
            string username = ValidationUtils.ReadNonEmptyString("Enter username: ", "Username cannot be empty.");
            string password = ValidationUtils.ReadNonEmptyString("Enter password: ", "Password cannot be empty.");

            Console.WriteLine("Enter role: ");
            foreach (var roleOption in Enum.GetValues(typeof(Role)))
            {
                Console.WriteLine($"{(int)roleOption}. {roleOption}");
            }
            string selectedRole = ValidationUtils.ReadNonEmptyString("Select role by entering the corresponding number: ", "Invalid role selection.");

            // convert the username to lowercase
            string convertedUsername = username.ToLower();
            // check if the username already exists
            using (var db = new TaskContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == convertedUsername);
                if (user != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Username already exists");
                    Console.ResetColor();
                    RegisterInput();
                }
                else
                {
                    Register(convertedUsername, password, selectedRole);
                }
            }
        }

        public void LoginInput()
        {
            string username = ValidationUtils.ReadNonEmptyString("Enter username: ", "Username cannot be empty.");
            string password = ValidationUtils.ReadNonEmptyString("Enter password: ", "Password cannot be empty.");
            bool isLogin = Login(username, password);
        }


        public static void Register(string convertedUsername, string password, string roleName)
        {
            using (var db = new TaskContext())
            {
                var user = new User
                {
                    Username = convertedUsername,
                    Password = password,
                    Role = (Role)Enum.Parse(typeof(Role), roleName)
                };
                // success message
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("User registered successfully!");
                Console.ResetColor();
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

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
            int choice = ValidationUtils.ReadValidInt("Enter your choice: ", "Invalid choice");

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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice");
                    Console.ResetColor();
                    AdminMenu();
                    break;
            }
        }

        public static void UserMenu()
        {
            Console.WriteLine("1. View task assigned to you");
            Console.WriteLine("2. Update task status");
            Console.Write("Enter your choice: ");
            int choice = ValidationUtils.ReadValidInt("Enter your choice: ", "Invalid choice");

            UserOptions userOptions = new UserOptions();
            switch (choice)
            {
                case 1:
                    userOptions.ViewTaskAssignedToYou(int.Parse(loggedInUser.ToString()));
                    break;
                case 2:
                    userOptions.UpdateTaskStatus(int.Parse(loggedInUser.ToString()));
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
}
