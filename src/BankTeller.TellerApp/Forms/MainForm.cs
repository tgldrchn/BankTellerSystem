using BankTeller.Core.Interfaces;
using BankTeller.TellerApp.Services;

namespace BankTeller.TellerApp.Forms;

/// <summary>
/// Теллерийн үндсэн форм.
/// Теллер энэ дэлгэцээр үйлчлүүлэгч дуудах, мөнгө шилжүүлэх,
/// валютын ханш өөрчлөх гэсэн 3 үндсэн үйлдлийг гүйцэтгэнэ.
/// </summary>
public class MainForm : Form
{
    // ── Services ────────────────────────────────────────────────
    /// <summary>Дугаарын дараалал удирдах сервис.</summary>
    private readonly IQueueService _queueService = new MockQueueService();

    /// <summary>Мөнгөн гүйлгээ гүйцэтгэх сервис.</summary>
    private readonly ITransactionService _transactionService = new MockTransactionService();

    /// <summary>Валютын ханш унших/бичих сервис.</summary>
    private readonly ICurrencyService _currencyService = new MockCurrencyService();

    // ── Controls ────────────────────────────────────────────────
    private Panel _headerPanel = null!;
    private Panel _numberPanel = null!;
    private Label _lblCurrentNumber = null!;
    private Label _lblWaiting = null!;
    private Label _lblStatus = null!;
    private Button _btnCallNext = null!;
    private Button _btnTransfer = null!;
    private Button _btnCurrency = null!;

    // ── Constructor ─────────────────────────────────────────────

    /// <summary>
    /// MainForm-ийг эхлүүлж UI байгуулна.
    /// Mock сервисүүд шууд тохируулагдана; API бэлэн болсон үед
    /// Program.cs-д DI-ээр солино.
    /// </summary>
    public MainForm()
    {
        InitializeComponents();
        _ = RefreshCurrentNumberAsync();
    }

    // ── UI Setup ────────────────────────────────────────────────

    /// <summary>
    /// Формын бүх UI элементүүдийг үүсгэж байрлуулна.
    /// </summary>
    private void InitializeComponents()
    {
        Text = "Теллерийн апп";
        Size = new Size(420, 600);
        MinimumSize = new Size(420, 520);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        BackColor = Color.FromArgb(245, 247, 250);
        MaximizeBox = false;

        // ── Header ──────────────────────────────────────────────
        _headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = Color.FromArgb(26, 60, 110)
        };

        var lblTitle = new Label
        {
            Text = "🏦  Теллерийн апп",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 0, 0, 0)
        };
        _headerPanel.Controls.Add(lblTitle);

        // ── Number display panel ─────────────────────────────────
        _numberPanel = new Panel
        {
            Location = new Point(20, 80),
            Size = new Size(362, 140),
            BackColor = Color.White
        };
        _numberPanel.Paint += (s, e) =>
        {
            var rect = new Rectangle(0, 0, _numberPanel.Width - 1, _numberPanel.Height - 1);
            using var pen = new Pen(Color.FromArgb(220, 225, 235), 1);
            e.Graphics.DrawRectangle(pen, rect);
        };

        var lblNumberTitle = new Label
        {
            Text = "ОДООГИЙН ДУГААР",
            Location = new Point(0, 16),
            Size = new Size(362, 20),
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            ForeColor = Color.Gray,
            TextAlign = ContentAlignment.MiddleCenter
        };

        _lblCurrentNumber = new Label
        {
            Text = "—",
            Location = new Point(0, 36),
            Size = new Size(362, 72),
            Font = new Font("Segoe UI", 52, FontStyle.Bold),
            ForeColor = Color.FromArgb(26, 60, 110),
            TextAlign = ContentAlignment.MiddleCenter
        };

        _lblWaiting = new Label
        {
            Text = "Хүлээж буй: —",
            Location = new Point(0, 110),
            Size = new Size(362, 22),
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.Gray,
            TextAlign = ContentAlignment.MiddleCenter
        };

        _numberPanel.Controls.AddRange(new Control[]
            { lblNumberTitle, _lblCurrentNumber, _lblWaiting });

        // ── Buttons ─────────────────────────────────────────────
        _btnCallNext = CreateButton(
            "👤  Дараагийн үйлчлүүлэгч дуудах",
            new Point(20, 240),
            Color.FromArgb(26, 60, 110));
        _btnCallNext.Click += async (_, _) => await CallNextAsync();

        _btnTransfer = CreateButton(
            "💸  Мөнгө шилжүүлэх",
            new Point(20, 310),
            Color.FromArgb(22, 120, 80));
        _btnTransfer.Click += (_, _) =>
            new TransactionForm(_transactionService).ShowDialog();

        _btnCurrency = CreateButton(
            "💱  Валютын ханш өөрчлөх",
            new Point(20, 380),
            Color.FromArgb(180, 90, 20));
        _btnCurrency.Click += (_, _) =>
            new CurrencyForm(_currencyService).ShowDialog();

        // ── Status label ────────────────────────────────────────
        _lblStatus = new Label
        {
            Text = "Бэлэн",
            Location = new Point(20, 448),
            Size = new Size(362, 22),
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.Gray,
            TextAlign = ContentAlignment.MiddleCenter
        };

        // ── Assemble ────────────────────────────────────────────
        Controls.AddRange(new Control[]
        {
            _headerPanel, _numberPanel,
            _btnCallNext, _btnTransfer, _btnCurrency,
            _lblStatus
        });
    }

    /// <summary>
    /// Нэгдсэн загварын товч үүсгэх туслах метод.
    /// </summary>
    /// <param name="text">Товчны текст.</param>
    /// <param name="location">Байрлал.</param>
    /// <param name="color">Арын өнгө.</param>
    private static Button CreateButton(string text, Point location, Color color)
    {
        var btn = new Button
        {
            Text = text,
            Location = location,
            Size = new Size(362, 56),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(12, 0, 0, 0)
        };
        btn.FlatAppearance.BorderSize = 0;

        // Hover effect
        btn.MouseEnter += (_, _) =>
            btn.BackColor = ControlPaint.Dark(color, 0.1f);
        btn.MouseLeave += (_, _) =>
            btn.BackColor = color;

        return btn;
    }

    // ── Logic ────────────────────────────────────────────────────

    /// <summary>
    /// Дараагийн хүлээж буй үйлчлүүлэгчийг дуудна.
    /// Дугаар дэлгэцэнд шинэчлэгдэж, Socket сервер дамжуулан
    /// дугаарын дэлгэцэнд харуулах захиалга явуулна.
    /// Хэрэв дараалал хоосон бол мэдэгдэл харуулна.
    /// </summary>
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
                SetStatus("Дараалал хоосон байна", Color.Gray);
                return;
            }

            _lblCurrentNumber.Text = ticket.Number.ToString();
            SetStatus($"✓  Дугаар {ticket.Number} дуудагдлаа  •  {DateTime.Now:HH:mm:ss}",
                Color.SeaGreen);

            await RefreshWaitingCountAsync();
        }
        catch (Exception ex)
        {
            SetStatus($"Алдаа: {ex.Message}", Color.Crimson);
        }
        finally
        {
            SetBusy(false, "");
        }
    }

    /// <summary>
    /// Апп нээгдэхэд одоогийн дугаарыг серверээс авч харуулна.
    /// </summary>
    private async Task RefreshCurrentNumberAsync()
    {
        try
        {
            var number = await _queueService.GetCurrentNumberAsync();
            _lblCurrentNumber.Text = number == 0 ? "—" : number.ToString();
            await RefreshWaitingCountAsync();
        }
        catch
        {
            SetStatus("Серверт холбогдож чадсангүй", Color.Crimson);
        }
    }

    /// <summary>
    /// Хүлээж буй үйлчлүүлэгчдийн тоог шинэчилж харуулна.
    /// </summary>
    private async Task RefreshWaitingCountAsync()
    {
        try
        {
            var count = await _queueService.GetWaitingCountAsync();
            _lblWaiting.Text = $"Хүлээж буй: {count} үйлчлүүлэгч";
        }
        catch
        {
            _lblWaiting.Text = "Хүлээж буй: —";
        }
    }

    // ── Helpers ─────────────────────────────────────────────────

    /// <summary>
    /// Ачааллаж байх үед товчнуудыг идэвхгүй болгож статус харуулна.
    /// </summary>
    /// <param name="busy">Үнэн бол товчнуудыг хаана.</param>
    /// <param name="message">Харуулах мессеж.</param>
    private void SetBusy(bool busy, string message)
    {
        _btnCallNext.Enabled = !busy;
        _btnTransfer.Enabled = !busy;
        _btnCurrency.Enabled = !busy;
        if (!string.IsNullOrEmpty(message))
            SetStatus(message, Color.DarkOrange);
    }

    /// <summary>
    /// Доод хэсгийн статус мессеж болон өнгийг тохируулна.
    /// </summary>
    /// <param name="message">Харуулах мессеж.</param>
    /// <param name="color">Текстийн өнгө.</param>
    private void SetStatus(string message, Color color)
    {
        _lblStatus.Text = message;
        _lblStatus.ForeColor = color;
    }
}