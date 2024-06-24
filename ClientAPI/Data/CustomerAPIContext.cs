using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Models;

namespace CustomerAPI.Data
{
    public class CustomerAPIContext : DbContext
    {
        public CustomerAPIContext (DbContextOptions<CustomerAPIContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Customer>().ToTable(null);
            modelBuilder.Entity<Models.Customer>().HasBaseType(null).ToTable("AccountCreated");
            modelBuilder.Entity<Models.Customer>().HasBaseType(null).ToTable("RemovedCustomer");
            modelBuilder.Entity<Models.Customer>().HasKey(c => c.Document);
        }

        public DbSet<Models.Customer> Customer { get; set; } = default!;
        public DbSet<Models.Customer> AccountCreated { get; set; } = default!;
        public DbSet<Models.Customer> RemovedCustomer { get; set; } = default!;

    }
}
