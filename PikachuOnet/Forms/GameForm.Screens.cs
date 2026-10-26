using System.Drawing;
using System.Drawing.Drawing2D;
using PikachuOnet.Config;
using PikachuOnet.Rendering;

namespace PikachuOnet.Forms;

public partial class GameForm
{
    private enum GameScreen { Menu, Playing, Paused }

    private GameScreen _screen = GameScreen.Menu;
    private bool _gameStarted;
    private bool _lineWasRunning;

    private readonly RoundedButton _startButton = new();
    private readonly RoundedButton _continueButton = new();
    private readonly RoundedButton _exitButton = new();
    private readonly RoundedButton _leaderboardButton = new();
    private readonly RoundedButton _helpButton = new();
    private readonly RoundedButton _menuSoundButton = new();

    private readonly RoundedButton _pauseButton = new();

    private readonly RoundedButton _resumeButton = new();
    private readonly RoundedButton _quitToMenuButton = new();

    private static readonly Color PikachuYellow = Color.FromArgb(255, 213, 0);
    private static readonly Color PikachuBrown = Color.FromArgb(70, 45, 0);
    private static readonly Color ExitRed = Color.FromArgb(240, 110, 90);

    private const int MenuButtonWidth = 260, MenuButtonHeight = 56, MenuButtonGap = 16;
    private const int PausePanelWidth = 360, PausePanelHeight = 300;

    private bool CanContinue => _gameStarted && !_over;

    private void InitializeScreens()
    {
        KeyPreview = true;

        int centerX = BoardLayout.ClientWidth / 2;

        int top = 230;
        SetupBigButton(_startButton, "Bắt đầu", centerX, top, PikachuYellow);
        SetupBigButton(_continueButton, "Tiếp tục", centerX, top + MenuButtonHeight + MenuButtonGap, PikachuYellow);
        SetupBigButton(_exitButton, "Thoát", centerX, top + (MenuButtonHeight + MenuButtonGap) * 2, ExitRed);

        _startButton.Click += (_, _) => StartFromMenu();
        _continueButton.Click += (_, _) => ResumeGame();
        _exitButton.Click += (_, _) => Close();

        int smallTop = top + (MenuButtonHeight + MenuButtonGap) * 3;
        int smallWidth = (MenuButtonWidth - 10) / 2;
        SetupSmallButton(_leaderboardButton, "Xếp hạng", centerX - MenuButtonWidth / 2, smallTop, smallWidth);
        SetupSmallButton(_helpButton, "Hướng dẫn", centerX + 5, smallTop, smallWidth);
        _leaderboardButton.Click += (_, _) => ShowLeaderboard();
        _helpButton.Click += (_, _) => ShowHelp();

        _menuSoundButton.SetBounds(BoardLayout.ClientWidth - 10 - 44, 14, 44, 32);
        _menuSoundButton.CornerRadius = 14;
        _menuSoundButton.BorderWidth = 2;
        _menuSoundButton.IconPainter = PaintSoundIcon;
        _menuSoundButton.Visible = false;
        _menuSoundButton.Click += (_, _) => ToggleSound();
        Controls.Add(_menuSoundButton);

        var panel = PausePanelBounds;
        int pauseTop = panel.Y + 110;
        SetupBigButton(_resumeButton, "Tiếp tục", centerX, pauseTop, PikachuYellow);
        SetupBigButton(_quitToMenuButton, "Thoát", centerX, pauseTop + MenuButtonHeight + MenuButtonGap, ExitRed);

        _resumeButton.Click += (_, _) => ResumeGame();
        _quitToMenuButton.Click += (_, _) => ExitToMenu();

        _pauseButton.CornerRadius = 14;
        _pauseButton.BorderWidth = 2;
        _pauseButton.IconPainter = PaintPauseIcon;
        _pauseButton.Click += (_, _) => PauseGame();
        Controls.Add(_pauseButton);
    }

    private void SetupBigButton(RoundedButton button, string text, int centerX, int top, Color fill)
    {
        button.Text = text;
        button.Size = new Size(MenuButtonWidth, MenuButtonHeight);
        button.Location = new Point(centerX - MenuButtonWidth / 2, top);
        button.Font = new Font("Segoe UI", 16, FontStyle.Bold);
        button.FillColor = fill;
        button.BorderColor = PikachuBrown;
        button.BorderWidth = 3;
        button.CornerRadius = MenuButtonHeight / 2;
        button.Visible = false;
        Controls.Add(button);
    }

    private void SetupSmallButton(RoundedButton button, string text, int left, int top, int width)
    {
        button.Text = text;
        button.SetBounds(left, top, width, 44);
        button.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        button.FillColor = Color.White;
        button.BorderColor = PikachuBrown;
        button.BorderWidth = 3;
        button.CornerRadius = 22;
        button.Visible = false;
        Controls.Add(button);
    }

    private void PaintSoundIcon(Graphics g, Rectangle bounds)
    {
        int cx = bounds.X + bounds.Width / 2 - 4;
        int cy = bounds.Y + bounds.Height / 2;

        using var brush = new SolidBrush(PikachuBrown);
        using var pen = new Pen(PikachuBrown, 2);

        g.FillRectangle(brush, cx - 10, cy - 3, 5, 6);
        g.FillPolygon(brush, new[]
        {
            new Point(cx - 5, cy - 3), new Point(cx + 2, cy - 9),
            new Point(cx + 2, cy + 9), new Point(cx - 5, cy + 3)
        });

        if (_sound.Enabled)
        {
            g.DrawArc(pen, cx - 2, cy - 5, 10, 10, -50, 100);
            g.DrawArc(pen, cx - 4, cy - 9, 18, 18, -50, 100);
        }
        else
        {
            using var red = new Pen(Color.FromArgb(200, 40, 30), 3);
            g.DrawLine(red, cx + 6, cy - 6, cx + 14, cy + 6);
            g.DrawLine(red, cx + 14, cy - 6, cx + 6, cy + 6);
        }
    }

    private static void PaintPauseIcon(Graphics g, Rectangle bounds)
    {
        const int barWidth = 6, barHeight = 16, gap = 6;
        int totalWidth = barWidth * 2 + gap;
        int x = bounds.X + (bounds.Width - totalWidth) / 2;
        int y = bounds.Y + (bounds.Height - barHeight) / 2;

        using var brush = new SolidBrush(PikachuBrown);
        g.FillRectangle(brush, x, y, barWidth, barHeight);
        g.FillRectangle(brush, x + barWidth + gap, y, barWidth, barHeight);
    }

    private void StartFromMenu()
    {
        NewGame();
        _screen = GameScreen.Playing;
        ApplyScreen();
    }

    private void PauseGame()
    {
        if (_screen != GameScreen.Playing) return;

        _gameTimer.Stop();
        _lineWasRunning = _lineTimer.Enabled;
        _lineTimer.Stop();
        _hintTimer.Stop();
        _view.ClearHint();

        _screen = GameScreen.Paused;
        ApplyScreen();
    }

    private void ResumeGame()
    {
        if (_screen == GameScreen.Menu && !CanContinue) return;

        _screen = GameScreen.Playing;
        if (!_over) _gameTimer.Start();
        if (_lineWasRunning)
        {
            _lineWasRunning = false;
            _lineTimer.Start();
        }
        ApplyScreen();
    }

    private void ExitToMenu()
    {
        _screen = GameScreen.Menu;
        ApplyScreen();
    }

    private void ApplyScreen()
    {
        bool menu = _screen == GameScreen.Menu;
        bool playing = _screen == GameScreen.Playing;
        bool paused = _screen == GameScreen.Paused;

        _levelLabel.Visible = !menu;
        _scoreLabel.Visible = !menu;
        _timeLabel.Visible = !menu;
        _hintButton.Visible = playing;
        _shuffleButton.Visible = playing;
        _newGameButton.Visible = playing;
        _pauseButton.Visible = playing;
        _soundButton.Visible = playing;

        _startButton.Visible = menu;
        _continueButton.Visible = menu;
        _exitButton.Visible = menu;
        _leaderboardButton.Visible = menu;
        _helpButton.Visible = menu;
        _menuSoundButton.Visible = menu;

        _resumeButton.Visible = paused;
        _quitToMenuButton.Visible = paused;

        _continueButton.Enabled = CanContinue;
        _continueButton.Cursor = CanContinue ? Cursors.Hand : Cursors.Default;

        UpdateMusic();
        Invalidate();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode != Keys.Escape) return;

        if (_screen == GameScreen.Playing) PauseGame();
        else if (_screen == GameScreen.Paused) ResumeGame();
    }

    private Rectangle PausePanelBounds
    {
        get
        {
            int w = BoardLayout.ClientWidth;
            int h = BoardLayout.ClientHeight;
            int areaTop = GameSettings.TopBarHeight;
            return new Rectangle((w - PausePanelWidth) / 2,
                                 areaTop + (h - areaTop - PausePanelHeight) / 2,
                                 PausePanelWidth, PausePanelHeight);
        }
    }

    private void DrawMenuScreen(Graphics g)
    {
        PrepareGraphics(g);
        BoardRenderer.DrawBackground(g, _background);

        int w = BoardLayout.ClientWidth;
        DrawOutlinedText(g, "PIKACHU ONET", 56, new RectangleF(0, 70, w, 90), PikachuYellow, PikachuBrown);
        DrawOutlinedText(g, "Trò chơi nối hình", 22, new RectangleF(0, 160, w, 40), Color.White, PikachuBrown);
    }

    private void DrawPausedScreen(Graphics g)
    {
        PrepareGraphics(g);
        BoardRenderer.DrawBackground(g, _background);

        int w = BoardLayout.ClientWidth;
        int h = BoardLayout.ClientHeight;

        using (var dim = new SolidBrush(Color.FromArgb(140, 0, 0, 0)))
            g.FillRectangle(dim, 0, 0, w, h);

        var panel = PausePanelBounds;
        using (var path = RoundedRectangle(panel, 24))
        using (var fill = new SolidBrush(Color.FromArgb(245, 255, 250, 215)))
        using (var border = new Pen(PikachuBrown, 3))
        {
            g.FillPath(fill, path);
            g.DrawPath(border, path);
        }

        DrawOutlinedText(g, "TẠM DỪNG", 34, new RectangleF(panel.X, panel.Y + 20, panel.Width, 70), PikachuYellow, PikachuBrown);
    }

    private static void PrepareGraphics(Graphics g)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.Half;
    }

    private static void DrawOutlinedText(Graphics g, string text, float emSize, RectangleF area, Color fill, Color outline)
    {
        using var family = new FontFamily("Segoe UI");
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        using var path = new GraphicsPath();
        path.AddString(text, family, (int)FontStyle.Bold, emSize, area, format);

        using var pen = new Pen(outline, Math.Max(3f, emSize / 9f)) { LineJoin = LineJoin.Round };
        using var brush = new SolidBrush(fill);
        g.DrawPath(pen, path);
        g.FillPath(brush, path);
    }

    private static GraphicsPath RoundedRectangle(Rectangle r, int radius)
    {
        int d = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}
