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

    // ─────────────────────────────────────────────────────────
    // POST /api/surat
    // Catat surat masuk + upload file PDF (wajib, 1 request)
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Catat surat masuk baru beserta file PDF-nya.
    /// Kirim semua field sebagai <b>multipart/form-data</b> dalam satu request:
    /// data surat (NoSurat, TanggalSurat, Pengirim, dll) + file PDF wajib.
    /// Nomor agenda di-generate otomatis oleh sistem.
    /// </summary>
    /// <response code="201">Surat masuk berhasil dicatat.</response>
    /// <response code="400">Request tidak valid / file PDF tidak disertakan / nomor surat duplikat.</response>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(SuratResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSurat(
        [FromForm] CreateSuratWithFileRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new
            {
                success = false,
                errors  = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });

        if (request.File == null || request.File.Length == 0)
            return BadRequest(new { success = false, message = "File PDF wajib dilampirkan." });

        try
        {
            var result = await _suratService.CreateSuratMasukAsync(request, request.File);

            _logger.LogInformation(
                "Surat masuk berhasil dicatat. NomorAgenda: {NomorAgenda}", result.NomorAgenda);

            return CreatedAtAction(
                actionName: nameof(GetSuratById),
                routeValues: new { id = result.Id },
                value: new
                {
                    success = true,
                    message = "Surat masuk berhasil dicatat.",
                    data    = result
                });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Validasi gagal: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat mencatat surat masuk.");
            return StatusCode(500, new { success = false, message = "Terjadi kesalahan pada server." });
        }
    }

    // ─────────────────────────────────────────────────────────
    // GET /api/surat/{id}
    // ─────────────────────────────────────────────────────────
    /// <summary>Get detail surat berdasarkan ID.</summary>
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
        catch (KeyNotFoundException ex) { return NotFound(new { success = false, message = ex.Message }); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get surat ID: {Id}", id);
            return StatusCode(500, new { success = false, message = "Terjadi kesalahan pada server." });
        }
    }

    // ─────────────────────────────────────────────────────────
    // POST /api/surat/{id}/upload-pdf
    // Ganti file PDF untuk surat yang sudah ada
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Ganti file PDF untuk surat yang sudah ada.
    /// Gunakan endpoint ini jika perlu mengganti/memperbarui lampiran PDF.
    /// File lama akan dihapus otomatis.
    /// </summary>
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
            return Ok(new { success = true, message = "File PDF berhasil diganti.", data = result });
        }
        catch (KeyNotFoundException ex)      { return NotFound(new { success = false, message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { success = false, message = ex.Message }); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat upload PDF suratID: {Id}", id);
            return StatusCode(500, new { success = false, message = "Terjadi kesalahan pada server." });
        }
    }

    // ─────────────────────────────────────────────────────────
    // GET /api/surat/nomor-agenda/preview
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Get preview nomor agenda berikutnya untuk ditampilkan di form input.
    /// Nomor ini bersifat PREVIEW — nomor final dikonfirmasi saat simpan.
    /// </summary>
    [HttpGet("nomor-agenda/preview")]
    [ProducesResponseType(typeof(NomorAgendaPreviewResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNomorAgendaPreview()
    {
        try
        {
            var result = await _suratService.GetNomorAgendaPreviewAsync();
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat generate preview nomor agenda.");
            return StatusCode(500, new { success = false, message = "Terjadi kesalahan pada server." });
        }
    }

    // ─────────────────────────────────────────────────────────
    // GET /api/surat  — Dashboard semua surat (kode Akmal)
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Get semua surat untuk halaman Dashboard/Inbox.
    /// Mendukung filter, search, sorting, dan pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<SuratListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSurat([FromQuery] SuratQueryParams query)
    {
        try
        {
            var result = await _suratService.GetAllSuratAsync(query);
            return Ok(new { success = true, data = result.Data, meta = result.Meta });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get all surat.");
            return StatusCode(500, new { success = false, message = "Terjadi kesalahan pada server." });
        }
    }

    // ─────────────────────────────────────────────────────────
    // GET /api/surat/masuk — Surat Masuk dengan filter (kode Akmal)
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Get daftar surat masuk untuk halaman Surat Masuk.
    /// Mendukung filter (status, kategori, tanggal), search, sorting, dan pagination.
    /// </summary>
    [HttpGet("masuk")]
    [ProducesResponseType(typeof(PaginatedResponse<SuratListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuratMasuk([FromQuery] SuratQueryParams query)
    {
        try
        {
            var result = await _suratService.GetSuratMasukAsync(query);
            return Ok(new { success = true, data = result.Data, meta = result.Meta });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get surat masuk.");
            return StatusCode(500, new { success = false, message = "Terjadi kesalahan pada server." });
        }
    }

    // ─────────────────────────────────────────────────────────
    // GET /api/surat/statistik
    // Statistik jumlah surat & disposisi
    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Statistik jumlah surat (per jenis &amp; per status) dan jumlah disposisi (per status).
    /// </summary>
    [HttpGet("statistik")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatistik()
    {
        try
        {
            var result = await _suratService.GetStatistikAsync();
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat get statistik.");
            return StatusCode(500, new { success = false, message = "Terjadi kesalahan pada server." });
        }
    }
}