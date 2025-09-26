using Helpers;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerApp.Handlers.AdminMenu
{
    public static class AdminUserManagementMenu
    {
       
        public static void UserManagementMenu(UserService userService, AuthServices authServices, User admin)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=======================================");
                Console.WriteLine("          USER MANAGEMENT MENU          ");
                Console.WriteLine("=======================================");
                Console.WriteLine("1) Create User");
                Console.WriteLine("2) View All Users");
                Console.WriteLine("3) Edit User");
                Console.WriteLine("4) Delete User");
                Console.WriteLine("5) Back to Admin Menu");
                Console.WriteLine("=======================================");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        CreateUser(userService,admin);
                        break;
                    case "2":
                        ViewAllUsers(userService);
                        break;
                    case "3":
                        EditUser(userService);
                        break;
                    case "4":
                        DeleteUser(userService);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("\nInvalid option! Press Enter to continue...");
                        Console.ReadLine();
                        break;
                }
            }
        }



        private static void CreateUser(UserService userService, User currentUser)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("----- Create User -----");

                Console.Write("Username: ");
                var username = Console.ReadLine()?.Trim();

                Console.Write("Password: ");
                var password = UtilityClass.ReadPassword();

                Console.Write("Role (Admin/User): ");
                var roleInput = Console.ReadLine()?.Trim() ?? "User";
                var role = roleInput.Equals("Admin", StringComparison.OrdinalIgnoreCase) ? Role.Admin : Role.User;

                
                var created = userService.CreateUserByAdmin(currentUser, username, password, role);

                Console.WriteLine(created ? $"\nUser '{username}' created." : "\nFailed to create user.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private static void ViewAllUsers(UserService userService)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("----- User List -----");
                var allUsers = userService.GetAllUser();
                for (int i = 0; i < allUsers.Count; i++)
                    Console.WriteLine($"{i + 1}) {allUsers[i].Username} [{allUsers[i].Role}] (Id: {allUsers[i].Id})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error viewing users: {ex.Message}");
            }
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private static void EditUser(UserService userService)
        {
            try
            {
                var usersToEdit = userService.GetAllUser();
                if (usersToEdit.Count == 0)
                {
                    Console.WriteLine("No users available.");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("\nSelect User to edit:");
                for (int i = 0; i < usersToEdit.Count; i++)
                    Console.WriteLine($"{i + 1}) {usersToEdit[i].Username} ({usersToEdit[i].Role})");

                Console.Write("Enter number: ");
                if (int.TryParse(Console.ReadLine(), out int editIdx) && editIdx >= 1 && editIdx <= usersToEdit.Count)
                {
                    var userToEdit = usersToEdit[editIdx - 1];
                    Console.WriteLine($"\nEditing {userToEdit.Username}");

                    Console.Write("New Username (leave blank to keep same): ");
                    var newUsername = Console.ReadLine();
                    Console.Write("New Password (leave blank to keep same): ");
                    var newPassword = Console.ReadLine();
                    Console.Write("New Role (Admin/User - leave blank to keep same): ");
                    var newRoleInput = Console.ReadLine();

                    string finalUsername = string.IsNullOrWhiteSpace(newUsername) ? userToEdit.Username : newUsername;
                    string finalPassword = string.IsNullOrWhiteSpace(newPassword)
                        ? CryptoHelper.Decrypt(userToEdit.PasswordEncrypted)
                        : newPassword;

                    Role finalRole = userToEdit.Role;
                    if (!string.IsNullOrWhiteSpace(newRoleInput))
                    {
                        if (Enum.TryParse<Role>(newRoleInput, true, out var parsedRole))
                            finalRole = parsedRole;
                        else
                            Console.WriteLine("Invalid role entered. Keeping old role.");
                    }

                    bool updated = userService.EditUser(userToEdit.Id, finalUsername, finalPassword, finalRole);
                    Console.WriteLine(updated ? "User updated successfully." : "Update failed.");
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editing user: {ex.Message}");
            }
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private static void DeleteUser(UserService userService)
        {
            try
            {
                var usersToDelete = userService.GetAllUser();
                if (usersToDelete.Count == 0)
                {
                    Console.WriteLine("No users available.");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("\nSelect User to delete:");
                for (int i = 0; i < usersToDelete.Count; i++)
                    Console.WriteLine($"{i + 1}) {usersToDelete[i].Username} ({usersToDelete[i].Role})");

                Console.Write("Enter number: ");
                if (int.TryParse(Console.ReadLine(), out int delIdx) && delIdx >= 1 && delIdx <= usersToDelete.Count)
                {
                    var userToDelete = usersToDelete[delIdx - 1];
                    bool deleted = userService.DeleteUser(userToDelete.Id);
                    Console.WriteLine(deleted ? "User deleted successfully." : "Deletion failed.");
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
            }
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}
