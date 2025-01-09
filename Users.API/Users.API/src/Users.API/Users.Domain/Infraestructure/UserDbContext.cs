using Microsoft.EntityFrameworkCore;
using User.Domain.EntityConfigurations;

namespace User.Domain.Infraestructure
{
    
    public class UserDbContext : DbContext
    {
        public DbSet<Entities.User> Users { get; set; }
        
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        }
    }
};

