using BankTeller.Core.Enums;
using BankTeller.TellerApp.Services;
using Xunit;

namespace BankTeller.TellerApp.Tests;

/// <summary>
/// <see cref="MockQueueService"/>-ийн unit тестүүд.
/// Дугаарын дараалал зөв ажиллаж байгааг шалгана.
/// </summary>
public class MockQueueServiceTests
{
    // ── Fixture ──────────────────────────────────────────────────
    // Тест бүр тусдаа instance ашиглана — төлөв хооронд нөлөөлөхгүй
    private readonly MockQueueService _service = new();

    // ── IssueNextAsync ───────────────────────────────────────────

    /// <summary>
    /// Эхний дугаар олгоход 1 байх ёстой.
    /// </summary>
    [Fact]
    public async Task IssueNextAsync_FirstTicket_NumberIsOne()
    {
        var ticket = await _service.IssueNextAsync();

        Assert.Equal(1, ticket.Number);
    }

    /// <summary>
    /// Дугаар олгох бүрд нэгээр нэмэгдэх ёстой.
    /// </summary>
    [Fact]
    public async Task IssueNextAsync_MultipleTickets_NumberIncrementsCorrectly()
    {
        var t1 = await _service.IssueNextAsync();
        var t2 = await _service.IssueNextAsync();
        var t3 = await _service.IssueNextAsync();

        Assert.Equal(1, t1.Number);
        Assert.Equal(2, t2.Number);
        Assert.Equal(3, t3.Number);
    }

    /// <summary>
    /// Шинэ дугаарын төлөв Waiting байх ёстой.
    /// </summary>
    [Fact]
    public async Task IssueNextAsync_NewTicket_StatusIsWaiting()
    {
        var ticket = await _service.IssueNextAsync();

        Assert.Equal(TicketStatus.Waiting, ticket.Status);
    }

    /// <summary>
    /// Шинэ дугаарын IssuedAt утга хоосон байж болохгүй.
    /// </summary>
    [Fact]
    public async Task IssueNextAsync_NewTicket_IssuedAtIsSet()
    {
        var before = DateTime.Now;
        var ticket = await _service.IssueNextAsync();
        var after = DateTime.Now;

        Assert.InRange(ticket.IssuedAt, before, after);
    }

    // ── CallNextAsync ────────────────────────────────────────────

    /// <summary>
    /// Дараалалд байгаа тасалбарыг дуудахад null биш буцаах ёстой.
    /// </summary>
    [Fact]
    public async Task CallNextAsync_WhenTicketExists_ReturnsTicket()
    {
        await _service.IssueNextAsync();

        var ticket = await _service.CallNextAsync();

        Assert.NotNull(ticket);
    }

    /// <summary>
    /// Дуудагдсан тасалбарын төлөв Called болох ёстой.
    /// </summary>
    [Fact]
    public async Task CallNextAsync_WhenCalled_StatusIsCalled()
    {
        await _service.IssueNextAsync();

        var ticket = await _service.CallNextAsync();

        Assert.Equal(TicketStatus.Called, ticket!.Status);
    }

    /// <summary>
    /// Дараалал хоосон үед null буцаах ёстой.
    /// </summary>
    [Fact]
    public async Task CallNextAsync_WhenEmpty_ReturnsNull()
    {
        var ticket = await _service.CallNextAsync();

        Assert.Null(ticket);
    }

    /// <summary>
    /// FIFO дараалал — эхэлж орсон нь эхэлж гарах ёстой.
    /// </summary>
    [Fact]
    public async Task CallNextAsync_FIFO_ReturnsInOrder()
    {
        await _service.IssueNextAsync(); // №1
        await _service.IssueNextAsync(); // №2

        var first = await _service.CallNextAsync();
        var second = await _service.CallNextAsync();

        Assert.Equal(1, first!.Number);
        Assert.Equal(2, second!.Number);
    }

    // ── GetCurrentNumberAsync ────────────────────────────────────

    /// <summary>
    /// Дугаар олгоогүй үед 0 буцаах ёстой.
    /// </summary>
    [Fact]
    public async Task GetCurrentNumberAsync_Initially_ReturnsZero()
    {
        var number = await _service.GetCurrentNumberAsync();

        Assert.Equal(0, number);
    }

    /// <summary>
    /// Дугаар олгосны дараа сүүлийн дугаарыг буцаах ёстой.
    /// </summary>
    [Fact]
    public async Task GetCurrentNumberAsync_AfterIssue_ReturnsLatestNumber()
    {
        await _service.IssueNextAsync();
        await _service.IssueNextAsync();

        var number = await _service.GetCurrentNumberAsync();

        Assert.Equal(2, number);
    }

    // ── GetWaitingCountAsync ─────────────────────────────────────

    /// <summary>
    /// Дараалал хоосон үед 0 буцаах ёстой.
    /// </summary>
    [Fact]
    public async Task GetWaitingCountAsync_WhenEmpty_ReturnsZero()
    {
        var count = await _service.GetWaitingCountAsync();

        Assert.Equal(0, count);
    }

    /// <summary>
    /// Дугаар олгосны дараа хүлээж буй тоо нэмэгдэх ёстой.
    /// </summary>
    [Fact]
    public async Task GetWaitingCountAsync_AfterIssue_ReturnsCorrectCount()
    {
        await _service.IssueNextAsync();
        await _service.IssueNextAsync();

        var count = await _service.GetWaitingCountAsync();

        Assert.Equal(2, count);
    }

    /// <summary>
    /// Дуудсаны дараа хүлээж буй тоо буурах ёстой.
    /// </summary>
    [Fact]
    public async Task GetWaitingCountAsync_AfterCall_Decrements()
    {
        await _service.IssueNextAsync();
        await _service.IssueNextAsync();
        await _service.CallNextAsync();

        var count = await _service.GetWaitingCountAsync();

        Assert.Equal(1, count);
    }
}