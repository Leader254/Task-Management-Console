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
        public bool IsAdmin { get; set; } = false;

        public Tasks Task { get; set; }
    }
}