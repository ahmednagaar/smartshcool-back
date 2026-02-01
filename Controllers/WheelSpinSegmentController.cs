using Microsoft.AspNetCore.Mvc;
using Nafes.API.Modules;
using Nafes.API.Services;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WheelSpinSegmentController : ControllerBase
{
    private readonly IWheelSpinSegmentService _service;

    public WheelSpinSegmentController(IWheelSpinSegmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WheelSpinSegment>>> GetActiveSegments()
    {
        return Ok(await _service.GetActiveSegmentsAsync());
    }
}
