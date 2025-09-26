using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class UserService
    {
        private readonly FileService _fileService;
        private List<User> _users;

        public UserService(FileService fileService)
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
                Console.WriteLine($"Failed to load users from file: {ex.Message}");
                _users = new List<User>(); 
            }

            try
            {
                EnsureDefaultAdmin();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to ensure default admin: {ex.Message}");
            }
        }

        public void EnsureDefaultAdmin()
        {
            try
            {
                if (!_users.Any(u => u.Role == Role.Admin))
                {
                    var admin = new User
                    {
                        Id = Guid.NewGuid(),
                        Username = "admin",
                        PasswordEncrypted = CryptoHelper.Encrypt("admin"),
                        Role = Role.Admin
                    };
                    _users.Add(admin);
                    _fileService.SaveUsers(_users);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error ensuring default admin: {ex.Message}");
            }
        }

        public bool Register(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return false;
                if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                    return false;

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    PasswordEncrypted = CryptoHelper.Encrypt(password),
                    Role = Role.User
                };
                _users.Add(user);
                _fileService.SaveUsers(_users);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error registering user: {ex.Message}");
                return false;
            }
        }

        public bool CreateUserByAdmin(User admin, string username, string password, Role role)
        {
            try
            {
                // Ensure only a real admin can create users
                if (admin == null || admin.Role != Role.Admin)
                {
                    Console.WriteLine("Only an admin can create users.");
                    return false;
                }

                // Validate username and password
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Username and password cannot be empty.");
                    return false;
                }

                // Check if username already exists
                if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("Username already exists.");
                    return false;
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    PasswordEncrypted = CryptoHelper.Encrypt(password),
                    Role = role
                };

                _users.Add(user);
                _fileService.SaveUsers(_users);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error creating user by admin: {ex.Message}");
                return false;
            }
        }


        public List<User> GetAllUser()
        {
            try
            {
                return _users.Select(x => new User
                {
                    Id = x.Id,
                    Username = x.Username,
                    Role = x.Role
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error getting users: {ex.Message}");
                return new List<User>();
            }
        }

        public bool EditUser(Guid id, string newUsername, string newPassword, Role newRole)
        {
            try
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null) return false;

                if (string.IsNullOrWhiteSpace(newUsername) || string.IsNullOrWhiteSpace(newPassword))
                    return false;

                user.Username = newUsername;
                user.PasswordEncrypted = CryptoHelper.Encrypt(newPassword);
                user.Role = newRole;
                _fileService.SaveUsers(_users);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error editing user: {ex.Message}");
                return false;
            }
        }

        public bool DeleteUser(Guid id)
        {
            try
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null) return false;

                _users.Remove(user);
                _fileService.SaveUsers(_users);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error deleting user: {ex.Message}");
                return false;
            }
        }

        public User? GetUserById(Guid id)
        {
            try
            {
                return _users.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error getting user by ID: {ex.Message}");
                return null;
            }
        }
    }
}
