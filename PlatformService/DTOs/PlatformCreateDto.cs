namespace PlatformService.DTOs;

public class PlatformCreateDto
{
    public required string Name { get; init; }
    
    public required string Publisher { get; init; }
    
    public required string Cost { get; init; }
}