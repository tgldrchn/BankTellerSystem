using System.Drawing;
using System.Windows.Forms;
using BankTeller.NumberDisplay.Services;

namespace BankTeller.NumberDisplay.Forms;

/// <summary>
/// Дугаар харуулах дэлгэц.
/// Socket-оор холбогдож теллер дугаар дуудахад харуулна.
/// </summary>
public class DisplayForm : Form
{
    private readonly SocketClient _socketClient = new();
    private readonly CancellationTokenSource _cts = new();

    private Label _lblNumber = null!;
    private Label _lblStatus = null!;
    private Label _lblSubtitle = null!;
    private Panel _pnlTitleBar = null!;

    // ── Өнгөний тэмдэглэл (TerminalForm-тай нэг палитр) ────────────
    private static readonly Color ClrBg = Color.FromArgb(24, 28, 24);
    private static readonly Color ClrCard = Color.FromArgb(19, 51, 40);   // дугаарын хэсгийн фон
    private static readonly Color ClrBorder = Color.FromArgb(46, 52, 46);
    private static readonly Color ClrTitleBar = Color.FromArgb(35, 40, 35);
    private static readonly Color ClrGreenLight = Color.FromArgb(58, 232, 160);   // том дугаар
    private static readonly Color ClrBeige = Color.FromArgb(212, 201, 162);
    private static readonly Color ClrMuted = Color.FromArgb(96, 112, 96);
    private static readonly Color ClrConnected = Color.FromArgb(90, 212, 140);
    private static readonly Color ClrDisconnected = Color.FromArgb(200, 80, 80);
    // ────────────────────────────────────────────────────────────────

    public DisplayForm()
    {
        InitializeComponents();
        _ = StartSocketAsync();
    }

    private void InitializeComponents()
    {
        _lblNumber = new Label();
        _lblStatus = new Label();
        _lblSubtitle = new Label();
        _pnlTitleBar = new Panel();

        // ── Үндсэн цонх ─────────────────────────────────────────────
        Text = "Дугаарын дэлгэц";
        Size = new Size(600, 440);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = ClrBg;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        // ── TitleBar (гурван дугуй) ──────────────────────────────────
        _pnlTitleBar.Dock = DockStyle.Top;
        _pnlTitleBar.Height = 44;
        _pnlTitleBar.BackColor = ClrTitleBar;
        _pnlTitleBar.Paint += PaintTitleBar;

        var lblWinTitle = new Label
        {
            Text = "Дугаарын дэлгэц",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = ClrBeige,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(48, 0, 0, 0),
        };
        _pnlTitleBar.Controls.Add(lblWinTitle);

        // ── Хуваагч шугам ────────────────────────────────────────────
        var divider = new Panel
        {
            BackColor = ClrBorder,
            Size = new Size(540, 1),
            Location = new Point(30, 52),
        };

        // ── "ДУУДАГДСАН ДУГААР" тайлбар ─────────────────────────────
        var lblTitle = new Label
        {
            Text = "ДУУДАГДСАН ДУГААР",
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = ClrMuted,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(600, 28),
            Location = new Point(0, 68),
        };

        // ── Дугаар харуулах хэсэг (өөрийн фонтой хайрцаг) ──────────
        var pnlDisplay = new Panel
        {
            BackColor = ClrCard,
            Size = new Size(480, 200),
            Location = new Point(60, 102),
            BorderStyle = BorderStyle.None,
        };
        pnlDisplay.Region = RoundedRegion(pnlDisplay.Size, 12);

        _lblNumber.Text = "—";
        _lblNumber.Font = new Font("Courier New", 110, FontStyle.Bold);
        _lblNumber.ForeColor = ClrGreenLight;
        _lblNumber.BackColor = Color.Transparent;
        _lblNumber.TextAlign = ContentAlignment.MiddleCenter;
        _lblNumber.Dock = DockStyle.Fill;
        pnlDisplay.Controls.Add(_lblNumber);

        // ── "1-р теллер рүү ирнэ үү" дэд гарчиг ────────────────────
        _lblSubtitle.Text = "—";
        _lblSubtitle.Font = new Font("Segoe UI", 11, FontStyle.Regular);
        _lblSubtitle.ForeColor = ClrMuted;
        _lblSubtitle.BackColor = Color.Transparent;
        _lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
        _lblSubtitle.Size = new Size(600, 28);
        _lblSubtitle.Location = new Point(0, 314);

        // ── Socket статус ─────────────────────────────────────────────
        _lblStatus.Text = "Холбогдож байна...";
        _lblStatus.Font = new Font("Segoe UI", 9, FontStyle.Regular);
        _lblStatus.ForeColor = ClrMuted;
        _lblStatus.BackColor = Color.Transparent;
        _lblStatus.TextAlign = ContentAlignment.MiddleCenter;
        _lblStatus.Size = new Size(600, 24);
        _lblStatus.Location = new Point(0, 358);

        // ── Socket event-үүд ─────────────────────────────────────────
        _socketClient.NumberReceived += number =>
        {
            Invoke(() =>
            {
                _lblNumber.Text = number.ToString("D3");
                _lblSubtitle.Text = "1-р теллер рүү ирнэ үү";
                _lblSubtitle.ForeColor = ClrBeige;
                _lblStatus.Text = $"Socket: холбогдсон  •  {DateTime.Now:HH:mm:ss}";
                _lblStatus.ForeColor = ClrConnected;
            });
        };

        _socketClient.Disconnected += () =>
        {
            Invoke(() =>
            {
                _lblSubtitle.Text = "—";
                _lblStatus.Text = "Socket: холболт тасарлаа. Дахин холбогдож байна...";
                _lblStatus.ForeColor = ClrDisconnected;
                _lblSubtitle.ForeColor = ClrMuted;
            });
        };

        Controls.AddRange(new Control[]
        {
            _pnlTitleBar,
            divider,
            lblTitle,
            pnlDisplay,
            _lblSubtitle,
            _lblStatus,
        });
    }

    private async Task StartSocketAsync()
    {
        _lblStatus.Text = "Серверт холбогдож байна...";
        _lblStatus.ForeColor = ClrMuted;
        await _socketClient.ConnectAsync(_cts.Token);
    }

    // ── Гурван дугуй ─────────────────────────────────────────────────
    private void PaintTitleBar(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        DrawDot(g, new Point(14, 16), Color.FromArgb(224, 85, 85));
        DrawDot(g, new Point(30, 16), Color.FromArgb(212, 160, 32));
        DrawDot(g, new Point(46, 16), Color.FromArgb(90, 170, 90));
    }

    private static void DrawDot(System.Drawing.Graphics g, Point center, Color color)
    {
        using var b = new SolidBrush(color);
        g.FillEllipse(b, center.X - 5, center.Y - 5, 11, 11);
    }

    private static System.Drawing.Region RoundedRegion(Size size, int radius)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
        path.AddArc(size.Width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
        path.AddArc(size.Width - radius * 2, size.Height - radius * 2, radius * 2, radius * 2, 0, 90);
        path.AddArc(0, size.Height - radius * 2, radius * 2, radius * 2, 90, 90);
        path.CloseFigure();
        return new System.Drawing.Region(path);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _cts.Cancel();
        _socketClient.Dispose();
        base.OnFormClosing(e);
    }
}