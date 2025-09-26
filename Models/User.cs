using System;

namespace Models
{

    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string PasswordEncrypted { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.User;
    }
}
