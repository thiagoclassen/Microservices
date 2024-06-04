using System.Collections;
using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;

namespace CommandsService.SyncDataServices.Grpc;

public class PlatformDataClient(IConfiguration configuration, IMapper mapper) : IPlatformDataClient
{

    public IEnumerable<Platform> ReturnAllPlatforms()
    {
        Console.WriteLine($"--> Calling GRPC Service {configuration["GrpcPlatform:Host"]}");
        var grpcUrl = $"{configuration["GrpcPlatform:Host"]}:{configuration["GrpcPlatform:Port"]}";
        var channel = GrpcChannel.ForAddress(grpcUrl);
        var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var response = new GetAllRequest();

        try
        {
            var reply = client.GetAll(response);
            return mapper.Map<IEnumerable<Platform>>(reply.Platforms);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not call GRPC Server {e.Message}");
        }
        return [];
    }
}