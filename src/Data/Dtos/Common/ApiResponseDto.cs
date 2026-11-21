namespace LedgerAI.Api.Data.Dtos.Common;

public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public IReadOnlyList<string>? Errors { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    public static ApiResponseDto<T> Ok(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public static ApiResponseDto<T> Fail(string error, IReadOnlyList<string>? errors = null) =>
        new() { Success = false, Message = error, Errors = errors ?? new[] { error } };
}
