namespace admin_blazor.Models;

public class Schedule
{
    public uint Id { get; set; }
    public uint? TeacherId { get; set; }
    public string? Date { get; set; }
    // В legacy-БД поле time имеет тип VARCHAR и хранит Unix timestamp строкой.
    public string? Time { get; set; }
    public uint? Vendor { get; set; }
    public uint? AddTime { get; set; }
    public double? Reserv { get; set; }
    public uint? Teacher { get; set; }
    public uint? CreateTime { get; set; }
}