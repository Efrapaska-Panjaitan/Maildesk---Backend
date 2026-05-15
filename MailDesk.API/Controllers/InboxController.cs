using MailDesk.API.DTOs.Inbox;
using MailDesk.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MailDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InboxController : ControllerBase
{
    private readonly IInboxService _inboxService;
    private readonly ILogger<InboxController> _logger;

    public InboxController(
        IInboxService inboxService,
        ILogger<InboxController> logger)
    {
        _inboxService = inboxService;
        _logger = logger;
    }

    // Helper: ambil user ID dari header
    private int GetUserIdFromHeader()
    {
        if (Request.Headers.TryGetValue("X-User-Id", out var userIdValue) &&
            int.TryParse(userIdValue, out var userId))
        {
            return userId;
        }
        return 0;
    }

    // ─────────────────────────────────────────────────────────
    // GET /api/inbox
    // Get inbox sesuai role: Admin lihat semua,
    // selain Admin hanya lihat inbox miliknya (PenerimaId = userId)
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Get inbox dengan filter role-based:
    /// - Admin (role 1): lihat semua inbox
    /// - Selain Admin: lihat hanya inbox untuk mereka (PenerimaId = User ID)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllInbox([FromQuery] InboxQueryParams query)
    {
        try
        {
            var userId = GetUserIdFromHeader();

            // Hanya Admin (role 1) yang bisa lihat semua inbox
            var isAdminRole = userId == 1;

            // Jika bukan admin, filter inbox hanya untuk user itu (PenerimaId = userId)
            if (!isAdminRole)
                query.FilterByPenerimaId = userId;

            var result = await _inboxService.GetAllInboxAsync(query);

            _logger.LogInformation("Get inbox - UserId: {UserId}, IsAdmin: {IsAdmin}, Total: {Total}",
                userId, isAdminRole, result.Meta.TotalData);

            return Ok(new { success = true, data = result.Data, meta = result.Meta });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get all inbox.");
            return StatusCode(500, new { success = false, message = "Terjadi kesalahan pada server." });
        }
    }

    // ─────────────────────────────────────────────────────────
    // GET /api/inbox/{id}
    // Get detail inbox — cek user adalah penerima atau admin
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Get detail inbox. User biasa hanya bisa lihat jika mereka adalah penerima.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(InboxDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetInboxById([FromRoute] int id)
    {
        try
        {
            var userId = GetUserIdFromHeader();
            var isAdminRole = userId == 1;

            var result = await _inboxService.GetInboxByIdAsync(id);

            // Jika bukan admin dan PenerimaId tidak sesuai, forbidden
            if (!isAdminRole && result.PenerimaId != userId)
            {
                _logger.LogWarning("User {UserId} mencoba akses inbox {InboxId} yang bukan miliknya",
                    userId, id);
                return StatusCode(403, new { success = false, message = "Anda tidak memiliki akses ke inbox ini." });
            }

            return Ok(new { success = true, data = result });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get inbox ID: {Id}", id);
            return StatusCode(500, new { success = false, message = "Terjadi kesalahan pada server." });
        }
    }

    // ─────────────────────────────────────────────────────────
    // POST /api/inbox
    // Buat inbox dari surat (hanya admin roles: 1, 2, 3)
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Buat inbox baru dari surat.
    /// Hanya Admin, TU, dan Sekretaris yang bisa membuat.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InboxDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateInbox([FromBody] CreateInboxFromSuratRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });

        try
        {
            var result = await _inboxService.CreateInboxFromSuratAsync(request);

            var userId = GetUserIdFromHeader();
            _logger.LogInformation(
                "Inbox berhasil dibuat - UserId: {UserId}, SuratId: {SuratId}, InboxId: {InboxId}",
                userId, request.SuratId, result.Id);

            return CreatedAtAction(
                actionName: nameof(GetInboxById),
                routeValues: new { id = result.Id },
                value: new
                {
                    success = true,
                    message = "Inbox berhasil dibuat dari surat.",
                    data = result
                });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Validasi gagal: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat membuat inbox dari surat.");
            return StatusCode(500, new { success = false, message = "Terjadi kesalahan pada server." });
        }
    }
}

