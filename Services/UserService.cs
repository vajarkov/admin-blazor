using admin_blazor.Data;
using admin_blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace admin_blazor.Services;

public class UserService(AppDbContext db)
{
    public Task<List<User>> GetUsersAsync()
    {
        return db.Users
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .Take(100)
            .ToListAsync();
    }
}
