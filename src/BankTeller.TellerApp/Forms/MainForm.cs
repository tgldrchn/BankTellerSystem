using System.Drawing;
using System.Windows.Forms;
using BankTeller.Core.Interfaces;
using BankTeller.TellerApp.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace BankTeller.TellerApp.Forms;

/// <summary>
/// Теллерийн үндсэн форм.
/// Теллер энэ дэлгэцээр үйлчлүүлэгч дуудах, мөнгө шилжүүлэх,
/// валютын ханш өөрчлөх гэсэн 3 үндсэн үйлдлийг гүйцэтгэнэ.
/// </summary>
public class MainForm : Form
{
    // ── Services ────────────────────────────────────────────────
    private readonly IQueueService _queueService = new ApiQueueService();
    private readonly ITransactionService _transactionService = new ApiTransactionService();
    private readonly ICurrencyService _currencyService = new ApiCurrencyService();
    private HubConnection _hub = null!;

    // ── Controls ────────────────────────────────────────────────
    private Panel _pnlTitleBar = null!;
    private Panel _pnlNumberCard = null!;
    private Label _lblCurrentNumber = null!;
    private Label _lblWaiting = null!;
    private Label _lblStatus = null!;
    private Button _btnCallNext = null!;
    private Button _btnTransfer = null!;
    private Button _btnCurrency = null!;

    // ── Өнгөний тэмдэглэл (нэг палитр) ─────────────────────────
    private static readonly Color ClrBg = Color.FromArgb(24, 28, 24);
    private static readonly Color ClrCard = Color.FromArgb(30, 34, 30);
    private static readonly Color ClrBorder = Color.FromArgb(46, 52, 46);
    private static readonly Color ClrTitleBar = Color.FromArgb(35, 40, 35);
    private static readonly Color ClrGreen = Color.FromArgb(15, 110, 86);
    private static readonly Color ClrGreenHover = Color.FromArgb(10, 80, 65);
    private static readonly Color ClrGreenLight = Color.FromArgb(58, 170, 128);
    private static readonly Color ClrBeige = Color.FromArgb(212, 201, 162);
    private static readonly Color ClrBeigeLight = Color.FromArgb(237, 227, 198);
    private static readonly Color ClrMuted = Color.FromArgb(96, 112, 96);
    private static readonly Color ClrBtnTransfer = Color.FromArgb(18, 90, 65);
    private static readonly Color ClrBtnCurrency = Color.FromArgb(90, 72, 20);
    private static readonly Color ClrOk = Color.FromArgb(80, 200, 140);
    private static readonly Color ClrErr = Color.FromArgb(200, 80, 80);
    private static readonly Color ClrWarn = Color.FromArgb(210, 150, 50);
    // ────────────────────────────────────────────────────────────

    public MainForm()
    {
        InitializeComponents();
        _ = RefreshCurrentNumberAsync();
        _ = ConnectSignalRAsync();
    }

    private void InitializeComponents()
    {
        _pnlTitleBar = new Panel();
        _pnlNumberCard = new Panel();
        _lblCurrentNumber = new Label();
        _lblWaiting = new Label();
        _lblStatus = new Label();

        // ── Цонх ────────────────────────────────────────────────
        Text = "Теллерийн апп";
        Size = new Size(420, 560);
        MinimumSize = new Size(420, 520);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        BackColor = ClrBg;
        MaximizeBox = false;

        // ── TitleBar ─────────────────────────────────────────────
        _pnlTitleBar.Dock = DockStyle.Top;
        _pnlTitleBar.Height = 44;
        _pnlTitleBar.BackColor = ClrTitleBar;
        _pnlTitleBar.Paint += PaintTitleBar;

        var lblWinTitle = new Label
        {
            Text = "Теллерийн апп",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = ClrBeige,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(48, 0, 0, 0),
        };
        _pnlTitleBar.Controls.Add(lblWinTitle);

        // ── Хуваагч шугам ────────────────────────────────────────
        var divider = new Panel
        {
            BackColor = ClrBorder,
            Size = new Size(362, 1),
            Location = new Point(20, 52),
        };

        // ── Дугаарын карт ────────────────────────────────────────
        _pnlNumberCard.Location = new Point(20, 66);
        _pnlNumberCard.Size = new Size(362, 144);
        _pnlNumberCard.BackColor = ClrCard;
        _pnlNumberCard.Region = RoundedRegion(_pnlNumberCard.Size, 10);
        _pnlNumberCard.Paint += PaintCardBorder;

        var lblNumberTitle = new Label
        {
            Text = "ОДООГИЙН ДУГААР",
            Location = new Point(0, 14),
            Size = new Size(362, 20),
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            ForeColor = ClrMuted,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent,
        };

        _lblCurrentNumber.Text = "—";
        _lblCurrentNumber.Location = new Point(0, 34);
        _lblCurrentNumber.Size = new Size(362, 80);
        _lblCurrentNumber.Font = new Font("Courier New", 40, FontStyle.Bold);
        _lblCurrentNumber.ForeColor = ClrGreenLight;
        _lblCurrentNumber.BackColor = Color.Transparent;
        _lblCurrentNumber.TextAlign = ContentAlignment.MiddleCenter;

        _lblWaiting.Text = "Хүлээж буй: —";
        _lblWaiting.Location = new Point(0, 116);
        _lblWaiting.Size = new Size(362, 22);
        _lblWaiting.Font = new Font("Segoe UI", 9);
        _lblWaiting.ForeColor = ClrMuted;
        _lblWaiting.BackColor = Color.Transparent;
        _lblWaiting.TextAlign = ContentAlignment.MiddleCenter;

        _pnlNumberCard.Controls.AddRange(new Control[]
            { lblNumberTitle, _lblCurrentNumber, _lblWaiting });

        // ── Товчнууд ─────────────────────────────────────────────
        _btnCallNext = MakeButton(
            "👤  Дараагийн үйлчлүүлэгч дуудах",
            new Point(20, 228), ClrGreen, ClrGreenHover);
        _btnCallNext.Click += async (_, _) => await CallNextAsync();

        _btnTransfer = MakeButton(
            "⇄  Мөнгө шилжүүлэх",
            new Point(20, 294),
            ClrBtnTransfer,
            Color.FromArgb(12, 65, 46));
        _btnTransfer.Click += (_, _) =>
            new TransactionForm(_transactionService).ShowDialog();

        _btnCurrency = MakeButton(
            "$  Валютын ханш өөрчлөх",
            new Point(20, 360),
            ClrBtnCurrency,
            Color.FromArgb(65, 50, 12));
        _btnCurrency.Click += (_, _) =>
            new CurrencyForm(_currencyService).ShowDialog();

        // ── Статус ───────────────────────────────────────────────
        _lblStatus.Text = "Ханш өөрчлөх цонх нээгдэж байна...";
        _lblStatus.Location = new Point(20, 430);
        _lblStatus.Size = new Size(362, 20);
        _lblStatus.Font = new Font("Segoe UI", 8);
        _lblStatus.ForeColor = ClrMuted;
        _lblStatus.TextAlign = ContentAlignment.MiddleCenter;
        _lblStatus.BackColor = Color.Transparent;

        Controls.AddRange(new Control[]
        {
            _pnlTitleBar, divider,
            _pnlNumberCard,
            _btnCallNext, _btnTransfer, _btnCurrency,
            _lblStatus,
        });
    }

    // ── Товч үүсгэх ──────────────────────────────────────────────
    private static Button MakeButton(string text, Point loc, Color color, Color hover)
    {
        var btn = new Button
        {
            Text = text,
            Location = loc,
            Size = new Size(362, 52),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            BackColor = color,
            ForeColor = Color.FromArgb(237, 227, 198),   // beige текст
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 0, 0, 0),
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.Region = RoundedRegion(btn.Size, 8);
        btn.MouseEnter += (_, _) => btn.BackColor = hover;
        btn.MouseLeave += (_, _) => btn.BackColor = color;
        return btn;
    }

    // ── SignalR ──────────────────────────────────────────────────
    private async Task ConnectSignalRAsync()
    {
        _hub = new HubConnectionBuilder()
            .WithUrl("http://localhost:5200/hubs/bank")
            .WithAutomaticReconnect()
            .Build();

        _hub.On<int>("NumberCalled", number =>
        {
            Invoke(() =>
            {
                _lblCurrentNumber.Text = number.ToString("D3");
                SetStatus($"✓  Дугаар {number} дуудагдлаа  •  {DateTime.Now:HH:mm:ss}", ClrOk);
            });
        });

        _hub.On("QueueUpdated", async () =>
        {
            var count = await _queueService.GetWaitingCountAsync();
            Invoke(() => _lblWaiting.Text = $"Хүлээж буй: {count} үйлчлүүлэгч");
        });

        try
        {
            await _hub.StartAsync();
            SetStatus("SignalR: ханш шинэчлэгдлээ", ClrOk);
        }
        catch
        {
            SetStatus("SignalR холбогдож чадсангүй", ClrErr);
        }
    }

    // ── Логик ────────────────────────────────────────────────────
    private async Task CallNextAsync()
    {
        SetBusy(true, "Дуудаж байна...");
        try
        {
            var ticket = await _queueService.CallNextAsync();
            if (ticket == null)
            {
                MessageBox.Show(
                    "Хүлээж буй үйлчлүүлэгч байхгүй байна.",
                    "Мэдэгдэл",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                SetStatus("Дараалал хоосон байна", ClrMuted);
                return;
            }
            _lblCurrentNumber.Text = ticket.Number.ToString("D3");
            SetStatus($"✓  Дугаар {ticket.Number} дуудагдлаа  •  {DateTime.Now:HH:mm:ss}", ClrOk);
            await RefreshWaitingCountAsync();
        }
        catch (Exception ex)
        {
            SetStatus($"Алдаа: {ex.Message}", ClrErr);
        }
        finally
        {
            SetBusy(false, "");
        }
    }

    private async Task RefreshCurrentNumberAsync()
    {
        try
        {
            var number = await _queueService.GetCurrentNumberAsync();
            _lblCurrentNumber.Text = number == 0 ? "—" : number.ToString("D3");
            await RefreshWaitingCountAsync();
        }
        catch
        {
            SetStatus("Серверт холбогдож чадсангүй", ClrErr);
        }
    }

    private async Task RefreshWaitingCountAsync()
    {
        try
        {
            var count = await _queueService.GetWaitingCountAsync();
            Invoke(() => _lblWaiting.Text = $"Хүлээж буй: {count} үйлчлүүлэгч");
        }
        catch
        {
            Invoke(() => _lblWaiting.Text = "Хүлээж буй: —");
        }
    }

    // ── Туслах методууд ──────────────────────────────────────────
    private void SetBusy(bool busy, string msg)
    {
        _btnCallNext.Enabled = !busy;
        _btnTransfer.Enabled = !busy;
        _btnCurrency.Enabled = !busy;
        if (!string.IsNullOrEmpty(msg))
            SetStatus(msg, ClrWarn);
    }

    private void SetStatus(string msg, Color color)
    {
        if (InvokeRequired) { Invoke(() => SetStatus(msg, color)); return; }
        _lblStatus.Text = msg;
        _lblStatus.ForeColor = color;
    }

    // ── Paint туслахууд ──────────────────────────────────────────
    private void PaintTitleBar(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        DrawDot(g, new Point(14, 16), Color.FromArgb(224, 85, 85));
        DrawDot(g, new Point(30, 16), Color.FromArgb(212, 160, 32));
        DrawDot(g, new Point(46, 16), Color.FromArgb(90, 170, 90));
    }

    private void PaintCardBorder(object? sender, PaintEventArgs e)
    {
        using var pen = new Pen(ClrBorder, 1f);
        var r = new Rectangle(0, 0, _pnlNumberCard.Width - 1, _pnlNumberCard.Height - 1);
        e.Graphics.DrawRectangle(pen, r);
    }

    private static void DrawDot(System.Drawing.Graphics g, Point c, Color color)
    {
        using var b = new SolidBrush(color);
        g.FillEllipse(b, c.X - 5, c.Y - 5, 11, 11);
    }

    private static System.Drawing.Region RoundedRegion(Size size, int r)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        path.AddArc(0, 0, r * 2, r * 2, 180, 90);
        path.AddArc(size.Width - r * 2, 0, r * 2, r * 2, 270, 90);
        path.AddArc(size.Width - r * 2, size.Height - r * 2, r * 2, r * 2, 0, 90);
        path.AddArc(0, size.Height - r * 2, r * 2, r * 2, 90, 90);
        path.CloseFigure();
        return new System.Drawing.Region(path);
    }

    protected override async void OnFormClosing(FormClosingEventArgs e)
    {
        if (_hub != null) await _hub.DisposeAsync();
        base.OnFormClosing(e);
    }
}