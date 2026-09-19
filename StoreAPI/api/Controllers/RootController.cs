namespace Api.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/")]
public class RootController : ControllerBase
{
    [HttpGet]
    public IActionResult GetRoot()
    {
        return Ok();
    }
}