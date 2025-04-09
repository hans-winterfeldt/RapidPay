using Microsoft.EntityFrameworkCore;
using RapidPay.Infrastructure.Persistence;

namespace RapidPay.Tests.Factories;

public static class InMemoryDbContextFactory
{
    public static RapidPayDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<RapidPayDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database for each test
            .Options;

        var dbContext = new RapidPayDbContext(options);

        // Seed initial data if needed
        SeedData(dbContext);

        return dbContext;
    }

    private static void SeedData(RapidPayDbContext dbContext)
    {
        // Add any initial data for testing purposes
        dbContext.Cards.Add(new Domain.Models.Card
        {
            CardNumber = "123456789012345",
            Balance = 1000m
        });

        dbContext.SaveChanges();
    }
}