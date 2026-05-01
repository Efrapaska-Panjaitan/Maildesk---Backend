using MailDesk.API.DTOs.SuratMasuk;
using MailDesk.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MailDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SuratMasukController : ControllerBase
{
    private readonly ISuratMasukService _suratMasukService;
    private readonly ILogger<SuratMasukController> _logger;

    public SuratMasukController(
        ISuratMasukService suratMasukService,
        ILogger<SuratMasukController> logger)
    {
        _suratMasukService = suratMasukService;
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
    [ProducesResponseType(typeof(SuratMasukResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSuratMasuk(
        [FromBody] CreateSuratMasukRequest request)
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
            var result = await _suratMasukService.CreateSuratMasukAsync(request);

            _logger.LogInformation(
                "Surat masuk berhasil dicatat. NomorAgenda: {NomorAgenda}, NoSurat: {NoSurat}",
                result.NomorAgenda, result.NoSurat);

            return CreatedAtAction(
                actionName: nameof(GetSuratMasukById), // akan dibuat di task tracking
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
            _logger.LogWarning("Duplikat surat masuk: {Message}", ex.Message);
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

    /// <summary>
    /// Get surat masuk by ID. (Placeholder — akan dikembangkan di task tracking)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSuratMasukById(int id)
    {
        // Akan diimplementasikan penuh di task: API tracking disposisi
        return Ok(new { message = $"Endpoint get by ID {id} - akan dikembangkan di task tracking." });
    }
}