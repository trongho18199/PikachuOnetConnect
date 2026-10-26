using System.Drawing;

namespace PikachuOnet.Services;

public static class CardImageLoader
{
    private static readonly string[] SupportedExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
    private const int MinTypes = 2;

    public static CardSet Load(string folder, int maxTypes)
    {
        var names = new List<string>();
        var images = new List<Image>();

        try
        {
            if (Directory.Exists(folder))
            {
                var files = Directory.GetFiles(folder)
                    .Where(f => SupportedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    .OrderBy(f => f, StringComparer.OrdinalIgnoreCase);

                foreach (var file in files)
                {
                    if (images.Count >= maxTypes) break;
                    try
                    {
                        var image = Image.FromStream(new MemoryStream(File.ReadAllBytes(file)));
                        images.Add(image);
                        names.Add(Path.GetFileNameWithoutExtension(file));
                    }
                    catch (Exception ex) when (IsFileOrImageError(ex))
                    {
                    }
                }
            }
        }
        catch (Exception ex) when (IsFileOrImageError(ex))
        {
        }

        if (images.Count >= MinTypes)
            return new CardSet(names.ToArray(), images.ToArray());

        foreach (var image in images) image.Dispose();
        return CreatePlaceholderSet(maxTypes);
    }

    private static CardSet CreatePlaceholderSet(int count)
    {
        var names = Enumerable.Range(1, count).Select(i => "#" + i).ToArray();
        return new CardSet(names, new Image[count]) { IsPlaceholder = true };
    }

    private static bool IsFileOrImageError(Exception ex) =>
        ex is ArgumentException or IOException or UnauthorizedAccessException
           or OutOfMemoryException or NotSupportedException or System.Security.SecurityException;
}
