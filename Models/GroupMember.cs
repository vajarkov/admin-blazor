namespace admin_blazor.Models;

public class GroupMember
{
    public uint Id { get; set; }
    public uint? GroupId { get; set; }
    public uint? Student { get; set; }
    public uint? DateTime { get; set; }
}