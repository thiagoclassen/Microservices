namespace PlatformService.DTOs;

public class PlatformPublishedDto
{
    public PlatformPublishedDto(int Id,
        string Name,
        string? Event)
    {
        this.Id = Id;
        this.Name = Name;
        this.Event = Event;
    }

    public PlatformPublishedDto()
    {
        
    }

    public int Id { get; init; }
    public string Name { get; init; }
    public string? Event { get; set; }

    public void Deconstruct(out int Id, out string Name, out string? Event)
    {
        Id = this.Id;
        Name = this.Name;
        Event = this.Event;
    }
}