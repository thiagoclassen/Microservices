using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Models;

namespace PlatformService.Repositories;

public class PlatformRepository : IPlatformRepository
{

    private readonly AppDbContext _context;

    public PlatformRepository(AppDbContext context)
    {
        _context = context;
    }

    public bool SaveChanges()
    {
        var result =  _context.SaveChanges();
        return result > 0;
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.ToImmutableList();
    }

    public Platform? GetPlatformById(int id)
    {
        return _context.Platforms.Find(id);
    }

    public void CreatePlatform(Platform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);
        _context.Add(platform);
    }
}