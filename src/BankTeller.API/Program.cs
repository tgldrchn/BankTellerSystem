using BankTeller.API.Channels;
using BankTeller.API.Data;
using BankTeller.API.Hubs;
using BankTeller.API.Services;
using BankTeller.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using BankTeller.API.Sockets;



var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5200");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

// SQLite
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=bank.db"));

// Channels
builder.Services.AddSingleton<NumberDisplayChannel>();
builder.Services.AddSingleton<TransactionChannel>();
builder.Services.AddHostedService(p => p.GetRequiredService<TransactionChannel>());

// Channels-ийн доор нэмнэ
builder.Services.AddSingleton<SocketServer>();
builder.Services.AddHostedService(p => p.GetRequiredService<SocketServer>());

// Services
builder.Services.AddScoped<IQueueService, QueueService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();


// CORS
builder.Services.AddCors(opt => opt.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5080")
     .AllowAnyMethod()
     .AllowAnyHeader()
     .AllowCredentials()));

var app = builder.Build();

// DB автоматаар үүсгэнэ
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapOpenApi();
app.UseCors();
app.MapControllers();
app.MapHub<BankHub>("/hubs/bank");

app.Run();