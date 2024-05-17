namespace Cubichi.Services;

public class SkinService : ISkinService
{
    public async Task UploadSkinAsync(string userName, IFileForm file)
    {
        try
        {
            // File mode create or change if exists
            using (var stream = new FileStream(Path.Combine("files", "skins", userName + ".png"), FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(stream);
            }
        }
        catch (Exception e)
        {
            throw new Exception("Failed to upload skin", e);
        }
    }
}