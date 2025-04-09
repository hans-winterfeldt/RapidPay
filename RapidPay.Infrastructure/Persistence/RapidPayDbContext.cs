using Microsoft.EntityFrameworkCore;
using RapidPay.Domain.Models;

namespace RapidPay.Infrastructure.Persistence;

public class RapidPayDbContext : DbContext
{
    public DbSet<Card> Cards { get; set; }

    public RapidPayDbContext(DbContextOptions<RapidPayDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from the Configurations folder
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RapidPayDbContext).Assembly);
    }
}