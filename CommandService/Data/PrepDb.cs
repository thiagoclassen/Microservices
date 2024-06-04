using CommandsService.Models;
using CommandsService.Repositories;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder applicationBuilder)
    {
        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
        var grpcClient = serviceScope.ServiceProvider.GetRequiredService<IPlatformDataClient>();
        
        var platforms = grpcClient.ReturnAllPlatforms();

        var commandRepository = serviceScope.ServiceProvider.GetRequiredService<ICommandRepository>();
        SeedData(commandRepository, platforms);
    }

    private static async void SeedData(ICommandRepository commandRepository, IEnumerable<Platform> platforms)
    {
        foreach (var platform in platforms)
        {
            if (await commandRepository.ExternalPlatformExists(platform.ExternalId)) continue;
            await commandRepository.CreatePlatform(platform);
            await commandRepository.SaveChanges();
        }
    }
}