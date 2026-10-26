using System.Drawing;
using System.Drawing.Drawing2D;
using PikachuOnet.Config;
using PikachuOnet.Core;
using PikachuOnet.Services;

namespace PikachuOnet.Rendering;

public sealed class BoardRenderer : IDisposable
{
    private readonly Font _labelFont = new("Segoe UI", 8, FontStyle.Bold);
    private readonly StringFormat _centered = new()
    {
        Alignment = StringAlignment.Center,
        LineAlignment = StringAlignment.Center
    };

    public void Draw(Graphics g, GameBoard board, CardSet cards, Image background, BoardViewState view)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.Half;

        DrawBackground(g, background);

        for (int row = 1; row <= board.Rows; row++)
            for (int col = 1; col <= board.Cols; col++)
            {
                int type = board[row, col];
                if (type == GameBoard.Empty) continue;
                DrawCard(g, cards, type, new Point(col, row), view);
            }

        DrawConnection(g, view.Connection);
    }

    public static void DrawBackground(Graphics g, Image background)
    {
        if (background == null) return;

        int w = BoardLayout.ClientWidth;
        int h = BoardLayout.ClientHeight;
        float scale = Math.Max((float)w / background.Width, (float)h / background.Height);
        float srcW = w / scale;
        float srcH = h / scale;
        var source = new RectangleF((background.Width - srcW) / 2f, (background.Height - srcH) / 2f, srcW, srcH);
        g.DrawImage(background, new RectangleF(0, 0, w, h), source, GraphicsUnit.Pixel);
    }

    private void DrawCard(Graphics g, CardSet cards, int type, Point cell, BoardViewState view)
    {
        var bounds = BoardLayout.CellBounds(cell.Y, cell.X);

        var image = cards.GetImage(type);
        if (image != null)
        {
            DrawCardImage(g, image, bounds);
        }
        else
        {
            g.FillRectangle(Brushes.White, bounds);
            g.DrawRectangle(Pens.LightGray, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            g.DrawString(cards.GetName(type), _labelFont, Brushes.Black, bounds, _centered);
        }

        bool selected = view.Selected == cell;
        bool hinted = view.IsHint(cell);
        if (selected || hinted)
        {
            using var border = new Pen(selected ? Color.Red : Color.LimeGreen, 3);
            var inner = Rectangle.Inflate(bounds, -2, -2);
            g.DrawRectangle(border, inner);
        }
    }

    private static void DrawCardImage(Graphics g, Image image, Rectangle box)
    {
        if (GameSettings.StretchImagesToCell)
        {
            g.DrawImage(image, box);
            return;
        }

        float scale = Math.Min((float)box.Width / image.Width, (float)box.Height / image.Height);
        float w = image.Width * scale;
        float h = image.Height * scale;
        g.DrawImage(image, box.X + (box.Width - w) / 2, box.Y + (box.Height - h) / 2, w, h);
    }

    private static void DrawConnection(Graphics g, IReadOnlyList<Point> connection)
    {
        if (connection == null || connection.Count < 2) return;

        var points = connection.Select(BoardLayout.CellCenter).ToArray();
        using var pen = new Pen(Color.Red, 4) { LineJoin = LineJoin.Round };
        g.DrawLines(pen, points);
    }

    public void Dispose()
    {
        _labelFont.Dispose();
        _centered.Dispose();
    }
}
