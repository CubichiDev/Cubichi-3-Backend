namespace Cubichi.Services;

public class SkinService : ISkinService
{
    public SkinService()
    {
        // Create directories if they don't exist
        Directory.CreateDirectory(Path.Combine("files", "skins"));
    }
    public async Task UploadSkinAsync(string userName, IFormFile file)
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