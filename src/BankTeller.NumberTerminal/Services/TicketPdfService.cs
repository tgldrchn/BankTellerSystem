using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

using BankTeller.Core.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace BankTeller.NumberTerminal.Services;

public class TicketPdfService
{
    public string GenerateTicketPdf(QueueTicket ticket)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var ticketNumber = $"A-{ticket.Number:D3}";
        var issuedTime = ticket.IssuedAt.ToLocalTime();

        var folderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "BankTickets"
        );

        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(
            folderPath,
            $"Ticket_{ticketNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
        );

        QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(300, 420);
                page.Margin(20);

                page.Content().Column(column =>
                {
                    column.Spacing(10);

                    column.Item().AlignCenter().Text("BANK TELLER")
                        .FontSize(20)
                        .Bold();

                    column.Item().LineHorizontal(1);

                    column.Item().AlignCenter().Text("Таны дугаар")
                        .FontSize(14);

                    column.Item().AlignCenter().Text(ticketNumber)
                        .FontSize(48)
                        .Bold();

                    column.Item().LineHorizontal(1);

                    column.Item().Text($"Огноо: {issuedTime:yyyy-MM-dd}");
                    column.Item().Text($"Цаг: {issuedTime:HH:mm:ss}");
                    column.Item().Text("Төлөв: Хүлээж байна");

                    column.Item().PaddingTop(15).AlignCenter()
                        .Text("Дэлгэц дээр дугаараа хүлээнэ үү.")
                        .FontSize(12);
                });
            });
        })
        .GeneratePdf(filePath);

        return filePath;
    }
}