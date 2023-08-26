using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskMgmt.Models
{
    // enum Role
    public enum Role
    {
        User = 1,
        Admin = 2
    }
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public Role Role { get; set; } = Role.User;

        public Tasks Task { get; set; }
    }
}