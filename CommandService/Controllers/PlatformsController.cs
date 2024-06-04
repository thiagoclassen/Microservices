using AutoMapper;
using CommandsService.DTOs;
using CommandsService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[ApiController]
[Route("api/c/[Controller]")]
public class PlatformsController(
    ICommandRepository repository) 
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatforms()
    {
        Console.WriteLine("--> Getting Platforms from CommandService...");
        
        var platforms = await repository.GetAllPlatforms();

        return Ok(platforms);
    }
    
    [HttpPost]
    public ActionResult TestInboundConnection()
    {
        Console.WriteLine("--> Inbound Post # CommandService");

        return Ok("--> Inbound test from CommandService/PlatformsController.");
    }
}
