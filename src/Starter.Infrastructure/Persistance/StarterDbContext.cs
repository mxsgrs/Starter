using Microsoft.EntityFrameworkCore;
using Starter.Domain.Aggregates.UserAggregate;
using System.Reflection;

namespace Starter.Infrastructure.Persistance;

public partial class StarterDbContext : DbContext
{
    public StarterDbContext(DbContextOptions<StarterDbContext> options)
        : base(options)
    {
        string? aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (aspNetCoreEnvironment == "Development" || aspNetCoreEnvironment == "Integration")
        {
            Database.Migrate();
        }
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
