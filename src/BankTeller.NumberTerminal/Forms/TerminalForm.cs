using System.Drawing;
using System.Windows.Forms;
using BankTeller.Core.Interfaces;
using BankTeller.Core.Models;
using BankTeller.NumberTerminal.Services;

namespace BankTeller.NumberTerminal.Forms;

public class TerminalForm : Form
{
    private readonly IQueueService _queueClient;
    private readonly TicketPdfService _ticketPdfService;

    private Label lblCurrentLabel = null!;
    private Label lblTicketNumber = null!;
    private Button btnIssueTicket = null!;
    private Label lblMessage = null!;
    private Panel pnlTitleBar = null!;

    // ── Өнгөний тэмдэглэл ──────────────────────────────────────────────
    private static readonly Color ClrBg = Color.FromArgb(24, 28, 24);   // хар-ногоон фон
    private static readonly Color ClrCard = Color.FromArgb(30, 34, 30);   // картын дотор
    private static readonly Color ClrBorder = Color.FromArgb(46, 52, 46);   // хил
    private static readonly Color ClrTitleBar = Color.FromArgb(35, 40, 35);   // цонхны гарчиг
    private static readonly Color ClrGreen = Color.FromArgb(15, 110, 86);   // үндсэн ногоон
    private static readonly Color ClrGreenHover = Color.FromArgb(10, 80, 65);   // hover ногоон
    private static readonly Color ClrGreenLight = Color.FromArgb(58, 170, 128);   // том дугаарын өнгө
    private static readonly Color ClrBeige = Color.FromArgb(212, 201, 162);  // beige гарчиг
    private static readonly Color ClrMuted = Color.FromArgb(96, 112, 96);   // бүдгэрсэн текст
    // ───────────────────────────────────────────────────────────────────

    public TerminalForm()
    {
        InitializeComponents();
        _queueClient = new ApiQueueService();
        _ticketPdfService = new TicketPdfService();
    }

    private void InitializeComponents()
    {
        lblCurrentLabel = new Label();
        lblTicketNumber = new Label();
        btnIssueTicket = new Button();
        lblMessage = new Label();
        pnlTitleBar = new Panel();

        // ── Үндсэн цонх ─────────────────────────────────────────────────
        ClientSize = new Size(900, 520);
        BackColor = ClrBg;
        Text = "Дугаар авах терминал";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        ForeColor = ClrBeige;

        // ── "Гарчиг" мөр (гурван дугуй + нэр) ──────────────────────────
        pnlTitleBar.Dock = DockStyle.Top;
        pnlTitleBar.Height = 44;
        pnlTitleBar.BackColor = ClrTitleBar;
        pnlTitleBar.Paint += PaintTitleBar;

        var lblWinTitle = new Label
        {
            Text = "Дугаар авах терминал",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = ClrBeige,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(48, 0, 0, 0),
        };
        pnlTitleBar.Controls.Add(lblWinTitle);

        // ── "ОДООГИЙН ДУГААР" гэсэн жижиг тайлбар ────────────────────
        lblCurrentLabel.Text = "ОДООГИЙН ДУГААР";
        lblCurrentLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
        lblCurrentLabel.ForeColor = ClrMuted;
        lblCurrentLabel.BackColor = Color.Transparent;
        lblCurrentLabel.TextAlign = ContentAlignment.MiddleCenter;
        lblCurrentLabel.Size = new Size(900, 28);
        lblCurrentLabel.Location = new Point(0, 80);

        // ── Том дугаар ───────────────────────────────────────────────────
        lblTicketNumber.Font = new Font("Courier New", 100, FontStyle.Bold);
        lblTicketNumber.ForeColor = ClrGreenLight;
        lblTicketNumber.BackColor = Color.Transparent;
        lblTicketNumber.Text = "---";
        lblTicketNumber.TextAlign = ContentAlignment.MiddleCenter;
        lblTicketNumber.Size = new Size(900, 190);
        lblTicketNumber.Location = new Point(0, 110);

        // ── Мессеж ───────────────────────────────────────────────────────
        lblMessage.Font = new Font("Segoe UI", 11, FontStyle.Regular);
        lblMessage.ForeColor = ClrMuted;
        lblMessage.BackColor = Color.Transparent;
        lblMessage.Text = "Дугаар авах товчийг дарна уу.";
        lblMessage.TextAlign = ContentAlignment.MiddleCenter;
        lblMessage.Size = new Size(900, 28);
        lblMessage.Location = new Point(0, 318);

        // ── Товч ─────────────────────────────────────────────────────────
        btnIssueTicket.Text = "Дугаар авах";
        btnIssueTicket.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        btnIssueTicket.BackColor = ClrGreen;
        btnIssueTicket.ForeColor = Color.White;
        btnIssueTicket.FlatStyle = FlatStyle.Flat;
        btnIssueTicket.FlatAppearance.BorderSize = 0;
        btnIssueTicket.FlatAppearance.BorderColor = ClrGreen;
        btnIssueTicket.Size = new Size(360, 52);
        btnIssueTicket.Location = new Point(270, 362);
        btnIssueTicket.Cursor = Cursors.Hand;
        btnIssueTicket.Click += btnIssueTicket_Click;
        btnIssueTicket.MouseEnter += (_, _) => btnIssueTicket.BackColor = ClrGreenHover;
        btnIssueTicket.MouseLeave += (_, _) => btnIssueTicket.BackColor = ClrGreen;

        // Товчны булан дугуйлах (дүрслэлийн арга)
        btnIssueTicket.Region = RoundedRegion(btnIssueTicket.Size, 8);

        // ── Хуваагч шугам ────────────────────────────────────────────────
        var divider = new Panel
        {
            BackColor = ClrBorder,
            Size = new Size(820, 1),
            Location = new Point(40, 56),
        };

        Controls.AddRange(new Control[]
        {
            pnlTitleBar,
            divider,
            lblCurrentLabel,
            lblTicketNumber,
            lblMessage,
            btnIssueTicket,
        });
    }

    // ── Гурван дугуй зурна ────────────────────────────────────────────
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

    // ── Булан дугуйлах туслах ────────────────────────────────────────
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

    // ── Дугаар олгох логик ────────────────────────────────────────────
    private async void btnIssueTicket_Click(object sender, EventArgs e)
    {
        try
        {
            btnIssueTicket.Enabled = false;
            lblMessage.Text = "Дараагийн дугаарыг авч байна...";
            lblMessage.ForeColor = ClrMuted;

            var ticket = await _queueClient.IssueNextAsync();
            var pdfPath = _ticketPdfService.GenerateTicketPdf(ticket);

            lblTicketNumber.Text = $"{ticket.Number:D3}";
            lblMessage.Text = $"Дугаар {ticket.Number} олгогдлоо";
            lblMessage.ForeColor = ClrGreenLight;

            MessageBox.Show(
                $"Дугаар амжилттай үүслээ.\n\nPDF файл:\n{pdfPath}",
                "Амжилттай",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            lblMessage.Text = "Дугаар авах үед алдаа гарлаа.";
            lblMessage.ForeColor = Color.FromArgb(200, 80, 80);

            MessageBox.Show(
                $"Алдаа:\n{ex.Message}",
                "Алдаа",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            btnIssueTicket.Enabled = true;
        }
    }
}