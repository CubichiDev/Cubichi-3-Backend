namespace Cubichi.Services;
public interface ISkinService
{
    public Task<byte[]> GetSkinAsync(string userName);
    public Task UploadSkinAsync(string userName, IFormFile file);
}