using Models;
using Services;
using System;

namespace TaskManagerApp
{
    public static class UtilityClass
    {
        // Read Password and Masking It
        public static string? ReadPassword(int minLength = 4)
        {
            string pass = string.Empty;

            try
            {
                ConsoleKeyInfo key;

                do
                {
                    key = Console.ReadKey(true);

                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        pass += key.KeyChar;
                        Console.Write("*");
                    }
                    else if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, pass.Length - 1);
                        int pos = Console.CursorLeft;
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                } while (key.Key != ConsoleKey.Enter);

                Console.WriteLine();

                if (pass.Length < minLength)
                {
                    Console.WriteLine($"\nPassword must be at least {minLength} characters long.");
                    return null; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Error] Failed to read password: {ex.Message}");
                return null;
            }

            return pass;
        }

        // Change password
        public static void ChangeUserPassword(AuthServices authServices, User user)
        {
            try
            {
                if (authServices == null)
                {
                    Console.WriteLine("[Error] AuthServices is not available.");
                    return;
                }

                if (user == null)
                {
                    Console.WriteLine("[Error] User is not valid.");
                    return;
                }

                Console.Write("\nEnter New Password: ");
                var newPassword = ReadPassword();

                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    Console.WriteLine("\nPassword cannot be empty.");
                }
                else
                {
                    bool success = authServices.ChangePassword(user, newPassword);
                    Console.WriteLine(success ? "\nPassword changed successfully." : "\nFailed to change password.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Error] Failed to change user password: {ex.Message}");
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}
