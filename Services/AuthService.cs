
using admin_blazor.Data;
using Microsoft.EntityFrameworkCore;

namespace admin_blazor.Services;

public class AuthService(AppDbContext db)
{
    public async Task<bool> ValidateAdminAsync(
        string login,
        string password)
    {
        var user = await db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Phone.ToString() == login);

        if (user is null ||
            user.UserType != 0 ||
            string.IsNullOrEmpty(user.Password))
        {
            return false;
        }

        return BCrypt.Net.BCrypt.Verify(password, user.Password);
    }
}
