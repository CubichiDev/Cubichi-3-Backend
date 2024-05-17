namespace Cubichi.Services;
public interface ISkinService
{
    public Task UploadSkinAsync(string userName, IFileForm file);
}