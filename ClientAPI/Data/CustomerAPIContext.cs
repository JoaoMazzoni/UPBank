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
        public CustomerAPIContext(DbContextOptions<CustomerAPIContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Customer>().ToTable("Customer");
            modelBuilder.Entity<Models.CopyClasses.RemovedCustomer>().ToTable("RemovedCustomer");
            modelBuilder.Entity<Models.CopyClasses.AccountRequest>().ToTable("AccountRequest");
            modelBuilder.Entity<Models.Customer>().HasKey(c => c.Document);
        }

        public DbSet<Models.Customer> Customer { get; set; } = default!;
        public DbSet<Models.CopyClasses.RemovedCustomer> RemovedCustomer { get; set; } = default!;
        public DbSet<Models.CopyClasses.AccountRequest> AccountRequest { get; set; } = default!;
    }
}



