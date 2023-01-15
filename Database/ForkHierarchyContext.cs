using Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Database;

public class ForkHierarchyContext : DbContext
{
    public DbSet<GitHubRepository> GitHubRepositories { get; set; }
    public DbSet<GitHubUser> GitHubUsers { get; set; }
    public DbSet<QueuedRepository> QueuedRepositories { get; set; }

    public ForkHierarchyContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ForkHierarchyContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
