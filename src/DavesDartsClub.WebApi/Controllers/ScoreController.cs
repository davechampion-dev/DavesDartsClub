using Microsoft.AspNetCore.Mvc;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ScoreController : ControllerBase
{
#pragma warning disable S125
    // namespace AdminAssistant.WebAPI.v1;
    //
    // public sealed class MappingProfile : MappingProfileBase
    // {
    //     public MappingProfile()
    //         : base(typeof(MappingProfile).Assembly)
    //     {
    //     }
    // }
#pragma warning restore S125
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











