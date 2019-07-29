using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace App1.Models
{
    public class BureauContext : DbContext
    {
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().HasMany(c => c.Workers)
                .WithMany(s => s.Projects)
                .Map(t => t.MapLeftKey("Project_Id")
                .MapRightKey("Worker_Id")
                .ToTable("ProjectWorker"));
        }
    }
}