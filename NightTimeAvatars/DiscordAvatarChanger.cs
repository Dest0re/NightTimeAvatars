using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NightTimeAvatars;

internal class InvalidImageFormat : Exception
{
    
}

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal class DiscordAvatarChanger : IDisposable
{
    private readonly HttpClient _httpClient = new();
    
    internal DiscordAvatarChanger(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
    
    private static string _imageToBase64(Image image)
    {
        using var memoryStream = new MemoryStream();
        
        image.Save(memoryStream, image.RawFormat);
        var imageBytes = memoryStream.ToArray();

        return Convert.ToBase64String(imageBytes);
    }

    private async Task<string> ChangeAvatarAsync(Image image)
    {
        string? imageFormat = null;

        if (ImageFormat.Png.Equals(image.RawFormat))
        {
            imageFormat = "png";
        } else if (ImageFormat.Jpeg.Equals(image.RawFormat))
        {
            imageFormat = "jpeg";
        } else if (ImageFormat.Gif.Equals(image.RawFormat))
        {
            imageFormat = "gif";
        }

        if (imageFormat == null) throw new InvalidImageFormat();

        var imageString = $"data:image/{imageFormat};base64," + _imageToBase64(image);

        var requestJsonString = JsonSerializer.Serialize(new
        {
            avatar = imageString
        });

        var response = await _httpClient.PatchAsync("https://ptb.discord.com/api/v9/users/@me", 
            new StringContent(requestJsonString, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    internal async Task<string> ChangeAvatarAsync(string filename)
    {
        var image = Image.FromFile(filename);

        return await ChangeAvatarAsync(image);
    }
}