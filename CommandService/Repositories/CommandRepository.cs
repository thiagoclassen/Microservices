using CommandsService.Data;
using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Repositories;

public class CommandRepository(AppDbContext dbContext) : ICommandRepository
{
    public async Task<bool> SaveChanges()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<Platform>> GetAllPlatforms()
    {
        return await dbContext.Platforms.Include(p => p.Commands).ToListAsync();
    }

    public async Task CreatePlatform(Platform platform)
    {
        ArgumentNullException.ThrowIfNull(nameof(platform));

        await dbContext.Platforms.AddAsync(platform);
    }

    public async Task<bool> PlatformExists(int platformId)
    {
        return await dbContext.Platforms.AnyAsync(p => p.Id == platformId);
    }

    public async Task<bool> ExternalPlatformExists(int externalId)
    {
        return await dbContext.Platforms.AnyAsync(p => p.ExternalId == externalId);
    }

    public async Task<IEnumerable<Command>> GetCommandsForPlatform(int platformId)
    {
        return await dbContext
            .Commands
            .Where(c => c.PlatformId == platformId)
            .OrderBy(c => c.Platform.Name)
            .Include(c => c.Platform)
            .ToListAsync();

    }

    public async Task<Command?> GetCommand(int platformId, int commandId)
    {
        return await dbContext
            .Commands
            .Where(c => c.PlatformId == platformId && c.Id == commandId)
            .Include(c => c.Platform)
            .FirstOrDefaultAsync();
    }

    public async Task CreateCommand(int platformId, Command command)
    {
        ArgumentNullException.ThrowIfNull(nameof(command));

        command.PlatformId = platformId;

        await dbContext
            .Commands
            .AddAsync(command);
    }
}