[ApiController]
[Route("api/skin")]
public class IAuthService : ControllerBase
{
    [HttpPost("upload"), Authorize]
    public async Task<ActionResult> UploadSkinAsync([FromForm] IFormFile file)
    {
        try
        {
            var userName = GetUserNameFromToken(Request.Headers["Authorization"]);
            await _skinService.UploadSkinAsync(userName, file);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    private static string GetUserNameFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        return jsonToken?.Claims.First(claim => claim.Type == "name").Value;
    }
}