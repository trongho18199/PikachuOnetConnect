using PikachuOnet.Services;

namespace PikachuOnet.Forms;

public partial class GameForm
{
    private void OfferSaveScore()
    {
        if (_score <= 0) return;

        try
        {
            using var dialog = new NameInputForm(_score, ScoreEntry.FormatTime(_playSeconds));
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            var entry = new ScoreEntry(dialog.PlayerName, _score, _playSeconds);
            if (!ScoreService.TrySave(entry, out string error))
            {
                MessageBox.Show(this,
                    $"Không lưu được thành tích:\n{error}\n\nBạn vẫn có thể tiếp tục chơi.",
                    "Lỗi lưu thành tích", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ShowLeaderboard(entry);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this,
                $"Có lỗi khi lưu thành tích:\n{ex.Message}\n\nBạn vẫn có thể tiếp tục chơi.",
                "Lỗi lưu thành tích", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ShowLeaderboard(ScoreEntry highlight = null)
    {
        try
        {
            using var form = new LeaderboardForm(highlight);
            form.ShowDialog(this);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this,
                $"Không mở được bảng xếp hạng:\n{ex.Message}",
                "Lỗi bảng xếp hạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ShowHelp()
    {
        using var form = new HelpForm();
        form.ShowDialog(this);
    }

    private void ToggleSound()
    {
        _sound.Toggle();
        UpdateMusic();
        _soundButton.Invalidate();
        _menuSoundButton.Invalidate();
    }

    private void UpdateMusic()
    {
        if (_sound.Enabled && _screen != GameScreen.Paused) _sound.StartMusic();
        else _sound.StopMusic();
    }

    private void ShowStartupWarnings()
    {
        var notes = new List<string>();

        if (_cards.IsPlaceholder)
            notes.Add("• Không có ít nhất 2 ảnh thẻ hợp lệ trong thư mục \"Images\": game dùng số thứ tự thay cho ảnh.");
        if (_background == null)
            notes.Add("• Không tải được ảnh nền (Images\\Background): game dùng nền màu đơn.");
        foreach (string problem in _sound.Problems)
            notes.Add("• " + problem);

        if (notes.Count == 0) return;

        MessageBox.Show(this,
            "Một số tài nguyên bị thiếu hoặc lỗi:\n\n" + string.Join("\n", notes) + "\n\nGame vẫn chạy bình thường.",
            "Thiếu tài nguyên", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}