using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(u => new { u.System, u.ExternalId });

        // Other configurations
    }

    // Define DbSets for your entities
    public DbSet<Chat> Chats { get; set; }
    public DbSet<User> Users { get; set; }
}