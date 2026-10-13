using Microsoft.AspNetCore.Mvc;

namespace SmartGatewayApi.Controllers;

[ApiController]
[Route("api/demo")]
public class DemoController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new
        {
            message = "pong",
            time = DateTime.UtcNow
        });
    }
}
