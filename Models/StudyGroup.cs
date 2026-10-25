namespace admin_blazor.Models;

public class StudyGroup
{
    public uint Id { get; set; }
    public uint? Vendor { get; set; }
    public string? Name { get; set; }
    public uint? Teacher { get; set; }
    public string? Time { get; set; }
    public uint? Count { get; set; }
    public uint? Price { get; set; }
    public uint? CreateTime { get; set; }
    public string? Weekdays { get; set; }
}