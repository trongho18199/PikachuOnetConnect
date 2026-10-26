using System.Drawing;

namespace PikachuOnet.Core;

public static class PathFinder
{
    public static List<Point> FindPath(GameBoard board, Point a, Point b)
    {
        if (a == b || board.GetAt(a) != board.GetAt(b)) return null;

        if ((a.X == b.X || a.Y == b.Y) && IsClear(board, a, b))
            return new List<Point> { a, b };

        foreach (var corner in new[] { new Point(b.X, a.Y), new Point(a.X, b.Y) })
            if (board.IsEmpty(corner) && IsClear(board, a, corner) && IsClear(board, corner, b))
                return new List<Point> { a, corner, b };

        for (int c = 0; c < board.GridWidth; c++)
        {
            var p = new Point(c, a.Y);
            var q = new Point(c, b.Y);
            if (p == a || q == b) continue;
            if (board.IsEmpty(p) && board.IsEmpty(q)
                && IsClear(board, a, p) && IsClear(board, p, q) && IsClear(board, q, b))
                return new List<Point> { a, p, q, b };
        }

        for (int r = 0; r < board.GridHeight; r++)
        {
            var p = new Point(a.X, r);
            var q = new Point(b.X, r);
            if (p == a || q == b) continue;
            if (board.IsEmpty(p) && board.IsEmpty(q)
                && IsClear(board, a, p) && IsClear(board, p, q) && IsClear(board, q, b))
                return new List<Point> { a, p, q, b };
        }

        return null;
    }

    public static bool TryFindMove(GameBoard board, out Point a, out Point b)
    {
        var cells = new List<Point>();
        for (int r = 1; r <= board.Rows; r++)
            for (int c = 1; c <= board.Cols; c++)
                if (board[r, c] != GameBoard.Empty)
                    cells.Add(new Point(c, r));

        for (int i = 0; i < cells.Count; i++)
            for (int j = i + 1; j < cells.Count; j++)
                if (board.GetAt(cells[i]) == board.GetAt(cells[j])
                    && FindPath(board, cells[i], cells[j]) != null)
                {
                    a = cells[i];
                    b = cells[j];
                    return true;
                }

        a = b = default;
        return false;
    }

    private static bool IsClear(GameBoard board, Point a, Point b)
    {
        if (a.X == b.X)
        {
            for (int r = Math.Min(a.Y, b.Y) + 1; r < Math.Max(a.Y, b.Y); r++)
                if (board[r, a.X] != GameBoard.Empty) return false;
            return true;
        }

        if (a.Y == b.Y)
        {
            for (int c = Math.Min(a.X, b.X) + 1; c < Math.Max(a.X, b.X); c++)
                if (board[a.Y, c] != GameBoard.Empty) return false;
            return true;
        }

        return false;
    }
}
