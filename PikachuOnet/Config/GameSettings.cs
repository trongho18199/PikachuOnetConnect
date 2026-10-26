namespace PikachuOnet.Config;

public static class GameSettings
{
    public const int MaxRows = 8;
    public const int MaxCols = 18;
    public const int CopiesPerType = 4;
    public const int MaxTypes = MaxRows * MaxCols / CopiesPerType;

    public const int CellWidth = 50;
    public const int CellHeight = 50;
    public const int TopBarHeight = 60;
    public const bool StretchImagesToCell = true;

    public const int PointsPerPair = 10;
    public const int TimeBonusPerSecond = 2;

    public const int LineDisplayMs = 300;
    public const int HintDisplayMs = 1200;

    public const string ImageFolder = "Images";
    public const string BackgroundSubfolder = "Background";
    public const string AudioFolder = "Audio";

    public const string DataFolder = "Data";
    public const string ScoreFileName = "thanhtichs.txt";
    public const int LeaderboardSize = 10;
    public const int MaxPlayerNameLength = 20;
}
