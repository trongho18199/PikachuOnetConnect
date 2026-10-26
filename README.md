# Pikachu Onet (WinForms)

Game nối hình kiểu Pikachu Onet viết bằng C# WinForms (.NET 8).

## Yêu cầu
- Windows
- .NET 8 SDK
- Visual Studio 2022 (workload *.NET desktop development*) hoặc dùng dòng lệnh

## Chạy game
1. Bỏ ảnh thẻ của bạn vào `PikachuOnet/Images/` (png, jpg, jpeg, bmp, gif; tên file tùy ý). Bộ 33 ảnh có sẵn nằm trong `PikachuOnet/Images.zip`, giải nén vào thư mục dự án `PikachuOnet/` để có `Images/1.png`, `Images/2.png`...
2. Mở `PikachuOnet.sln` bằng Visual Studio rồi nhấn F5, hoặc chạy:

```
dotnet run --project PikachuOnet
```

Mỗi ảnh là một loại thẻ, mỗi loại có 4 thẻ. Bàn lớn nhất 8x18 chứa tối đa 36 loại.
Nếu không có ít nhất 2 ảnh hợp lệ, game hiển thị số thứ tự thay cho ảnh.

## Luật chơi
- Click hai thẻ giống nhau. Nếu nối được bằng đường có tối đa 2 lần rẽ (đi qua ô trống, được vòng ra ngoài viền bàn) thì hai thẻ biến mất.
- Mỗi cặp +10 điểm, thưởng thêm theo thời gian còn lại khi thắng.
- Nút **Gợi ý** sáng viền xanh một cặp nối được. Nút **Xáo trộn** có 5 lần ở level 1, càng lên level càng ít (tối thiểu 1 lần). Hết nước đi thì game tự xáo.

## Chức năng bổ sung (theo yêu cầu giữa kỳ)

- **Bảng xếp hạng** (nút *Xếp hạng* ở màn hình chính): khi ván kết thúc (hết giờ, hoặc bấm *Chơi mới* khi đã có điểm) game hỏi tên rồi lưu `Tên,Điểm,mm:ss` vào `Data/thanhtichs.txt` bằng `System.IO`. Sắp xếp điểm giảm dần, bằng điểm thì thời gian ngắn hơn xếp trên. Dòng sai định dạng bị bỏ qua, chưa có file thì bảng rỗng.
- **Âm thanh**: nhạc nền và hiệu ứng (chọn thẻ, nối đúng, nối sai, qua level, thua) trong `Audio/`. Nút hình loa bật/tắt. Thiếu file thì game báo một lần rồi chơi tiếp không âm thanh.
- **Hướng dẫn & Thông tin nhóm** (nút *Hướng dẫn*): luật chơi và thông tin nhóm.
- **Bẫy lỗi**: thiếu ảnh thẻ/ảnh nền/âm thanh, lỗi đọc-ghi file thành tích, lỗi không lường trước (`Program.cs`) đều hiện thông báo thay vì làm game văng.

### Điền thông tin nhóm
Mở `PikachuOnet/Config/TeamInfo.cs` để sửa tên nhóm, giảng viên, họ tên, MSSV, lớp, vai trò. Thêm/bớt thành viên bằng cách thêm/xoá một dòng `new(...)`.

### Âm thanh
Các file `.wav` trong `Audio/`. Có thể thay bằng file của bạn nhưng giữ nguyên tên: `bgm`, `select`, `correct`, `wrong`, `win`, `lose`.

### Vị trí file thành tích
Khi chạy từ Visual Studio, file được ghi tại `bin/Debug/net8.0-windows/Data/thanhtichs.txt` (cạnh file .exe), không phải file trong thư mục source. File trong source chỉ là file mẫu (một dòng ghi chú và vài dòng điểm ví dụ).

## Cấu trúc thư mục

```
PikachuOnet.sln
PikachuOnet/
├── Program.cs                  Điểm khởi chạy
├── Config/
│   ├── GameSettings.cs         Hằng số: kích thước bàn, thời gian, điểm...
│   └── TeamInfo.cs             Thông tin nhóm (TỰ ĐIỀN Ở ĐÂY)
├── Core/                       Logic game, không phụ thuộc giao diện
│   ├── GameBoard.cs            Lưới thẻ: chia bài, xáo, xoá cặp
│   ├── PathFinder.cs           Thuật toán nối tối đa 2 lần rẽ
│   └── LevelDesigner.cs        Bàn, số loại thẻ, thời gian, lượt xáo theo level
├── Services/
│   ├── CardImageLoader.cs      Đọc ảnh từ thư mục Images
│   ├── BackgroundImageLoader.cs Đọc ảnh nền
│   ├── CardSet.cs              Tên và ảnh của các loại thẻ
│   ├── ScoreService.cs         Đọc/ghi bảng xếp hạng (System.IO)
│   └── SoundManager.cs         Nhạc nền và hiệu ứng âm thanh
├── Rendering/                  Vẽ bàn chơi bằng GDI+
│   ├── BoardRenderer.cs        Vẽ ảnh thẻ, viền chọn/gợi ý, đường nối
│   ├── BoardLayout.cs          Đổi qua lại giữa ô (hàng, cột) và pixel
│   ├── BoardViewState.cs       Thẻ đang chọn, gợi ý, đường nối đang hiện
│   └── CardPalette.cs          Màu nền thẻ theo loại
├── Forms/
│   ├── GameForm.cs             Luật chơi, timer, xử lý chuột
│   ├── GameForm.Ui.cs          Dựng giao diện và cập nhật HUD
│   ├── GameForm.Screens.cs     Màn hình chính / tạm dừng
│   ├── GameForm.Extras.cs      Lưu điểm, bảng xếp hạng, hướng dẫn, bật/tắt âm thanh
│   ├── NameInputForm.cs        Hộp thoại nhập tên
│   ├── LeaderboardForm.cs      Bảng xếp hạng
│   ├── HelpForm.cs             Hướng dẫn và thông tin nhóm
│   └── RoundedButton.cs        Nút bo tròn tự vẽ
├── Images/                     Ảnh thẻ (copy sang thư mục build)
├── Images.zip                  Bộ 33 ảnh thẻ có sẵn
├── Audio/                      File .wav (copy sang thư mục build)
└── Data/                       File thành tích mẫu (copy sang thư mục build)
```

## Tùy chỉnh nhanh
Mở `Config/GameSettings.cs` để đổi kích thước bàn tối đa (`MaxRows`/`MaxCols`), điểm số. Thời gian, kích thước bàn và số lần xáo của từng level chỉnh trong `Core/LevelDesigner.cs`.
Số loại thẻ tối đa tự tính lại theo `MaxRows * MaxCols / CopiesPerType`.