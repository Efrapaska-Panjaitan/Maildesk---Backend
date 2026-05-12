using MailDesk.API.DTOs.Disposisi;
using MailDesk.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MailDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DisposisiController : ControllerBase
{
    private readonly IDisposisiService _disposisiService;
    private readonly ILogger<DisposisiController> _logger;

    public DisposisiController(
        IDisposisiService disposisiService,
        ILogger<DisposisiController> logger)
    {
        _disposisiService = disposisiService;
        _logger           = logger;
    }

    // ─────────────────────────────────────────────────────────
    // POST /api/disposisi
    // Buat disposisi (Pimpinan/Admin)
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Buat lembar disposisi ke bawahan.
    /// Hanya Pimpinan dan Admin yang dapat melakukan disposisi.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DisposisiDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateDisposisi(
        [FromBody] CreateDisposisiRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new
            {
                success = false,
                errors  = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
            });

        try
        {
            var result = await _disposisiService.CreateDisposisiAsync(request);
            return CreatedAtAction(
                nameof(GetDisposisiById),
                new { id = result.Id },
                new
                {
                    success = true,
                    message = "Disposisi berhasil dibuat.",
                    data    = result
                });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat membuat disposisi.");
            return StatusCode(500, new
            {
                success = false,
                message = "Terjadi kesalahan pada server."
            });
        }
    }

    // ─────────────────────────────────────────────────────────
    // GET /api/disposisi?userId=1
    // Daftar disposisi — Page Disposisi
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Get daftar disposisi untuk page Disposisi.
    /// Filter by userId untuk melihat disposisi yang diberikan/diterima user tertentu.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DisposisiListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDisposisiList([FromQuery] int? userId)
    {
        try
        {
            var result = await _disposisiService.GetDisposisiListAsync(userId);
            return Ok(new
            {
                success    = true,
                totalData  = result.Count(),
                data       = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get list disposisi.");
            return StatusCode(500, new
            {
                success = false,
                message = "Terjadi kesalahan pada server."
            });
        }
    }

    // ─────────────────────────────────────────────────────────
    // GET /api/disposisi/{id}
    // Detail satu disposisi
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Get detail disposisi berdasarkan ID.
    /// Ditampilkan saat user mengklik salah satu item di page Disposisi.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DisposisiDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDisposisiById([FromRoute] int id)
    {
        try
        {
            var result = await _disposisiService.GetDisposisiByIdAsync(id);
            return Ok(new { success = true, data = result });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get disposisi ID: {Id}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "Terjadi kesalahan pada server."
            });
        }
    }

    // ─────────────────────────────────────────────────────────
    // GET /api/disposisi/surat/{suratId}/tracking
    // Tracking posisi surat + riwayat disposisi
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Get tracking posisi surat dan riwayat disposisi berdasarkan ID surat.
    /// Menampilkan timeline: siapa mendisposisi ke siapa beserta status terkini.
    /// </summary>
    [HttpGet("surat/{suratId}/tracking")]
    [ProducesResponseType(typeof(DisposisiTrackingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTrackingBySuratId([FromRoute] int suratId)
    {
        try
        {
            var result = await _disposisiService.GetTrackingBySuratIdAsync(suratId);
            return Ok(new { success = true, data = result });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get tracking surat ID: {Id}", suratId);
            return StatusCode(500, new
            {
                success = false,
                message = "Terjadi kesalahan pada server."
            });
        }
    }
}