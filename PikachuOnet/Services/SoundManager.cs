using System.Media;
using System.Runtime.InteropServices;

namespace PikachuOnet.Services;

public enum SoundEffect { Select, Correct, Wrong, LevelComplete, GameOver }

public sealed class SoundManager : IDisposable
{
    private const string MusicAlias = "pikachu_bgm";
    private const string MusicFileName = "bgm.wav";

    private static readonly Dictionary<SoundEffect, string> EffectFiles = new()
    {
        [SoundEffect.Select] = "select.wav",
        [SoundEffect.Correct] = "correct.wav",
        [SoundEffect.Wrong] = "wrong.wav",
        [SoundEffect.LevelComplete] = "win.wav",
        [SoundEffect.GameOver] = "lose.wav",
    };

    [DllImport("winmm.dll", CharSet = CharSet.Unicode)]
    private static extern int mciSendString(string command, System.Text.StringBuilder returnValue, int returnLength, IntPtr callback);

    private readonly string _folder;
    private readonly Dictionary<SoundEffect, SoundPlayer> _effects = new();
    private readonly List<string> _problems = new();
    private bool _musicUsable;
    private bool _musicOpened;
    private bool _musicPlaying;

    public SoundManager(string folder)
    {
        _folder = folder;

        foreach (var (effect, fileName) in EffectFiles)
        {
            string path = Path.Combine(folder, fileName);
            if (!File.Exists(path))
            {
                _problems.Add($"Thiếu file âm thanh: {fileName}");
                continue;
            }

            try
            {
                var player = new SoundPlayer(path);
                player.Load();
                _effects[effect] = player;
            }
            catch (Exception ex)
            {
                _problems.Add($"Không nạp được {fileName}: {ex.Message}");
            }
        }

        _musicUsable = File.Exists(Path.Combine(folder, MusicFileName));
        if (!_musicUsable) _problems.Add($"Thiếu file nhạc nền: {MusicFileName}");
    }

    public bool Enabled { get; private set; } = true;

    public IReadOnlyList<string> Problems => _problems;

    public bool Toggle()
    {
        Enabled = !Enabled;
        if (!Enabled) StopMusic();
        return Enabled;
    }

    public void PlayEffect(SoundEffect effect)
    {
        if (!Enabled || !_effects.TryGetValue(effect, out var player)) return;

        try
        {
            player.Play();
        }
        catch (Exception)
        {
        }
    }

    public void StartMusic()
    {
        if (!Enabled || !_musicUsable || _musicPlaying) return;

        try
        {
            string path = Path.Combine(_folder, MusicFileName);
            if (!_musicOpened)
            {
                bool opened = Mci($"open \"{path}\" type mpegvideo alias {MusicAlias}") == 0
                           || Mci($"open \"{path}\" type waveaudio alias {MusicAlias}") == 0;
                if (!opened)
                {
                    _musicUsable = false;
                    _problems.Add("Không mở được nhạc nền. Game vẫn chơi bình thường, chỉ không có nhạc nền.");
                    return;
                }
                _musicOpened = true;
            }

            if (Mci($"play {MusicAlias} from 0 repeat") != 0 && Mci($"play {MusicAlias} from 0") != 0)
            {
                _musicUsable = false;
                return;
            }
            _musicPlaying = true;
        }
        catch (Exception)
        {
            _musicUsable = false;
        }
    }

    public void StopMusic()
    {
        if (!_musicPlaying) return;
        _musicPlaying = false;

        try
        {
            Mci($"stop {MusicAlias}");
        }
        catch (Exception)
        {
        }
    }

    private static int Mci(string command) => mciSendString(command, null, 0, IntPtr.Zero);

    public void Dispose()
    {
        StopMusic();
        if (_musicOpened)
        {
            try { Mci($"close {MusicAlias}"); } catch (Exception) { }
            _musicOpened = false;
        }
        foreach (var player in _effects.Values) player.Dispose();
        _effects.Clear();
    }
}
