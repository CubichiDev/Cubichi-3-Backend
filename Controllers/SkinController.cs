using System.IdentityModel.Tokens.Jwt;
using Cubichi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/skin")]
public class SkinController : ControllerBase
{
    private readonly ISkinService _skinService;

    public SkinController(ISkinService skinService)
    {
        _skinService = skinService;
    }

    [HttpGet("{userName}")]
    public async Task<ActionResult> GetSkinAsync(string userName)
    {
        try
        {
            var skin = await _skinService.GetSkinAsync(userName);
            return File(skin, "image/png");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("upload"), Authorize]
    public async Task<ActionResult> UploadSkinAsync([FromForm] IFormFile file)
    {
        try
        {
            string userName = User?.Identity?.Name!;
            await _skinService.UploadSkinAsync(userName, file);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}