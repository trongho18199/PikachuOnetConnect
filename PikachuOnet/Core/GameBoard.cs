using System.Drawing;

namespace PikachuOnet.Core;

public sealed class GameBoard
{
    public const int Empty = 0;

    private const int MaxDealAttempts = 500;
    private const int MaxShuffleAttempts = 200;

    private readonly int[,] _grid;
    private readonly Random _random;

    public GameBoard(int rows, int cols, Random random = null)
    {
        Rows = rows;
        Cols = cols;
        _grid = new int[rows + 2, cols + 2];
        _random = random ?? new Random();
    }

    public int Rows { get; }

    public int Cols { get; }

    public int GridHeight => Rows + 2;

    public int GridWidth => Cols + 2;

    public int RemainingCards { get; private set; }

    public int this[int row, int col] => _grid[row, col];

    public int GetAt(Point cell) => _grid[cell.Y, cell.X];

    public bool IsEmpty(Point cell) => _grid[cell.Y, cell.X] == Empty;

    public void Deal(int typeCount, int copiesPerType)
    {
        int capacity = Rows * Cols;
        int cardCount = typeCount * copiesPerType;
        if (typeCount < 1 || cardCount > capacity)
            throw new ArgumentOutOfRangeException(nameof(typeCount),
                $"Cần 1..{capacity / copiesPerType} loại thẻ, nhận được {typeCount}.");

        var deck = new List<int>(capacity);
        for (int type = 1; type <= typeCount; type++)
            for (int k = 0; k < copiesPerType; k++)
                deck.Add(type);
        while (deck.Count < capacity) deck.Add(Empty);

        RemainingCards = cardCount;
        Array.Clear(_grid);

        for (int attempt = 0; attempt < MaxDealAttempts; attempt++)
        {
            Shuffle(deck);
            int i = 0;
            for (int r = 1; r <= Rows; r++)
                for (int c = 1; c <= Cols; c++)
                    _grid[r, c] = deck[i++];

            if (HasMove()) break;
        }
    }

    public void Reshuffle()
    {
        var cells = new List<Point>();
        var types = new List<int>();
        for (int r = 1; r <= Rows; r++)
            for (int c = 1; c <= Cols; c++)
                if (_grid[r, c] != Empty)
                {
                    cells.Add(new Point(c, r));
                    types.Add(_grid[r, c]);
                }

        for (int attempt = 0; attempt < MaxShuffleAttempts; attempt++)
        {
            Shuffle(types);
            for (int i = 0; i < cells.Count; i++)
                _grid[cells[i].Y, cells[i].X] = types[i];

            if (HasMove()) return;
        }
    }

    public void RemovePair(Point a, Point b)
    {
        _grid[a.Y, a.X] = Empty;
        _grid[b.Y, b.X] = Empty;
        RemainingCards -= 2;
    }

    public bool HasMove() => PathFinder.TryFindMove(this, out _, out _);

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
