using System.Drawing;

namespace PikachuOnet.Services;

public static class BackgroundImageLoader
{
    private static readonly string[] SupportedExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };

    public static Image Load(string folder)
    {
        try
        {
            if (!Directory.Exists(folder)) return null;

            var file = Directory.GetFiles(folder)
                .Where(f => SupportedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();
            if (file == null) return null;

            return Image.FromStream(new MemoryStream(File.ReadAllBytes(file)));
        }
        catch (Exception ex) when (IsFileOrImageError(ex))
        {
            return null;
        }
    }

    private static bool IsFileOrImageError(Exception ex) =>
        ex is ArgumentException or IOException or UnauthorizedAccessException
           or OutOfMemoryException or NotSupportedException or System.Security.SecurityException;
}
