
namespace admin_blazor.Models;

public class Subscription
{
    public uint Id { get; set; }

    public string? Title { get; set; }

    public uint? Price { get; set; }

    public byte? Descr { get; set; }

    public string? Pages { get; set; }
}
