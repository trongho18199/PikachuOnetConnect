using System.Drawing;

namespace PikachuOnet.Services;

public sealed class CardSet : IDisposable
{
    private readonly string[] _names;
    private readonly Image[] _images;

    public CardSet(string[] names, Image[] images)
    {
        _names = names;
        _images = images;
    }

    public bool IsPlaceholder { get; init; }

    public int TypeCount => _names.Length;

    public string GetName(int type) => _names[type - 1];

    public Image GetImage(int type) => _images[type - 1];

    public void Dispose()
    {
        foreach (var image in _images) image?.Dispose();
    }
}
