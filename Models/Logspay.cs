
namespace admin_blazor.Models;

public class Logspay
{
    public uint Id { get; set; }
    public uint? Time { get; set; }
    public uint? UserId { get; set; }
    public uint? Cancellation { get; set; }
    public double? Lvl { get; set; }
    public byte? Kzt { get; set; }
}
