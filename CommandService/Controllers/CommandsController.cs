using AutoMapper;
using CommandsService.DTOs;
using CommandsService.Models;
using CommandsService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[ApiController]
[Route("api/c/platforms/{platformId:int}/[controller]")]
public class CommandsController(
    ICommandRepository repository,
    IMapper mapper)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetCommandsForPlatform(int platformId)
    {
        Console.WriteLine($"--> Getting Commands for Platform: {platformId}...");

        if (!await repository.PlatformExists(platformId))
            return NotFound();

        var commands = await repository.GetCommandsForPlatform(platformId);

        return Ok(commands);
    }

    [HttpGet("{commandId:int}", Name = "GetCommandForPlatform")]
    public async Task<ActionResult<CommandReadDto>> GetCommandForPlatform(int platformId, int commandId)
    {
        Console.WriteLine($"--> Getting Command: {commandId} for Platform: {platformId}...");

        if (!await repository.PlatformExists(platformId))
            return NotFound();

        var command = await repository.GetCommand(platformId, commandId);

        if (command is null)
            return NotFound();

        return Ok(command);
    }

    [HttpPost]
    public async Task<ActionResult<CommandReadDto>> CreateCommandForPlatform(int platformId,
        [FromBody] CommandCreateDto commandCreateDto)
    {
        Console.WriteLine($"--> Creating Command for Platform: {platformId}...");

        if (!await repository.PlatformExists(platformId))
            return NotFound();

        var commandModel = mapper.Map<Command>(commandCreateDto);

        await repository.CreateCommand(platformId, commandModel);
        await repository.SaveChanges();

        return CreatedAtRoute(
            nameof(GetCommandForPlatform),
            new { platformId = platformId, controller = "Commands", commandId = commandModel.Id },
            commandModel);
    }
}