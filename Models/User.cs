
namespace admin_blazor.Models;

public class User
{
    public uint Id { get; set; }

    public double? Phone { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Password { get; set; }

    public string? Balance { get; set; }

    public uint? StudentType { get; set; }

    public uint? CreatedAt { get; set; }

    public uint? UpdatedAt { get; set; }

    public string? Avatar { get; set; }

    public uint? UserType { get; set; }

    public byte? Ban { get; set; }

    public byte? Verify { get; set; }

    public string? Sex { get; set; }

    public DateTime? DateOfBirth { get; set; }
}
