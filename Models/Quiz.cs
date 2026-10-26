namespace admin_blazor.Models;

public class Quiz
{
    public uint Id { get; set; }
    public uint? Vendor { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}