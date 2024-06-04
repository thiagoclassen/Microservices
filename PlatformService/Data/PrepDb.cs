using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>()!, isProduction);
    }

    private static void SeedData(AppDbContext context, bool isProduction)
    {
        if (isProduction)
            context.Database.Migrate();
        
        if (!context.Platforms.Any())
        {
            Console.WriteLine("--> Seeding data...");

            Platform[] platforms =
            [
                new(name: "Dot Net", publisher: "Microsoft", cost: "Free"),
                new(name: "SQL Server", publisher: "Microsoft", cost: "Free"),
                new(name: "Kubernetes", publisher: "Cloud Native Computing Foundation", cost: "Free")
            ];
            context.Platforms.AddRange(platforms);
            context.SaveChanges();
        }else
            Console.WriteLine("--> We already have data.");
    }
}