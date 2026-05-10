using Microsoft.AspNetCore.SignalR;

namespace BankTeller.API.Hubs;

/// <summary>
/// SignalR Hub — бодит цагийн мэдэгдэл илгээнэ.
/// Теллер дугаар дуудахад → бүх дэлгэц шинэчлэгдэнэ.
/// Ханш өөрчлөгдөхөд → Blazor дэлгэц шинэчлэгдэнэ.
/// </summary>
public class BankHub : Hub
{
    /// <summary>Дугаар дуудагдсан үед бүх дэлгэцэд мэдэгдэнэ</summary>
    public async Task NotifyNumberCalled(int number) =>
        await Clients.All.SendAsync("NumberCalled", number);

    /// <summary>Ханш өөрчлөгдсөн үед Blazor дэлгэцэд мэдэгдэнэ</summary>
    public async Task NotifyRateUpdated() =>
        await Clients.All.SendAsync("RateUpdated");
}