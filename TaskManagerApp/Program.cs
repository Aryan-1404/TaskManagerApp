using Models;
using Services;
using System;
using TaskManagerApp.Handlers;

namespace TaskManagerApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            FileService fileService = new FileService();
            UserService userService = new UserService(fileService);
            ProjectService projectService = new ProjectService(fileService);
            TaskService taskService = new TaskService(fileService);
            AuthServices authServices = new AuthServices(fileService);

            while (true)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("=======================================");
                    Console.WriteLine("         TASK MANAGER - MAIN MENU      ");
                    Console.WriteLine("=======================================");
                    Console.WriteLine("1) Register (Create User)");
                    Console.WriteLine("2) Login");
                    Console.WriteLine("3) Exit");
                    Console.WriteLine("=======================================");
                    Console.Write("Choose an option: ");

                    var choice = Console.ReadLine()?.Trim();

                    if (choice == "1")
                    {
                        RegisterFlow(userService);
                    }
                    else if (choice == "2")
                    {
                        var user = LoginFlow(authServices);
                        if (user != null)
                        {
                            if (user.Role == Role.Admin)
                                AdminMenuHandler.AdminMenu(userService, user, projectService, taskService, authServices);
                            else
                                UserMenuHandler.UserMenu(taskService, userService, user, projectService, authServices);
                        }
                    }
                    else if (choice == "3")
                    {
                        Console.WriteLine("\nExiting... Goodbye!");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid option! Press Enter to try again...");
                        Console.ReadLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nUnexpected error: {ex.Message}");
                    Console.WriteLine("Press Enter to return to the main menu...");
                    Console.ReadLine();
                }
            }
        }

        public static void RegisterFlow(UserService auth)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("----- Register New User -----");
                Console.Write("Username: ");
                var username = Console.ReadLine()?.Trim();
                Console.Write("Password: ");
                var password = UtilityClass.ReadPassword();
                var created = auth.Register(username, password);
                Console.WriteLine(created ? "User registered successfully." : "Failed to register user (username may exist).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError during registration: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }


        public static User LoginFlow(AuthServices authServices)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("----- User Login -----");
                Console.Write("Username: ");
                var username = Console.ReadLine()?.Trim();
                Console.Write("Password: ");
                var password = UtilityClass.ReadPassword();
                var user = authServices.Login(username, password);
                if (user == null)
                {
                    Console.WriteLine("Login failed. Press Enter to return...");
                    Console.ReadLine();
                    return null;
                }
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError during login: {ex.Message}");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return null;
            }
        }


    }
}
