namespace PikachuOnet.Config;

public sealed record TeamMember(string FullName, string StudentId, string ClassName, string Role);

public static class TeamInfo
{
    public const string ProjectName = "Pikachu Onet - Trò chơi nối hình";
    public const string TeamName = "NanoSecond";
    public const string CourseName = "Lập trình Windows";
    public const string Lecturer = "Th.S Lê Văn Nhân";

    public static readonly TeamMember[] Members =
    {
        new("Hồ Phước Trọng", "51.01.104.107", "CNTT.A", "Thiết kế form giao diện chính, xây dựng logic game, các nút tương tác, thiết kế hình ảnh, edit video"),
        new("Võ Thị Hồng Ân", "51.01.104.004", "CNTT.C", "Thiết kế form nhập tên khi kết thúc, kiểm thử trò chơi, âm thanh, quay video demo và thuyết minh"),
        new("Nguyễn Thiên Bảo", "51.01.104.129", "CNTT.A", "Thiết kế form bảng điểm thành tích, xây dựng logic game, bẫy lỗi try...catch, âm thanh"),
        new("Đỗ Chí An", "51.01.104.001", "CNTT.A", "Form hướng dẫn chơi và thông tin nhóm, xây dựng logic game, thiết kế giao diện"),
    };

}