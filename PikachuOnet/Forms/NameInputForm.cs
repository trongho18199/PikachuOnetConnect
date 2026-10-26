using System.Drawing;
using PikachuOnet.Config;
using PikachuOnet.Services;

namespace PikachuOnet.Forms;

public sealed class NameInputForm : Form
{
    private static readonly Color Brown = Color.FromArgb(70, 45, 0);

    private readonly TextBox _nameBox = new();

    public string PlayerName { get; private set; } = "";

    public NameInputForm(int score, string timeText)
    {
        Text = "Lưu thành tích";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(380, 200);
        BackColor = Color.FromArgb(255, 250, 215);

        var info = new Label
        {
            Text = $"Điểm: {score}     Thời gian chơi: {timeText}",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Brown,
            Location = new Point(20, 20),
            Size = new Size(340, 28)
        };

        var prompt = new Label
        {
            Text = "Nhập tên của bạn để lưu vào bảng xếp hạng:",
            Font = new Font("Segoe UI", 10),
            ForeColor = Brown,
            Location = new Point(20, 62),
            Size = new Size(340, 24)
        };

        _nameBox.Font = new Font("Segoe UI", 12);
        _nameBox.MaxLength = GameSettings.MaxPlayerNameLength;
        _nameBox.Location = new Point(20, 90);
        _nameBox.Size = new Size(340, 30);

        var saveButton = new RoundedButton { Text = "Lưu", Location = new Point(60, 140), Size = new Size(110, 38) };
        var skipButton = new RoundedButton { Text = "Bỏ qua", Location = new Point(210, 140), Size = new Size(110, 38) };
        foreach (var button in new[] { saveButton, skipButton })
        {
            button.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            button.CornerRadius = 16;
            button.BorderWidth = 2;
        }
        skipButton.FillColor = Color.FromArgb(235, 235, 235);
        skipButton.DialogResult = DialogResult.Cancel;
        saveButton.Click += (_, _) => TrySubmit();

        AcceptButton = saveButton;
        CancelButton = skipButton;

        Controls.AddRange(new Control[] { info, prompt, _nameBox, saveButton, skipButton });
        Shown += (_, _) => _nameBox.Focus();
    }

    private void TrySubmit()
    {
        string name = ScoreService.CleanName(_nameBox.Text);
        if (name.Length == 0)
        {
            MessageBox.Show(this, "Vui lòng nhập tên (không để trống) hoặc bấm \"Bỏ qua\" nếu không muốn lưu.",
                "Chưa nhập tên", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _nameBox.Focus();
            return;
        }

        PlayerName = name;
        DialogResult = DialogResult.OK;
    }
}
