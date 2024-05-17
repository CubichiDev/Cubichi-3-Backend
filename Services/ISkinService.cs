namespace Cubichi.Services;
public interface ISkinService
{
    public Task UploadSkinAsync(string userName, IFormFile file);
}