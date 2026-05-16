using MailDesk.API.DTOs.Tracking;
using MailDesk.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MailDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TrackingController : ControllerBase
{
    private readonly ITrackingService _trackingService;
    private readonly ILogger<TrackingController> _logger;

    public TrackingController(ITrackingService trackingService, ILogger<TrackingController> logger)
    {
        _trackingService = trackingService;
        _logger = logger;
    }

    private int GetUserIdFromHeader()
    {
        if (Request.Headers.TryGetValue("X-User-Id", out var userIdValue) &&
            int.TryParse(userIdValue, out var userId))
            return userId;
        return 0;
    }

    // ─────────────────────────────────────────────────────────
    // GET /api/tracking/{id}
    // Ambil timeline riwayat lengkap sebuah surat
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Get timeline tracking surat berdasarkan ID surat.
    /// Menampilkan seluruh riwayat: dibuat, masuk inbox, dan disposisi.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTrackingSurat([FromRoute] int id)
    {
        try
        {
            var result = await _trackingService.GetTrackingSuratAsync(id);

            _logger.LogInformation("Get tracking surat ID: {Id}", id);

            return Ok(new { success = true, data = result });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get tracking surat ID: {Id}", id);
            return StatusCode(500, new { success = false, message = "Terjadi kesalahan pada server." });
        }
    }

    // ─────────────────────────────────────────────────────────
    // PATCH /api/tracking/{id}/status
    // Update status surat
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Update status surat. Hanya Admin, TU, dan Sekretaris yang bisa mengubah.
    /// Status yang diizinkan: "Baru", "Diproses", "Selesai".
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSuratStatus(
        [FromRoute] int id,
        [FromBody] UpdateSuratStatusRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });

        try
        {
            var userId = GetUserIdFromHeader();

            // Hanya admin roles (1=Admin, 2=TU, 3=Sekretaris) yang boleh ubah status surat
            if (userId > 3)
            {
                _logger.LogWarning(
                    "User {UserId} tidak memiliki akses untuk mengubah status surat {SuratId}", userId, id);
                return StatusCode(403, new
                {
                    success = false,
                    message = "Hanya Admin, TU, atau Sekretaris yang dapat mengubah status surat."
                });
            }

            var result = await _trackingService.UpdateSuratStatusAsync(id, request);

            _logger.LogInformation(
                "Status surat diperbarui - UserId: {UserId}, SuratId: {Id}, Status: {Status}",
                userId, id, request.Status);

            return Ok(new
            {
                success = true,
                message = $"Status surat berhasil diperbarui menjadi \"{request.Status}\".",
                data = result
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat update status surat ID: {Id}", id);
            return StatusCode(500, new { success = false, message = "Terjadi kesalahan pada server." });
        }
    }
}
