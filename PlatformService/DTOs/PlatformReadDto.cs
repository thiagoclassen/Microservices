namespace PlatformService.DTOs;

public class PlatformReadDto
{
    public required int Id { get; init; }
    
    public required string Name { get; init; }
    
    public required string Publisher { get; init; }
    
    public required string Cost { get; init; }
}