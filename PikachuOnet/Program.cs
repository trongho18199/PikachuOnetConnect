using PikachuOnet.Forms;

namespace PikachuOnet;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => ShowUnexpectedError(e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, e) => ShowUnexpectedError(e.ExceptionObject as Exception);

        ApplicationConfiguration.Initialize();

        try
        {
            Application.Run(new GameForm());
        }
        catch (Exception ex)
        {
            ShowUnexpectedError(ex);
        }
    }

    private static void ShowUnexpectedError(Exception ex)
    {
        MessageBox.Show(
            "Đã xảy ra lỗi không mong muốn:\n\n" + (ex?.Message ?? "Không rõ nguyên nhân") +
            "\n\nBạn có thể thử thao tác lại; nếu lỗi lặp lại hãy khởi động lại game.",
            "Pikachu Onet", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
