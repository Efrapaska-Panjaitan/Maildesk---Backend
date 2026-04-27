using Maildesk.Api.Dtos;
using Maildesk.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Maildesk.Api.Controllers;

[ApiController]
[Route("api/surat-masuk")]
public class SuratMasukController : ControllerBase
{
    private readonly SuratMasukService _service;

    public SuratMasukController(SuratMasukService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SuratMasukQueryDto request)
    {
        try
        {
            var result = await _service.GetAllAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}