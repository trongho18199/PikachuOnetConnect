using System.Drawing;
using PikachuOnet.Config;

namespace PikachuOnet.Forms;

public sealed class HelpForm : Form
{
    private static readonly Color Brown = Color.FromArgb(70, 45, 0);

    public HelpForm()
    {
        Text = "Hướng dẫn & Thông tin nhóm";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(640, 520);
        BackColor = Color.FromArgb(255, 250, 215);

        var tabs = new TabControl
        {
            Location = new Point(10, 10),
            Size = new Size(620, 455),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        tabs.TabPages.Add(CreateGuideTab());
        tabs.TabPages.Add(CreateTeamTab());

        var closeButton = new RoundedButton
        {
            Text = "Đóng",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            CornerRadius = 16,
            BorderWidth = 2,
            Location = new Point(265, 473),
            Size = new Size(110, 38),
            DialogResult = DialogResult.OK
        };
        AcceptButton = closeButton;
        CancelButton = closeButton;

        Controls.AddRange(new Control[] { tabs, closeButton });
    }

    private static TabPage CreateGuideTab()
    {
        var page = new TabPage("Hướng dẫn chơi");

        string[] lines =
        {
            "MỤC TIÊU",
            "Xoá hết các thẻ trên bàn trước khi hết giờ để qua level tiếp theo.",
            "",
            "LUẬT NỐI",
            "• Click chọn hai thẻ GIỐNG NHAU. Thẻ đang chọn có viền đỏ; click lại thẻ đó để bỏ chọn.",
            "• Hai thẻ chỉ biến mất khi nối được bằng đường gấp khúc có TỐI ĐA 2 LẦN RẼ (tối đa 3 đoạn thẳng).",
            "• Đường nối chỉ được đi qua các ô trống và được phép đi vòng ra ngoài viền bàn.",
            "• Chọn hai thẻ không nối được thì thẻ vừa click trở thành thẻ đang chọn.",
            "",
            "ĐIỂM VÀ THỜI GIAN",
            $"• Mỗi cặp nối đúng được +{GameSettings.PointsPerPair} điểm.",
            $"• Qua level được thưởng thêm {GameSettings.TimeBonusPerSecond} điểm cho mỗi giây còn lại.",
            "• Hết giờ là thua; điểm và level bị mất (có thể lưu tên vào bảng xếp hạng).",
            "• Level càng cao thì bàn càng lớn và thời gian trên mỗi thẻ càng ít.",
            "",
            "CÁC NÚT",
            "• Gợi ý: sáng viền xanh một cặp thẻ đang nối được.",
            "• Xáo trộn: xáo lại các thẻ còn lại, số lượt hiện trong ngoặc và giới hạn theo từng level.",
            "  Khi bàn hết nước đi, game tự xáo trộn (không tốn lượt).",
            "• Chơi mới: quay về Level 1 (sẽ hỏi lại nếu đang ở level cao).",
            "• Nút hai vạch (hoặc phím Esc): tạm dừng; đồng hồ dừng cho tới khi bấm Tiếp tục.",
            "• Nút hình loa: bật/tắt nhạc nền và hiệu ứng âm thanh.",
            "",
            "BẢNG XẾP HẠNG",
            "Khi ván kết thúc, nhập tên để lưu Tên, Điểm, Thời gian chơi vào file Data/thanhtichs.txt.",
            "Bảng xếp hạng xếp theo điểm giảm dần; bằng điểm thì thời gian ngắn hơn xếp trên.",
        };

        page.Controls.Add(new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Font = new Font("Segoe UI", 10.5f),
            Text = string.Join(Environment.NewLine, lines),
            TabStop = false
        });
        return page;
    }

    private static TabPage CreateTeamTab()
    {
        var page = new TabPage("Thông tin nhóm") { BackColor = Color.White };

        var header = new Label
        {
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Brown,
            Location = new Point(12, 12),
            AutoSize = true,
            Text = string.Join(Environment.NewLine, new[]
            {
                $"Đề tài: {TeamInfo.ProjectName}",
                $"Nhóm: {TeamInfo.TeamName}",
                $"Môn học: {TeamInfo.CourseName}",
                $"Giảng viên: {TeamInfo.Lecturer}",
                "Công nghệ: C# WinForms (.NET 8)"
            })
        };

        var members = new DataGridView
        {
            Location = new Point(12, 155),
            Size = new Size(590, 265),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AllowUserToResizeColumns = false,
            RowHeadersVisible = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            EnableHeadersVisualStyles = false,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            ColumnHeadersHeight = 30,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
            ScrollBars = ScrollBars.Vertical
        };
        members.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        members.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 235, 170);
        members.ColumnHeadersDefaultCellStyle.ForeColor = Brown;
        members.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        members.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
        members.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        members.DefaultCellStyle.Padding = new Padding(3);
        members.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 245, 200);
        members.DefaultCellStyle.SelectionForeColor = Color.Black;

        members.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "STT", Width = 40, SortMode = DataGridViewColumnSortMode.NotSortable, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.TopCenter } });
        members.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Họ và tên", Width = 110, SortMode = DataGridViewColumnSortMode.NotSortable });
        members.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "MSSV", Width = 95, SortMode = DataGridViewColumnSortMode.NotSortable });
        members.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Lớp", Width = 60, SortMode = DataGridViewColumnSortMode.NotSortable });
        members.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Vai trò", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, SortMode = DataGridViewColumnSortMode.NotSortable });

        int stt = 0;
        foreach (var m in TeamInfo.Members)
        {
            stt++;
            members.Rows.Add(stt.ToString(), m.FullName, m.StudentId, m.ClassName, m.Role);
        }
        members.ClearSelection();

        page.Controls.Add(header);
        page.Controls.Add(members);
        return page;
    }
}