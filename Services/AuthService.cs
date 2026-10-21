
using admin_blazor.Data;
using admin_blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace admin_blazor.Services;

public class AuthService(AppDbContext db)
{
    public async Task<User?> ValidateAdminAsync(
        string phone,
        string password)
    {
        var normalizedPhone = new string(
            phone.Where(char.IsDigit).ToArray());

        if (!long.TryParse(normalizedPhone, out var phoneNumber))
            return null;

        var user = await db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u =>
                u.Phone == (double)phoneNumber &&
                u.UserType == 0);

        if (user?.Password is null)
            return null;

        return BCrypt.Net.BCrypt.Verify(password, user.Password)
            ? user
            : null;
    }
}
