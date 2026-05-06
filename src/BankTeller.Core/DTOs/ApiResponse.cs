namespace BankTeller.Core.DTOs;

/// <summary>
/// Бүх API endpoint-ын стандарт хариу загвар.
/// Амжилттай болон алдааны хариуг нэг хэлбэрт оруулна.
/// </summary>
/// <typeparam name="T">Буцаах өгөгдлийн төрөл</typeparam>
public class ApiResponse<T>
{
    /// <summary>Хүсэлт амжилттай болсон эсэх</summary>
    public bool Success { get; set; }

    /// <summary>Амжилт эсвэл алдааны тайлбар мессеж</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Буцаах өгөгдөл. Алдаа гарсан үед null байна.</summary>
    public T? Data { get; set; }

    /// <summary>Амжилттай хариу үүсгэнэ</summary>
    public static ApiResponse<T> Ok(T data, string message = "Амжилттай")
        => new() { Success = true, Data = data, Message = message };

    /// <summary>Алдааны хариу үүсгэнэ</summary>
    public static ApiResponse<T> Fail(string message)
        => new() { Success = false, Message = message };
}