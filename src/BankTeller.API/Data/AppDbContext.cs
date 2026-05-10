using BankTeller.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BankTeller.API.Data;

/// <summary>
/// SQLite өгөгдлийн сангийн контекст.
/// Бүх хүснэгтүүдийг удирдана.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<QueueTicket> QueueTickets => Set<QueueTicket>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<CurrencyRate> CurrencyRates => Set<CurrencyRate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().HasData(
            new Account { Id = 1,  AccountNumber = "ACN001", OwnerName = "Төгөлдөр",   Balance = 3_500_000, Currency = "MNT" },
            new Account { Id = 2,  AccountNumber = "ACN002", OwnerName = "Сарантуяа",  Balance = 1_250_000, Currency = "MNT" },
            new Account { Id = 3,  AccountNumber = "ACN003", OwnerName = "Энхжаргал",  Balance = 2_800_000, Currency = "MNT" },
            new Account { Id = 4,  AccountNumber = "ACN004", OwnerName = "Отгонбаяр",  Balance = 950_000,   Currency = "MNT" },
            new Account { Id = 5,  AccountNumber = "ACN005", OwnerName = "Нарантуяа",  Balance = 4_200_000, Currency = "MNT" },
            new Account { Id = 6,  AccountNumber = "ACN006", OwnerName = "Ганзориг",   Balance = 680_000,   Currency = "MNT" },
            new Account { Id = 7,  AccountNumber = "ACN007", OwnerName = "Уянгаа",     Balance = 1_750_000, Currency = "MNT" },
            new Account { Id = 8,  AccountNumber = "ACN008", OwnerName = "Мөнхбаяр",   Balance = 3_100_000, Currency = "MNT" },
            new Account { Id = 9,  AccountNumber = "ACN009", OwnerName = "Цэцэгмаа",   Balance = 520_000,   Currency = "MNT" },
            new Account { Id = 10, AccountNumber = "ACN010", OwnerName = "Жаргалсайхан", Balance = 2_450_000, Currency = "MNT" }
        );

        modelBuilder.Entity<CurrencyRate>().HasData(
            new CurrencyRate { Id = 1, CurrencyCode = "USD", BuyRate = 3420, SellRate = 3450, UpdatedAt = DateTime.UtcNow },
            new CurrencyRate { Id = 2, CurrencyCode = "EUR", BuyRate = 3700, SellRate = 3730, UpdatedAt = DateTime.UtcNow },
            new CurrencyRate { Id = 3, CurrencyCode = "CNY", BuyRate = 470, SellRate = 480, UpdatedAt = DateTime.UtcNow },
            new CurrencyRate { Id = 4, CurrencyCode = "RUB", BuyRate = 38, SellRate = 40, UpdatedAt = DateTime.UtcNow },
            new CurrencyRate { Id = 5, CurrencyCode = "JPY", BuyRate = 22, SellRate = 24, UpdatedAt = DateTime.UtcNow },
            new CurrencyRate { Id = 6, CurrencyCode = "KRW", BuyRate = 2, SellRate = 3, UpdatedAt = DateTime.UtcNow },
            new CurrencyRate { Id = 7, CurrencyCode = "GBP", BuyRate = 4300, SellRate = 4350, UpdatedAt = DateTime.UtcNow }
        );
    }
}