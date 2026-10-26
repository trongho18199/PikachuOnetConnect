using System.Drawing;

namespace PikachuOnet.Rendering;

public sealed class BoardViewState
{
    public Point? Selected { get; set; }
    public Point? HintA { get; set; }
    public Point? HintB { get; set; }
    public IReadOnlyList<Point> Connection { get; set; }

    public bool IsHint(Point cell) => HintA == cell || HintB == cell;

    public void ClearHint() => HintA = HintB = null;

    public void Reset()
    {
        Selected = null;
        ClearHint();
        Connection = null;
    }
}
