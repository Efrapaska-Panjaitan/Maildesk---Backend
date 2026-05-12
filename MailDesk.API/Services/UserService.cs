using MailDesk.API.Data;
using MailDesk.API.DTOs.User;
using MailDesk.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailDesk.API.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger  = logger;
    }

    public async Task<IEnumerable<UserDropdownResponse>> GetUserDropdownAsync(string? namaRole)
    {
        var query = _context.Users
            .Include(u => u.Role)
            .AsQueryable();

        // Filter berdasarkan role jika diberikan
        if (!string.IsNullOrWhiteSpace(namaRole))
            query = query.Where(u => u.Role != null && u.Role.NamaRole == namaRole);

        var result = await query
            .OrderBy(u => u.Nama)
            .Select(u => new UserDropdownResponse
            {
                Id       = u.Id,
                Nama     = u.Nama,
                NamaRole = u.Role != null ? u.Role.NamaRole : null
            })
            .ToListAsync();

        _logger.LogInformation(
            "User dropdown fetched. Role filter: {Role}, Count: {Count}",
            namaRole ?? "semua", result.Count);

        return result;
    }
}
