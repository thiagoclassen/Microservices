using System.Net.Mime;
using System.Text;
using System.Text.Json;
using PlatformService.DTOs;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration) : ICommandDataClient
{
    private readonly string _url = $"{configuration["CommandServiceUrl"]}/api/c/platforms";

    public async Task SendPlatformToCommand(PlatformReadDto platformReadDto)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(platformReadDto),
            Encoding.UTF8,
           MediaTypeNames.Application.Json);

        var response = await httpClient.PostAsync(_url, httpContent);

        Console.WriteLine(response.IsSuccessStatusCode
            ? "--> sync Post to CommandService was OK!"
            : "--> sync Post to CommandService was BAD!");
    }
}