using MailDesk.API.DTOs.User;
using MailDesk.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MailDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger      = logger;
    }

    // ─────────────────────────────────────────────────────────
    // GET /api/user/pimpinan
    // Daftar Pimpinan untuk dropdown "Ditujukan Kepada" di form Catat Surat Masuk
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Get daftar user dengan role Pimpinan untuk dropdown pemilihan tujuan disposisi.
    /// Dipanggil saat TU/Sekretaris membuka form Catat Surat Masuk.
    /// </summary>
    /// <remarks>
    /// Response diurutkan berdasarkan nama (A-Z).
    /// Gunakan field <c>id</c> sebagai nilai <c>ditujukanKeId</c> saat POST /api/surat.
    /// </remarks>
    /// <response code="200">Daftar Pimpinan berhasil diambil.</response>
    [HttpGet("pimpinan")]
    [ProducesResponseType(typeof(IEnumerable<UserDropdownResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPimpinan()
    {
        try
        {
            var result = await _userService.GetUserDropdownAsync("Pimpinan");
            return Ok(new
            {
                success   = true,
                totalData = result.Count(),
                data      = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get daftar Pimpinan.");
            return StatusCode(500, new
            {
                success = false,
                message = "Terjadi kesalahan pada server."
            });
        }
    }

    // ─────────────────────────────────────────────────────────
    // GET /api/user?role=Pimpinan
    // Daftar semua user dengan optional filter role
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Get daftar semua user, dengan optional filter berdasarkan nama role.
    /// </summary>
    /// <param name="role">
    /// Nama role untuk filter (contoh: "Pimpinan", "Sekretaris", "Admin").
    /// Kosongkan untuk mengambil semua user.
    /// </param>
    /// <response code="200">Daftar user berhasil diambil.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDropdownResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromQuery] string? role)
    {
        try
        {
            var result = await _userService.GetUserDropdownAsync(role);
            return Ok(new
            {
                success   = true,
                totalData = result.Count(),
                data      = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get daftar user. Role filter: {Role}", role);
            return StatusCode(500, new
            {
                success = false,
                message = "Terjadi kesalahan pada server."
            });
        }
    }
}
