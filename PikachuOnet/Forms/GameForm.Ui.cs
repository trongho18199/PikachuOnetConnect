using System.Drawing;
using PikachuOnet.Config;
using PikachuOnet.Rendering;

namespace PikachuOnet.Forms;

public partial class GameForm
{
    private readonly Label _levelLabel = new();
    private readonly Label _scoreLabel = new();
    private readonly Label _timeLabel = new();
    private readonly RoundedButton _hintButton = new();
    private readonly RoundedButton _shuffleButton = new();
    private readonly RoundedButton _newGameButton = new();
    private readonly RoundedButton _soundButton = new();

    private void InitializeUi()
    {
        Text = "Pikachu Onet - Nối hình";
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        DoubleBuffered = true;
        BackColor = Color.White;
        ClientSize = new Size(BoardLayout.ClientWidth, BoardLayout.ClientHeight);

        var hudFont = new Font("Segoe UI", 12, FontStyle.Bold);
        int width = ClientSize.Width;
        _levelLabel.Location = new Point(10, 16);
        _scoreLabel.Location = new Point(140, 16);
        _timeLabel.Location = new Point(340, 16);
        foreach (var label in new[] { _levelLabel, _scoreLabel, _timeLabel })
        {
            label.AutoSize = true;
            label.Font = hudFont;
            label.BackColor = Color.Transparent;
            label.ForeColor = Color.White;
        }
        _levelLabel.ForeColor = Color.FromArgb(255, 213, 0);

        const int margin = 10, buttonY = 14, buttonHeight = 32, pauseWidth = 44;
        int right = width - margin - pauseWidth - margin;
        _pauseButton.SetBounds(width - margin - pauseWidth, buttonY, pauseWidth, buttonHeight);
        _newGameButton.SetBounds(right - 100, buttonY, 100, buttonHeight);
        _shuffleButton.SetBounds(right - 100 - margin - 120, buttonY, 120, buttonHeight);
        _hintButton.SetBounds(right - 100 - margin - 120 - margin - 100, buttonY, 100, buttonHeight);
        _hintButton.Text = "Gợi ý";
        _newGameButton.Text = "Chơi mới";

        _soundButton.SetBounds(_hintButton.Left - margin - pauseWidth, buttonY, pauseWidth, buttonHeight);
        _soundButton.CornerRadius = 14;
        _soundButton.BorderWidth = 2;
        _soundButton.IconPainter = PaintSoundIcon;
        _soundButton.Click += (_, _) => ToggleSound();

        foreach (var button in new[] { _hintButton, _shuffleButton, _newGameButton })
        {
            button.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            button.CornerRadius = 14;
            button.BorderWidth = 2;
        }

        _hintButton.Click += (_, _) => ShowHint();
        _shuffleButton.Click += (_, _) => UserShuffle();
        _newGameButton.Click += (_, _) => RestartFromLevelOne();

        Controls.AddRange(new Control[] { _levelLabel, _scoreLabel, _timeLabel, _hintButton, _shuffleButton, _newGameButton, _soundButton });

        InitializeScreens();
    }

    private void UpdateHud()
    {
        _levelLabel.Text = $"Level {_level}";
        _scoreLabel.Text = $"Điểm: {_score}";
        _timeLabel.Text = $"Còn: {_timeLeft / 60}:{_timeLeft % 60:00}";
        _shuffleButton.Text = $"Xáo trộn ({_shufflesLeft})";
        _shuffleButton.Enabled = _shufflesLeft > 0 && !_over;

        foreach (var label in new[] { _levelLabel, _scoreLabel, _timeLabel })
            label.Top = (GameSettings.TopBarHeight - label.Height) / 2;
    }
}
