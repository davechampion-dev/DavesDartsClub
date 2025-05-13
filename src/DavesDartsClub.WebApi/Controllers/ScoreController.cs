using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Xml.Linq;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ScoreController : ControllerBase
{
    private readonly ILogger<ScoreController> _logger;
    public ScoreController(ILogger<ScoreController> logger)
    {
        _logger = logger;
    }
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello from ScoreController");
    }
    [HttpPost]
    public IActionResult Post([FromBody] string score)
    {
        // Process the score here
        return Ok($"Score received: {score}");
    }
}

    
    








