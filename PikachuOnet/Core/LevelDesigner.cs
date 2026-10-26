using PikachuOnet.Config;

namespace PikachuOnet.Core;

public sealed record LevelConfig(int Level, int Rows, int Cols, int TypeCount, int Seconds, int Shuffles)
{
    public int CardCount => TypeCount * GameSettings.CopiesPerType;
}

public static class LevelDesigner
{
    private static readonly (int Rows, int Cols)[] Boards =
    {
        (6, 12),
        (6, 14),
        (6, 16),
        (7, 16),
        (8, 16),
        (8, 18),
    };

    private const double StartSecondsPerCard = 5.0;
    private const double SecondsPerCardDrop = 0.3;
    private const double MinSecondsPerCard = 2.6;

    public static LevelConfig Create(int level, int availableTypes)
    {
        level = Math.Max(1, level);

        var (rows, cols) = Boards[Math.Min(level, Boards.Length) - 1];
        rows = Math.Min(rows, GameSettings.MaxRows);
        cols = Math.Min(cols, GameSettings.MaxCols);

        int types = Math.Max(1, Math.Min(rows * cols / GameSettings.CopiesPerType, availableTypes));
        int cards = types * GameSettings.CopiesPerType;

        double secondsPerCard = Math.Max(MinSecondsPerCard, StartSecondsPerCard - SecondsPerCardDrop * (level - 1));
        int seconds = (int)Math.Round(cards * secondsPerCard / 10.0) * 10;

        int shuffles = Math.Max(1, 5 - (level - 1) / 2);

        return new LevelConfig(level, rows, cols, types, seconds, shuffles);
    }
}
