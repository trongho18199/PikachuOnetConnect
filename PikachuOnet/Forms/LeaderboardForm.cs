using System.Drawing;
using PikachuOnet.Config;
using PikachuOnet.Services;

namespace PikachuOnet.Forms;

public sealed class LeaderboardForm : Form
{
    private static readonly Color Brown = Color.FromArgb(70, 45, 0);

    public LeaderboardForm(ScoreEntry highlight = null)
    {
        Text = "Bảng xếp hạng";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(460, 440);
        BackColor = Color.FromArgb(255, 250, 215);

        var title = new Label
        {
            Text = $"TOP {GameSettings.LeaderboardSize} ĐIỂM CAO NHẤT",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Brown,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(10, 10),
            Size = new Size(440, 36)
        };

        var list = new ListView
        {
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            HeaderStyle = ColumnHeaderStyle.Nonclickable,
            Font = new Font("Segoe UI", 11),
            Location = new Point(15, 55),
            Size = new Size(430, 300)
        };
        list.Columns.Add("Hạng", 60, HorizontalAlignment.Center);
        list.Columns.Add("Tên", 170);
        list.Columns.Add("Điểm", 90, HorizontalAlignment.Right);
        list.Columns.Add("Thời gian", 90, HorizontalAlignment.Center);

        var message = new Label
        {
            Font = new Font("Segoe UI", 10),
            ForeColor = Brown,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(15, 362),
            Size = new Size(430, 24)
        };

        bool loaded = ScoreService.TryLoad(out var entries, out string error);
        if (!loaded)
        {
            message.Text = "Không đọc được file thành tích: " + error;
        }
        else if (entries.Count == 0)
        {
            message.Text = "Chưa có thành tích nào. Hãy chơi và lưu điểm của bạn!";
        }

        bool highlighted = false;
        int rank = 0;
        foreach (var entry in entries.Take(GameSettings.LeaderboardSize))
        {
            rank++;
            var item = new ListViewItem(new[] { rank.ToString(), entry.Name, entry.Score.ToString(), entry.TimeText });
            if (!highlighted && highlight != null
                && entry.Name == highlight.Name && entry.Score == highlight.Score && entry.Seconds == highlight.Seconds)
            {
                item.BackColor = Color.FromArgb(255, 235, 120);
                item.Font = new Font(list.Font, FontStyle.Bold);
                highlighted = true;
            }
            list.Items.Add(item);
        }

        var closeButton = new RoundedButton
        {
            Text = "Đóng",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            CornerRadius = 16,
            BorderWidth = 2,
            Location = new Point(175, 392),
            Size = new Size(110, 38),
            DialogResult = DialogResult.OK
        };
        AcceptButton = closeButton;
        CancelButton = closeButton;

        Controls.AddRange(new Control[] { title, list, message, closeButton });
    }
}
