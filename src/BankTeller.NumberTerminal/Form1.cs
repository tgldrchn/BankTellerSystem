using BankTeller.Core.Models;
using BankTeller.NumberTerminal.Services;
using BankTeller.Core.Interfaces;

namespace BankTeller.NumberTerminal;

public partial class Form1 : Form
{
    private readonly IQueueService _queueClient;
    private readonly TicketPdfService _ticketPdfService;

    public Form1()
    {
        InitializeComponent();

        btnIssueTicket.Click += btnIssueTicket_Click;

        _queueClient = new ApiQueueService();
        _ticketPdfService = new TicketPdfService();

        lblTicketNumber.Text = "---";
        lblMessage.Text = "Дугаар авах товчийг дарна уу.";
    }

    private async void btnIssueTicket_Click(object sender, EventArgs e)
    {
        try
        {
            btnIssueTicket.Enabled = false;
            lblMessage.Text = "Дараагийн дугаарыг авч байна...";

            var ticket = await _queueClient.IssueNextAsync();

            ShowTicket(ticket);

            var pdfPath = _ticketPdfService.GenerateTicketPdf(ticket);

            lblMessage.Text = $"Дугаар амжилттай үүслээ. PDF файл үүслээ: {Path.GetFileName(pdfPath)}";

            MessageBox.Show(
                $"Дугаар амжилттай үүслээ.\n\nPDF файл:\n{pdfPath}",
                "Амжилттай",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        catch (Exception ex)
        {
            lblMessage.Text = "Дугаар авах үед алдаа гарлаа.";
            MessageBox.Show(ex.Message, "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnIssueTicket.Enabled = true;
        }
    }

    private void ShowTicket(QueueTicket ticket)
    {
        var ticketNumber = $"{ticket.Number:D}";
        var issuedTime = ticket.IssuedAt.ToLocalTime();

        lblTicketNumber.Text = ticketNumber;
        lblMessage.Text = $"Таны дугаар амжилттай үүслээ. Цаг: {issuedTime:HH:mm:ss}";
    }
}