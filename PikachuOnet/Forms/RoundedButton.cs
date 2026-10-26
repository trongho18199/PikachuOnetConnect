using System.Drawing;
using System.Drawing.Drawing2D;

namespace PikachuOnet.Forms;

public class RoundedButton : Button
{
    private bool _hover;
    private bool _pressed;
    private int _cornerRadius = 14;

    public int CornerRadius
    {
        get => _cornerRadius;
        set { _cornerRadius = value; UpdateShape(); Invalidate(); }
    }

    public int BorderWidth { get; set; } = 3;
    public Color FillColor { get; set; } = Color.FromArgb(255, 213, 0);
    public Color BorderColor { get; set; } = Color.FromArgb(70, 45, 0);
    public Color DisabledFillColor { get; set; } = Color.FromArgb(215, 215, 215);
    public Color DisabledTextColor { get; set; } = Color.FromArgb(125, 125, 125);

    public Action<Graphics, Rectangle> IconPainter { get; set; }

    public RoundedButton()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        SetStyle(ControlStyles.Selectable, false);

        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        UseVisualStyleBackColor = false;
        ForeColor = Color.FromArgb(70, 45, 0);
        Cursor = Cursors.Hand;
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateShape();
    }

    private void UpdateShape()
    {
        if (Width <= 0 || Height <= 0) return;

        using var path = RoundedPath(new RectangleF(0, 0, Width, Height), _cornerRadius);
        var old = Region;
        Region = new Region(path);
        old?.Dispose();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

        g.Clear(BorderColor);

        Color fill = !Enabled ? DisabledFillColor
                   : _pressed ? Shade(FillColor, 0.85f)
                   : _hover ? Shade(FillColor, 1.12f)
                   : FillColor;

        var inner = new RectangleF(BorderWidth, BorderWidth, Width - BorderWidth * 2, Height - BorderWidth * 2);
        using (var path = RoundedPath(inner, Math.Max(_cornerRadius - BorderWidth, 1)))
        using (var brush = new SolidBrush(fill))
        {
            g.FillPath(brush, path);
        }

        if (IconPainter != null)
        {
            IconPainter(g, ClientRectangle);
            return;
        }

        using var textBrush = new SolidBrush(Enabled ? ForeColor : DisabledTextColor);
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap
        };
        g.DrawString(Text, Font, textBrush, new RectangleF(0, 0, Width, Height), format);
    }

    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }

    protected override void OnMouseLeave(EventArgs e) { _hover = false; _pressed = false; Invalidate(); base.OnMouseLeave(e); }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left) { _pressed = true; Invalidate(); }
        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        _pressed = false;
        Invalidate();
        base.OnMouseUp(e);
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        _hover = false;
        _pressed = false;
        Invalidate();
        base.OnEnabledChanged(e);
    }

    private static Color Shade(Color c, float factor) =>
        Color.FromArgb(c.A,
            Math.Clamp((int)(c.R * factor), 0, 255),
            Math.Clamp((int)(c.G * factor), 0, 255),
            Math.Clamp((int)(c.B * factor), 0, 255));

    private static GraphicsPath RoundedPath(RectangleF r, int radius)
    {
        float d = Math.Min(radius * 2f, Math.Min(r.Width, r.Height));
        var path = new GraphicsPath();
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}
