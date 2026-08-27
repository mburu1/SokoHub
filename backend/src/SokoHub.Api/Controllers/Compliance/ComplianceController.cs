using Microsoft.AspNetCore.Mvc;

namespace SokoHub.Api.Controllers.Compliance;

[ApiController]
[Route(""api/v1/compliance"")]
public sealed class ComplianceController : ControllerBase
{
    /// <summary>Kenya Data Protection Act — consent / subject-request stubs.</summary>
    [HttpGet(""health"")]
    public IActionResult Health() => Ok(new { status = ""ok"" });
}
