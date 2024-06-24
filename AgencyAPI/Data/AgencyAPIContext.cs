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

        public DbSet<Agency> Agency { get; set; } = default!;
    }
}
