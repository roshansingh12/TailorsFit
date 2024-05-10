using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Tailors_fitv0._2.Models
{
    public class ApplicationDbContext : IdentityDbContext<AllApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<tailorModel> tailors { get; set; }
        public DbSet<UserModel> users { get; set; }
        public DbSet<Tailors_Feedbacks> comments { get; set; }
    }
}
