using CommandsService.Models;

namespace CommandsService.Repositories;

public interface ICommandRepository
{
    Task<bool> SaveChanges();
    
    // Platforms
    Task<IEnumerable<Platform>> GetAllPlatforms();
    Task CreatePlatform(Platform platform);
    Task<bool> PlatformExists(int platformId);
    Task<bool> ExternalPlatformExists(int externalId);
    
    // Commands
    Task<IEnumerable<Command>> GetCommandsForPlatform(int platformId);
    Task<Command?> GetCommand(int platformId, int commandId);
    Task CreateCommand(int platformId, Command command);
}