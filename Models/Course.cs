
namespace admin_blazor.Models;

public class Course
{
    public uint Id { get; set; }

    public uint? VId { get; set; }

    public string? Name { get; set; }

    public uint? Price { get; set; }

    public uint? Duration { get; set; }

    public string? Level { get; set; }

    public string? Description { get; set; }
}
