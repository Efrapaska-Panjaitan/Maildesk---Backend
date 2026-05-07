using MailDesk.API.DTOs.Surat;
using MailDesk.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MailDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SuratController : ControllerBase
{
    private readonly ISuratService _suratService;
    private readonly ILogger<SuratController> _logger;

    public SuratController(
        ISuratService suratService,
        ILogger<SuratController> logger)
    {
        _suratService = suratService;
        _logger = logger;
    }

    /// <summary>
    /// Catat surat masuk baru beserta metadata.
    /// </summary>
    /// <remarks>
    /// Nomor agenda akan di-generate otomatis oleh sistem.
    /// File lampiran dihandle terpisah di endpoint upload-pdf.
    /// </remarks>
    /// <response code="201">Surat masuk berhasil dicatat.</response>
    /// <response code="400">Request tidak valid / nomor surat duplikat.</response>
    /// <response code="500">Server error.</response>
    [HttpPost]
    [ProducesResponseType(typeof(SuratResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSurat(
        [FromBody] CreateSuratRequest request)
    {
        // Validasi model (dari Data Annotations)
        if (!ModelState.IsValid)
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
            });

        try
        {
            var result = await _suratService.CreateSuratMasukAsync(request);

            _logger.LogInformation(
                "Surat masuk berhasil dicatat. NomorAgenda: {NomorAgenda}",
                result.NomorAgenda);

            return CreatedAtAction(
                actionName: nameof(GetSuratById), // akan dibuat di task tracking
                routeValues: new { id = result.Id },
                value: new
                {
                    success = true,
                    message = "Surat masuk berhasil dicatat.",
                    data = result
                });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Validasi gagal: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat mencatat surat masuk.");
            return StatusCode(500, new
            {
                success = false,
                message = "Terjadi kesalahan pada server. Silakan coba lagi."
            });
        }
    }

     // ─────────────────────────────────────────────────────────
    // GET /api/surat/{id}
    // Get detail surat berdasarkan ID
    // (Akan dikembangkan di Task 4 - Tracking Disposisi)
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Get detail surat berdasarkan ID.
    /// </summary>
    /// <param name="id">ID surat</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SuratResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSuratById([FromRoute] int id)
    {
        try
        {
            var result = await _suratService.GetSuratByIdAsync(id);
            return Ok(new { success = true, data = result });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get surat ID: {Id}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "Terjadi kesalahan pada server."
            });
        }
    }

    /// <summary>
    /// Upload file PDF lampiran surat masuk.
    /// </summary>
    /// <param name="id">ID surat masuk yang sudah dicatat</param>
    /// <param name="file">File PDF (maksimal 10MB)</param>
    /// <response code="200">File berhasil diupload.</response>
    /// <response code="400">Validasi gagal (bukan PDF / terlalu besar).</response>
    /// <response code="404">Surat masuk tidak ditemukan.</response>
    [HttpPost("{id}/upload-pdf")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(UploadPdfResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadPdf(
        [FromRoute] int id,
        [FromForm(Name = "file")] IFormFile file)
    {
        try
        {
            var result = await _suratService.UploadPdfAsync(id, file);

            return Ok(new
            {
                success = true,
                message = "File PDF berhasil diupload.",
                data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat upload PDF suratID: {Id}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "Terjadi kesalahan pada server."
            });
        }
    }

    /// <summary>
    /// Get preview nomor agenda berikutnya untuk ditampilkan di form input.
    /// Dipanggil saat halaman Input Surat Masuk pertama kali dibuka.
    /// </summary>
    /// <remarks>
    /// Nomor ini bersifat PREVIEW — nomor final dikonfirmasi saat simpan.
    /// Jika ada 2 user buka form bersamaan, nomor final bisa berbeda.
    /// </remarks>
    /// <response code="200">Preview nomor agenda berhasil dibuat.</response>
    [HttpGet("nomor-agenda/preview")]
    [ProducesResponseType(typeof(NomorAgendaPreviewResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNomorAgendaPreview()
    {
        try
        {
            var result = await _suratService.GetNomorAgendaPreviewAsync();

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat generate preview nomor agenda.");
            return StatusCode(500, new
            {
                success = false,
                message = "Terjadi kesalahan pada server."
            });
        }
    }

    // GET /api/surat
    // Dashboard — semua surat (masuk + keluar)
    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// Get semua surat untuk halaman Dashboard/Inbox.
    /// Menampilkan surat masuk dan surat keluar sekaligus.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<SuratListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSurat([FromQuery] SuratQueryParams query)
    {
        try
        {
            var result = await _suratService.GetAllSuratAsync(query);
            return Ok(new
            {
                success = true,
                data    = result.Data,
                meta    = result.Meta
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get all surat.");
            return StatusCode(500, new
            {
                success = false,
                message = "Terjadi kesalahan pada server."
            });
        }
    }

    // GET /api/surat/masuk
    // Page Surat Masuk
    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// Get daftar surat masuk untuk halaman Surat Masuk.
    /// </summary>
    [HttpGet("masuk")]
    [ProducesResponseType(typeof(PaginatedResponse<SuratListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuratMasuk([FromQuery] SuratQueryParams query)
    {
        try
        {
            var result = await _suratService.GetSuratMasukAsync(query);
            return Ok(new
            {
                success = true,
                data    = result.Data,
                meta    = result.Meta
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get surat masuk.");
            return StatusCode(500, new
            {
                success = false,
                message = "Terjadi kesalahan pada server."
            });
        }
    }

    // GET /api/surat/keluar
    // Page Surat Keluar
    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// Get daftar surat keluar untuk halaman Surat Keluar.
    /// </summary>
    [HttpGet("keluar")]
    [ProducesResponseType(typeof(PaginatedResponse<SuratListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuratKeluar([FromQuery] SuratQueryParams query)
    {
        try
        {
            var result = await _suratService.GetSuratKeluarAsync(query);
            return Ok(new
            {
                success = true,
                data    = result.Data,
                meta    = result.Meta
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get surat keluar.");
            return StatusCode(500, new
            {
                success = false,
                message = "Terjadi kesalahan pada server."
            });
        }
    }
}