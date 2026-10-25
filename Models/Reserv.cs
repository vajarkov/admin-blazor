namespace admin_blazor.Models;

public class Reserv
{
    public uint Id { get; set; }
    public double? Vendor { get; set; }
    public uint? Student { get; set; }
    public uint? Teacher { get; set; }
    public string? Course { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public uint? Status { get; set; }
    public uint? For { get; set; }
    public uint? ReservTime { get; set; }
    public uint? Timecode { get; set; }
    public uint? Cancel { get; set; }
    public string? Reason { get; set; }
    public string? Coment { get; set; }
    public string? CanseledBy { get; set; }
}