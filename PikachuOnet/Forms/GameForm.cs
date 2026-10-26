using System.Drawing;
using PikachuOnet.Config;
using PikachuOnet.Core;
using PikachuOnet.Rendering;
using PikachuOnet.Services;

namespace PikachuOnet.Forms;

public partial class GameForm : Form
{
    private GameBoard _board = new(GameSettings.MaxRows, GameSettings.MaxCols);
    private readonly CardSet _cards;
    private readonly Image _background;
    private readonly BoardRenderer _renderer = new();
    private readonly BoardViewState _view = new();

    private readonly System.Windows.Forms.Timer _gameTimer = new() { Interval = 1000 };
    private readonly System.Windows.Forms.Timer _lineTimer = new() { Interval = GameSettings.LineDisplayMs };
    private readonly System.Windows.Forms.Timer _hintTimer = new() { Interval = GameSettings.HintDisplayMs };

    private Point _pendingA, _pendingB;
    private readonly SoundManager _sound;
    private int _score, _timeLeft, _shufflesLeft;
    private int _playSeconds;
    private int _level = 1;
    private LevelConfig _levelConfig;
    private bool _busy, _over;

    public GameForm()
    {
        _cards = CardImageLoader.Load(
            Path.Combine(AppContext.BaseDirectory, GameSettings.ImageFolder),
            GameSettings.MaxTypes);
        _background = BackgroundImageLoader.Load(
            Path.Combine(AppContext.BaseDirectory, GameSettings.ImageFolder, GameSettings.BackgroundSubfolder));

        _sound = new SoundManager(Path.Combine(AppContext.BaseDirectory, GameSettings.AudioFolder));

        InitializeUi();

        if (_background == null) BackColor = Color.FromArgb(34, 40, 58);

        _gameTimer.Tick += OnGameTick;
        _lineTimer.Tick += OnLineTimerTick;
        _hintTimer.Tick += OnHintTimerTick;

        UpdateHud();
        ApplyScreen();

        Shown += (_, _) => ShowStartupWarnings();
    }

    private void NewGame()
    {
        _level = 1;
        _score = 0;
        _playSeconds = 0;
        StartLevel();
    }

    private void StartLevel()
    {
        _gameTimer.Stop();
        _lineTimer.Stop();
        _hintTimer.Stop();

        _view.Reset();
        _busy = false;
        _over = false;
        _gameStarted = true;

        _levelConfig = LevelDesigner.Create(_level, _cards.TypeCount);
        _timeLeft = _levelConfig.Seconds;
        _shufflesLeft = _levelConfig.Shuffles;

        _board = new GameBoard(_levelConfig.Rows, _levelConfig.Cols);
        BoardLayout.Configure(_levelConfig.Rows, _levelConfig.Cols);
        _board.Deal(_levelConfig.TypeCount, GameSettings.CopiesPerType);

        UpdateHud();
        _gameTimer.Start();
        Invalidate();
    }

    private void RestartFromLevelOne()
    {
        if (_busy) return;

        _gameTimer.Stop();

        if (!_over && _level > 1)
        {
            var answer = MessageBox.Show(
                $"Bạn đang ở Level {_level}. Chơi mới sẽ quay về Level 1 và mất chuỗi level.\n\nVẫn chơi mới?",
                "Chơi mới", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes)
            {
                _gameTimer.Start();
                return;
            }
        }

        OfferSaveScore();
        NewGame();
    }

    private void EndGame(bool won)
    {
        _over = true;

        _gameTimer.Stop();
        _lineTimer.Stop();
        _hintTimer.Stop();
        _view.Connection = null;
        _view.ClearHint();
        _busy = false;
        Invalidate();

        if (won)
        {
            int bonus = _timeLeft * GameSettings.TimeBonusPerSecond;
            _score += bonus;
            UpdateHud();
            _sound.PlayEffect(SoundEffect.LevelComplete);

            var next = LevelDesigner.Create(_level + 1, _cards.TypeCount);
            MessageBox.Show(
                $"Hoàn thành Level {_level}!\nĐiểm: {_score} (thưởng thời gian {bonus})\n\n" +
                $"Level {next.Level}: bàn {next.Rows}x{next.Cols}, {next.CardCount} thẻ, {next.Seconds / 60}:{next.Seconds % 60:00} phút.",
                "Chiến thắng", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _level++;
            StartLevel();
        }
        else
        {
            UpdateHud();
            _sound.PlayEffect(SoundEffect.GameOver);
            var answer = MessageBox.Show(
                $"Hết giờ! Bạn dừng lại ở Level {_level} với {_score} điểm.\nChuỗi level đã bị mất.\n\nChơi lại từ Level 1?",
                "Thua cuộc", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            OfferSaveScore();

            if (answer == DialogResult.Yes)
            {
                NewGame();
            }
            else
            {
                _level = 1;
                _score = 0;
                ExitToMenu();
            }
        }
    }

    private void OnGameTick(object sender, EventArgs e)
    {
        _playSeconds++;
        if (--_timeLeft <= 0)
        {
            _timeLeft = 0;
            EndGame(false);
            return;
        }
        UpdateHud();
    }

    private void OnLineTimerTick(object sender, EventArgs e)
    {
        _lineTimer.Stop();

        _board.RemovePair(_pendingA, _pendingB);
        _view.Connection = null;
        _score += GameSettings.PointsPerPair;
        _busy = false;

        if (_board.RemainingCards == 0)
        {
            UpdateHud();
            Invalidate();
            EndGame(true);
            return;
        }

        if (!_board.HasMove()) _board.Reshuffle();

        UpdateHud();
        Invalidate();
    }

    private void OnHintTimerTick(object sender, EventArgs e)
    {
        _hintTimer.Stop();
        _view.ClearHint();
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (_screen != GameScreen.Playing || _over || _busy) return;

        try
        {
            if (!BoardLayout.TryHitTest(e.X, e.Y, _board, out Point cell)) return;

            _view.ClearHint();

            if (_view.Selected is null)
            {
                _view.Selected = cell;
                _sound.PlayEffect(SoundEffect.Select);
            }
            else if (_view.Selected == cell)
            {
                _view.Selected = null;
            }
            else
            {
                var first = _view.Selected.Value;
                List<Point> path;
                try
                {
                    path = PathFinder.FindPath(_board, first, cell);
                }
                catch (Exception ex) when (ex is IndexOutOfRangeException or ArgumentException or NullReferenceException)
                {
                    path = null;
                }

                if (path != null)
                    StartRemoval(first, cell, path);
                else
                {
                    _view.Selected = cell;
                    _sound.PlayEffect(SoundEffect.Wrong);
                }
            }
        }
        catch (Exception ex)
        {
            _lineTimer.Stop();
            _view.Selected = null;
            _view.Connection = null;
            _busy = false;
            MessageBox.Show(this,
                $"Có lỗi khi xử lý lượt chọn thẻ:\n{ex.Message}\n\nBạn có thể tiếp tục chơi.",
                "Lỗi chọn thẻ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        Invalidate();
    }

    private void StartRemoval(Point a, Point b, List<Point> path)
    {
        _busy = true;
        _pendingA = a;
        _pendingB = b;
        _view.Connection = path;
        _view.Selected = null;
        _sound.PlayEffect(SoundEffect.Correct);
        _lineTimer.Start();
    }

    private void ShowHint()
    {
        if (_over || _busy) return;
        if (!PathFinder.TryFindMove(_board, out Point a, out Point b)) return;

        _view.HintA = a;
        _view.HintB = b;
        _hintTimer.Stop();
        _hintTimer.Start();
        Invalidate();
    }

    private void UserShuffle()
    {
        if (_over || _busy || _shufflesLeft <= 0) return;

        _shufflesLeft--;
        _view.Reset();
        _board.Reshuffle();
        UpdateHud();
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        switch (_screen)
        {
            case GameScreen.Menu:
                DrawMenuScreen(e.Graphics);
                break;
            case GameScreen.Paused:
                DrawPausedScreen(e.Graphics);
                break;
            default:
                _renderer.Draw(e.Graphics, _board, _cards, _background, _view);
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _gameTimer.Dispose();
            _lineTimer.Dispose();
            _hintTimer.Dispose();
            _renderer.Dispose();
            _sound.Dispose();
            _cards.Dispose();
            _background?.Dispose();
        }
        base.Dispose(disposing);
    }
}