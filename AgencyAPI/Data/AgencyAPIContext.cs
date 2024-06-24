using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using Models;

namespace AgencyAPI.Data
{
    public class AgencyAPIContext : DbContext
    {
        public AgencyAPIContext(DbContextOptions<AgencyAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Agency> Agency { get; set; }
        public DbSet<RemovedAgency> AgencyHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Agency>().ToTable("Agency");
            modelBuilder.Entity<RemovedAgency>().ToTable("AgencyHistory");
        }

    }
}
