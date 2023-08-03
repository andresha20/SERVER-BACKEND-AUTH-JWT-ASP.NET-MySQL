using Microsoft.EntityFrameworkCore;

namespace APIToken.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }
        public DbSet<UserModel> UserRegistrations { get; set; }
    }
}
