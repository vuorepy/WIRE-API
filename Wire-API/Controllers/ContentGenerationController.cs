
using Microsoft.AspNetCore.Mvc;
using Wire.DTO;
using Wire.DTO.ContentGeneration;
using Wire.Models;
using Wire.Routes;
using Wire.Services;

namespace Wire.Controllers;

[ApiController]
public class ContentGenerationController : ControllerBase
{
    private readonly IContentGenerationService _contentGenerationService;
    public ContentGenerationController(IContentGenerationService contentGenerationService)
    {
        _contentGenerationService = contentGenerationService;
    }

    [HttpPost]
    [Route(ApiRoutes.ContentGeneration.GenerateText)]
    public async Task<IActionResult> GenerateText([FromBody] GenerateContentDto generateContentDto)
    {
        var content = await _contentGenerationService.GenerateText(generateContentDto.Prompt);

        if (content == null)
        {
            return BadRequest("Content generation failed.");
        }

        return Ok(content);
    }
}