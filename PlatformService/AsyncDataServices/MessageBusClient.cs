using System.Text;
using System.Text.Json;
using PlatformService.DTOs;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient :IMessageBusClient
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration configuration)
    {
        // _connection = connection;
        // _channel = channel;
        var factory = new ConnectionFactory()
        {
            HostName = configuration["RabbitMQ:Host"],
            Port = Convert.ToInt16(configuration["RabbitMQ:Port"])
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            
            Console.WriteLine("-->Connected to message bus...");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"--> Could not connect to the message Bus: {ex.Message}");
        }
    }

    private static void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> Shutting down RabbitMQ...");
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(
            exchange: "trigger",
            routingKey: "",
            basicProperties: null,
            body);
        Console.WriteLine($"--> Message Sent: {message}");
    }
    
    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        var message = JsonSerializer.Serialize(platformPublishedDto);

        if (_connection.IsOpen)
            SendMessage(message);    
    }

    public void Dispose()
    {
        Console.WriteLine("--> MessageBus Disposed...");
        if (!_channel.IsOpen) return;
        _channel.Close();
        _connection.Close();
    }
}