using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.Repositories;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class PlatformsController(
    IMapper mapper,
    IPlatformRepository repository,
    ICommandDataClient commandDataClient,
    IMessageBusClient messageBusClient)
    : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Console.WriteLine("--> Getting Platforms.....");
        return Ok(repository.GetAllPlatforms());
    }

    [HttpGet("{id:int}", Name = "GetPlatformById")]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatformById(int id)
    {
        Console.WriteLine("--> Get Platform.....");

        var platform = repository.GetPlatformById(id);
        if (platform is null)
            return NotFound();

        return Ok(platform);
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform([FromBody] PlatformCreateDto platform)
    {
        var platformModel = mapper.Map<Platform>(platform);
        repository.CreatePlatform(platformModel);
        repository.SaveChanges();

        var platformReadDto = mapper.Map<PlatformReadDto>(platformModel);

        // Send Sync Message
        try
        {
            await commandDataClient.SendPlatformToCommand(platformReadDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send {platformReadDto.Name} synchronously: ", e.Message);
        }
        
        // Send Async Message
        try
        {
            var platformPublishedDto = mapper.Map<PlatformPublishedDto>(platformReadDto);
            platformPublishedDto.Event = "Platform_Published";
            messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Failed to send the message Async: {ex.Message}");
        }

        return CreatedAtRoute(
            routeName: nameof(GetPlatformById),
            new { platformReadDto.Id },
            platformReadDto);
    }
}