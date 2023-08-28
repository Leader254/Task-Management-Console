using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskMgmt.Models;

namespace TaskMgmt.Context
{
    public class TaskContext : DbContext
    {

        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
 optionsBuilder.UseSqlServer("Server=localhost; Database=MgntDB; User Id=sa; Password=1234; Encrypt=False; TrustServerCertificate=True");        }

    }
}