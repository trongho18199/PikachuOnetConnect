using System.Drawing;
using PikachuOnet.Config;
using PikachuOnet.Core;

namespace PikachuOnet.Rendering;

public static class BoardLayout
{
    private static int _originX;
    private static int _originY;

    public static int ClientWidth => (GameSettings.MaxCols + 2) * GameSettings.CellWidth;

    public static int ClientHeight => GameSettings.TopBarHeight + (GameSettings.MaxRows + 2) * GameSettings.CellHeight;

    public static void Configure(int rows, int cols)
    {
        _originX = (GameSettings.MaxCols - cols) * GameSettings.CellWidth / 2;
        _originY = (GameSettings.MaxRows - rows) * GameSettings.CellHeight / 2;
    }

    public static Rectangle CellBounds(int row, int col) =>
        new(_originX + col * GameSettings.CellWidth,
            GameSettings.TopBarHeight + _originY + row * GameSettings.CellHeight,
            GameSettings.CellWidth,
            GameSettings.CellHeight);

    public static PointF CellCenter(Point cell) =>
        new(_originX + cell.X * GameSettings.CellWidth + GameSettings.CellWidth / 2f,
            GameSettings.TopBarHeight + _originY + cell.Y * GameSettings.CellHeight + GameSettings.CellHeight / 2f);

    public static bool TryHitTest(int x, int y, GameBoard board, out Point cell)
    {
        cell = default;
        int localX = x - _originX;
        int localY = y - GameSettings.TopBarHeight - _originY;
        if (localX < 0 || localY < 0) return false;

        int col = localX / GameSettings.CellWidth;
        int row = localY / GameSettings.CellHeight;
        if (row < 1 || row > board.Rows || col < 1 || col > board.Cols) return false;
        if (board[row, col] == GameBoard.Empty) return false;

        cell = new Point(col, row);
        return true;
    }
}
