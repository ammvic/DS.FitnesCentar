using Microsoft.EntityFrameworkCore;
using FitnessCentar.Members.Models;

namespace FitnessCentar.Members.Persistence
{
    public class MembersDbContext : DbContext
    {
        public MembersDbContext(DbContextOptions<MembersDbContext> options) : base(options) { }

        public DbSet<Member> Members { get; set; }
    }
}
