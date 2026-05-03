using Maildesk.Api.Dtos;
using Maildesk.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Maildesk.Api.Controllers;

[ApiController]
[Route("api/surat-keluar")]
public class SuratKeluarController : ControllerBase
{
    private readonly SuratKeluarService _service;

    public SuratKeluarController(SuratKeluarService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SuratKeluarQueryDto request)
    {
        try
        {
            var result = await _service.GetAllAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSuratKeluarDto request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }
}