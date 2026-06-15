namespace FptConnect.Shared.Results;

/// <summary>Kết quả thao tác dùng chung (không chứa nghiệp vụ cụ thể).</summary>
public class Result
{
    public bool Succeeded { get; init; }
    public string? Error { get; init; }
    public static Result Ok() => new() { Succeeded = true };
    public static Result Fail(string error) => new() { Succeeded = false, Error = error };
}

public sealed class Result<T> : Result
{
    public T? Value { get; init; }
    public static Result<T> Ok(T value) => new() { Succeeded = true, Value = value };
    public static new Result<T> Fail(string error) => new() { Succeeded = false, Error = error };
}
