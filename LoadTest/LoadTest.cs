using System.Net.Mime;
using System.Text;
using System.Text.Json;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace ConsoleApp1;

public static class LoadTest
{
    public static void Execute()
    {
        var httpClient = new HttpClient();

        PlatformCreateDto platformCreateDto = new() { Cost = "Free", Name = "Dockerszzz", Publisher = "Yo MaMa" };
        var body = new StringContent(
            JsonSerializer.Serialize(platformCreateDto),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);
        var scenario = Scenario.Create("http_scenario", async context =>
            {
                var request =
                    Http.CreateRequest("POST", "http://acme.com/api/Platforms")
                        .WithHeader("Accept", "application/json")
                        // .WithHeader("Content-Type", "application/json")
                        .WithBody(body);

                var response = await Http.Send(httpClient, request);

                return response;
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: 200,
                    interval: TimeSpan.FromSeconds(1),
                    during: TimeSpan.FromSeconds(100))
            );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }

    private class PlatformCreateDto
    {
        public required string Name { get; init; }

        public required string Publisher { get; init; }

        public required string Cost { get; init; }
    }
}