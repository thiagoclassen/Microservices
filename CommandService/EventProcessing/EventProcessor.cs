using System.Text.Json;
using AutoMapper;
using CommandsService.DTOs;
using CommandsService.Models;
using CommandsService.Repositories;

namespace CommandsService.EventProcessing;

public class EventProcessor(
    IServiceScopeFactory scopeFactory, 
    IMapper mapper) 
    : IEventProcessor
{
    public void ProcessEvent(string message)
    {
        if(DetermineEvent(message) == EventType.PlatformPublished)
            AddPlatform(message);
        else
            Console.WriteLine("--> Undetermined event type...");
    }

    private async void AddPlatform(string platformPublishedMessage)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
        var platform = mapper.Map<Platform>(platformPublishedDto);

        if (!await repository.ExternalPlatformExists(platform.ExternalId))
        {
            await repository.CreatePlatform(platform);
            await repository.SaveChanges();
        }
        else
        {
            Console.WriteLine($"Platform with ExternalId {platform.ExternalId} already exists...");
        }
    }

    private static EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        return eventType!.Event switch
        {
            "Platform_Published" => EventType.PlatformPublished,
            _ => EventType.Undetermined
        };
    }

    private enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}