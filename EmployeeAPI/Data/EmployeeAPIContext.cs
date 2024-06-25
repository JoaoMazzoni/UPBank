using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.CopyClasses;

namespace EmployeeAPI.Data
{
    public class EmployeeAPIContext : DbContext
    {
        public EmployeeAPIContext(DbContextOptions<EmployeeAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Employee> Employee { get; set; } = default!;
        public DbSet<DeletedEmployee> DeletedEmployee { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
            .Property(e => e.Register)
            .UseIdentityColumn(1,1);
            
        }
    }
}
