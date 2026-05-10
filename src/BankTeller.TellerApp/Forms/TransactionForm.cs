using BankTeller.Core.Interfaces;
using BankTeller.Core.Models;
using BankTeller.TellerApp.Services;

namespace BankTeller.TellerApp.Forms;

/// <summary>
/// Мөнгөн гүйлгээ гүйцэтгэх форм.
/// Теллер энэ формоор А данснаас Б данс руу мөнгө шилжүүлнэ.
/// Дансны мэдээллийг шалгасны дараа л шилжүүлэх боломжтой.
/// </summary>
public class TransactionForm : Form
{
    // ── Services ────────────────────────────────────────────────
    /// <summary>Гүйлгээ гүйцэтгэх болон данс шалгах сервис.</summary>
    private readonly ITransactionService _transactionService;

    // ── Controls ────────────────────────────────────────────────
    private TextBox _txtFrom = null!;
    private TextBox _txtTo = null!;
    private TextBox _txtAmount = null!;
    private Label _lblFromInfo = null!;
    private Label _lblToInfo = null!;
    private Label _lblStatus = null!;
    private Button _btnCheckFrom = null!;
    private Button _btnCheckTo = null!;
    private Button _btnTransfer = null!;

    // ── State ────────────────────────────────────────────────────
    /// <summary>Гарах данс шалгагдсан эсэх.</summary>
    private bool _fromVerified = false;

    /// <summary>Орох данс шалгагдсан эсэх.</summary>
    private bool _toVerified = false;

    // ── Constructor ──────────────────────────────────────────────

    /// <summary>
    /// TransactionForm-ийг DI-аар <see cref="ITransactionService"/> хүлээн авч эхлүүлнэ.
    /// </summary>
    /// <param name="transactionService">Гүйлгээ болон данс удирдах сервис.</param>
    public TransactionForm(ITransactionService transactionService)
    {
        _transactionService = transactionService;
        InitializeComponents();
    }

    // ── UI Setup ─────────────────────────────────────────────────

    /// <summary>
    /// Формын бүх UI элементүүдийг үүсгэж байрлуулна.
    /// </summary>
    private void InitializeComponents()
    {
        Text = "Мөнгө шилжүүлэх";
        Size = new Size(460, 600);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        BackColor = Color.FromArgb(245, 247, 250);
        MaximizeBox = false;

        // ── Header ───────────────────────────────────────────────
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 56,
            BackColor = Color.FromArgb(22, 120, 80)
        };
        header.Controls.Add(new Label
        {
            Text = "💸  Мөнгө шилжүүлэх",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 0, 0, 0)
        });

        // ── From account ─────────────────────────────────────────
        CreateAccountPanel("ГАРАХ ДАНС", 76,
            out _txtFrom, out _btnCheckFrom, out _lblFromInfo);

        _txtFrom.TextChanged += (_, _) => ResetFromVerified();
        _btnCheckFrom.Click += async (_, _) =>
            _fromVerified = await CheckAccountAsync(
                _txtFrom.Text, _lblFromInfo, _btnCheckFrom);

        // ── To account ───────────────────────────────────────────
        CreateAccountPanel("ОРОХ ДАНС", 216,
            out _txtTo, out _btnCheckTo, out _lblToInfo);

        _txtTo.TextChanged += (_, _) => ResetToVerified();
        _btnCheckTo.Click += async (_, _) =>
            _toVerified = await CheckAccountAsync(
                _txtTo.Text, _lblToInfo, _btnCheckTo);

        // ── Amount ───────────────────────────────────────────────
        var lblAmount = new Label
        {
            Text = "ДҮН",
            Location = new Point(20, 356),
            Size = new Size(400, 16),
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            ForeColor = Color.Gray
        };

        _txtAmount = new TextBox
        {
            Location = new Point(20, 376),
            Size = new Size(400, 32),
            Font = new Font("Segoe UI", 14),
            PlaceholderText = "0"
        };

        var lblCurrency = new Label
        {
            Text = "₮",
            Location = new Point(428, 378),
            Size = new Size(24, 28),
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(22, 120, 80)
        };

        // ── Transfer button ──────────────────────────────────────
        _btnTransfer = new Button
        {
            Text = "💸  Шилжүүлэх",
            Location = new Point(20, 424),
            Size = new Size(400, 48),
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            BackColor = Color.FromArgb(22, 120, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        _btnTransfer.FlatAppearance.BorderSize = 0;
        _btnTransfer.Click += async (_, _) => await TransferAsync();

        // ── Status ───────────────────────────────────────────────
        _lblStatus = new Label
        {
            Location = new Point(20, 478),
            Size = new Size(400, 20),
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.Gray,
            TextAlign = ContentAlignment.MiddleCenter
        };

        // ── Assemble ─────────────────────────────────────────────
        Controls.AddRange(new Control[]
        {
            header,
            lblAmount, _txtAmount, lblCurrency,
            _btnTransfer, _lblStatus
        });
    }

    /// <summary>
    /// Данс оруулах нэгдсэн панел үүсгэнэ (label + textbox + шалгах товч + мэдэгдэл).
    /// </summary>
    private void CreateAccountPanel(
        string title, int top,
        out TextBox txt, out Button btnCheck, out Label lblInfo)
    {
        var panel = new Panel
        {
            Location = new Point(20, top),
            Size = new Size(400, 130),
            BackColor = Color.White
        };
        panel.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(220, 225, 235));
            e.Graphics.DrawRectangle(pen,
                new Rectangle(0, 0, panel.Width - 1, panel.Height - 1));
        };

        var lbl = new Label
        {
            Text = title,
            Location = new Point(12, 10),
            Size = new Size(370, 16),
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            ForeColor = Color.Gray
        };

        txt = new TextBox
        {
            Location = new Point(12, 32),
            Size = new Size(248, 28),
            Font = new Font("Segoe UI", 11),
            PlaceholderText = "Дансны дугаар"
        };

        btnCheck = new Button
        {
            Text = "Шалгах",
            Location = new Point(268, 30),
            Size = new Size(112, 32),
            BackColor = Color.FromArgb(26, 60, 110),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnCheck.FlatAppearance.BorderSize = 0;

        lblInfo = new Label
        {
            Location = new Point(12, 74),
            Size = new Size(370, 46),
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.Gray,
            Text = "Дансны дугаар оруулаад шалгана уу."
        };

        panel.Controls.AddRange(new Control[] { lbl, txt, btnCheck, lblInfo });
        Controls.Add(panel);
    }

    // ── Logic ─────────────────────────────────────────────────────

    /// <summary>
    /// Дансны дугаараар данс хайж, эзэн болон үлдэгдлийг харуулна.
    /// </summary>
    /// <param name="accNumber">Хайх дансны дугаар.</param>
    /// <param name="infoLabel">Мэдэгдэл харуулах label.</param>
    /// <param name="btnCheck">Шалгах товч — амжилттай үед өнгө өөрчлөгдөнө.</param>
    /// <returns>Данс олдсон бол <c>true</c>, олдоогүй бол <c>false</c>.</returns>
    private async Task<bool> CheckAccountAsync(
        string accNumber, Label infoLabel, Button btnCheck)
    {
        if (string.IsNullOrWhiteSpace(accNumber))
        {
            infoLabel.Text = "Дансны дугаар хоосон байна.";
            infoLabel.ForeColor = Color.Crimson;
            return false;
        }

        btnCheck.Text = "...";
        btnCheck.Enabled = false;

        try
        {
            var acc = await _transactionService.GetAccountAsync(accNumber);

            if (acc == null)
            {
                infoLabel.Text = "⚠  Данс олдсонгүй.";
                infoLabel.ForeColor = Color.Crimson;
                btnCheck.BackColor = Color.Crimson;
                return false;
            }

            infoLabel.Text = $"✓  {acc.OwnerName}  •  Үлдэгдэл: {acc.Balance:N0} ₮";
            infoLabel.ForeColor = Color.FromArgb(22, 120, 80);
            btnCheck.BackColor = Color.FromArgb(22, 120, 80);
            return true;
        }
        catch (Exception ex)
        {
            infoLabel.Text = $"Алдаа: {ex.Message}";
            infoLabel.ForeColor = Color.Crimson;
            return false;
        }
        finally
        {
            btnCheck.Text = "Шалгах";
            btnCheck.Enabled = true;
        }
    }

    /// <summary>
    /// Хоёр данс шалгагдсан, дүн зөв оруулсан үед гүйлгээ явуулна.
    /// Амжилттай бол форм хаагдана.
    /// </summary>
    private async Task TransferAsync()
    {
        if (!_fromVerified || !_toVerified)
        {
            SetStatus("⚠  Эхлээд хоёр дансыг шалгана уу.", Color.DarkOrange);
            return;
        }

        if (!decimal.TryParse(_txtAmount.Text, out var amount) || amount <= 0)
        {
            SetStatus("⚠  Зөв дүн оруулна уу.", Color.Crimson);
            return;
        }

        if (_txtFrom.Text.Trim() == _txtTo.Text.Trim())
        {
            SetStatus("⚠  Гарах болон орох данс ижил байна.", Color.Crimson);
            return;
        }

        _btnTransfer.Text = "Шилжүүлж байна...";
        _btnTransfer.Enabled = false;
        SetStatus("Гүйлгээ хийгдэж байна...", Color.DarkOrange);

        try
        {
            var success = await _transactionService.TransferAsync(
                _txtFrom.Text.Trim(), _txtTo.Text.Trim(), amount);

            if (success)
            {
                MessageBox.Show(
                    $"{amount:N0} ₮ амжилттай шилжлээ!",
                    "Амжилттай",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                Close();
            }
            else
            {
                SetStatus("⚠  Үлдэгдэл хүрэлцэхгүй эсвэл данс олдсонгүй.", Color.Crimson);
            }
        }
        catch (Exception ex)
        {
            SetStatus($"Алдаа: {ex.Message}", Color.Crimson);
        }
        finally
        {
            _btnTransfer.Text = "💸  Шилжүүлэх";
            _btnTransfer.Enabled = true;
        }
    }

    // ── Reset helpers ─────────────────────────────────────────────

    /// <summary>
    /// Гарах дансны verified төлөвийг арилгаж товчны өнгийг анхны байдалд оруулна.
    /// </summary>
    private void ResetFromVerified()
    {
        _fromVerified = false;
        _lblFromInfo.Text = "Дансны дугаар оруулаад шалгана уу.";
        _lblFromInfo.ForeColor = Color.Gray;
        _btnCheckFrom.BackColor = Color.FromArgb(26, 60, 110);
    }

    /// <summary>
    /// Орох дансны verified төлөвийг арилгаж товчны өнгийг анхны байдалд оруулна.
    /// </summary>
    private void ResetToVerified()
    {
        _toVerified = false;
        _lblToInfo.Text = "Дансны дугаар оруулаад шалгана уу.";
        _lblToInfo.ForeColor = Color.Gray;
        _btnCheckTo.BackColor = Color.FromArgb(26, 60, 110);
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