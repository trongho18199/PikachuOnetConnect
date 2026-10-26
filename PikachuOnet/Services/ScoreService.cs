using System.Globalization;
using System.Text;
using PikachuOnet.Config;

namespace PikachuOnet.Services;

public sealed record ScoreEntry(string Name, int Score, int Seconds)
{
    public string TimeText => FormatTime(Seconds);

    public static string FormatTime(int seconds)
    {
        seconds = Math.Max(0, seconds);
        return $"{seconds / 60:00}:{seconds % 60:00}";
    }
}

public static class ScoreService
{
    public static string FilePath =>
        Path.Combine(AppContext.BaseDirectory, GameSettings.DataFolder, GameSettings.ScoreFileName);

    public static string CleanName(string name)
    {
        var sb = new StringBuilder();
        foreach (char c in name ?? "")
            sb.Append(char.IsControl(c) || c == ',' ? ' ' : c);

        string cleaned = string.Join(' ', sb.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        if (cleaned.Length > GameSettings.MaxPlayerNameLength)
            cleaned = cleaned[..GameSettings.MaxPlayerNameLength].Trim();
        return cleaned;
    }

    public static bool TrySave(ScoreEntry entry, out string error)
    {
        error = null;
        string name = CleanName(entry.Name);
        if (name.Length == 0)
        {
            error = "Tên người chơi trống.";
            return false;
        }

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            string prefix = EndsWithoutNewLine(FilePath) ? Environment.NewLine : "";
            string line = $"{name},{Math.Max(0, entry.Score)},{ScoreEntry.FormatTime(entry.Seconds)}";
            File.AppendAllText(FilePath, prefix + line + Environment.NewLine, new UTF8Encoding(false));
            return true;
        }
        catch (Exception ex) when (IsFileError(ex))
        {
            error = ex.Message;
            return false;
        }
    }

    public static bool TryLoad(out List<ScoreEntry> entries, out string error)
    {
        entries = new List<ScoreEntry>();
        error = null;

        if (!File.Exists(FilePath)) return true;

        string[] lines;
        try
        {
            lines = File.ReadAllLines(FilePath, Encoding.UTF8);
        }
        catch (Exception ex) when (IsFileError(ex))
        {
            error = ex.Message;
            return false;
        }

        foreach (string line in lines)
            if (TryParseLine(line, out var entry))
                entries.Add(entry);

        entries = entries
            .OrderByDescending(e => e.Score)
            .ThenBy(e => e.Seconds)
            .ThenBy(e => e.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
        return true;
    }

    public static bool TryParseLine(string line, out ScoreEntry entry)
    {
        entry = null;
        if (string.IsNullOrWhiteSpace(line)) return false;

        line = line.Trim();
        if (line.StartsWith('#')) return false;

        string[] parts = line.Split(',');
        if (parts.Length != 3) return false;

        string name = parts[0].Trim();
        if (name.Length == 0) return false;
        if (!int.TryParse(parts[1].Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int score)) return false;
        if (!TryParseTime(parts[2].Trim(), out int seconds)) return false;

        entry = new ScoreEntry(name, score, seconds);
        return true;
    }

    private static bool TryParseTime(string text, out int seconds)
    {
        seconds = 0;
        string[] parts = text.Split(':');
        if (parts.Length != 2) return false;
        if (!int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out int minutes)) return false;
        if (!int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out int secs) || secs >= 60) return false;

        seconds = minutes * 60 + secs;
        return true;
    }

    private static bool EndsWithoutNewLine(string path)
    {
        var info = new FileInfo(path);
        if (!info.Exists || info.Length == 0) return false;

        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        stream.Seek(-1, SeekOrigin.End);
        return stream.ReadByte() != '\n';
    }

    private static bool IsFileError(Exception ex) =>
        ex is IOException or UnauthorizedAccessException or NotSupportedException
           or ArgumentException or System.Security.SecurityException;
}
