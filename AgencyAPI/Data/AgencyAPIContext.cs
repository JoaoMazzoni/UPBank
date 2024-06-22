using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;

namespace AgencyAPI.Data
{
    public class AgencyAPIContext : DbContext
    {
        public AgencyAPIContext (DbContextOptions<AgencyAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Agency> Agency { get; set; } = default!;
    }
}
