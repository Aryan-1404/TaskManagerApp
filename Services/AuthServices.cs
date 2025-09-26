using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class AuthServices
    {
        private readonly FileService _fileService;
        private readonly List<User> _users;

        public AuthServices(FileService fileService)
        {
            if (fileService == null)
                throw new ArgumentNullException(nameof(fileService));

            _fileService = fileService;

            try
            {
                _users = _fileService.LoadUsers() ?? new List<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthServices] Failed to load users from file: {ex.Message}");
                _users = new List<User>();
            }
        }

        public User? Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return null;

                var encrypted = CryptoHelper.Encrypt(password);
                return _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                                                  && u.PasswordEncrypted == encrypted);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Login failed. {ex.Message}");
                return null;
            }
        }

        public bool ChangePassword(User user, string newPassword)
        {
            try
            {
                if (user == null || string.IsNullOrWhiteSpace(newPassword)) return false;

                var u = _users.FirstOrDefault(x => x.Id == user.Id);
                if (u == null) return false;

                u.PasswordEncrypted = CryptoHelper.Encrypt(newPassword);
                _fileService.SaveUsers(_users);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] ChangePassword failed. {ex.Message}");
                return false;
            }
        }
    }
}
