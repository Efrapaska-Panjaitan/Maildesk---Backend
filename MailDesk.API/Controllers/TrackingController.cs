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
}
