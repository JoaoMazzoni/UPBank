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
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<RemovedCustomer>().ToTable("RemovedCustomer");
            modelBuilder.Entity<Customer>().HasKey(c => c.Document);
            modelBuilder.Entity<RemovedCustomer>().HasKey(c => c.Document); 
        }

        public DbSet<Models.Customer> Customer { get; set; } = default!;
        public DbSet<Models.RemovedCustomer> RemovedCustomer { get; set; } = default!;
        public DbSet<Models.Customer> AccountRequest { get; set; } = default!;


    }
}
