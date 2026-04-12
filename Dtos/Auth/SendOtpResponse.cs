namespace Serai.AuthApi.Dtos.Auth;

public sealed class SendOtpResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = default!;

    public int ExpiresInSeconds { get; set; }

    public int ResendAfterSeconds { get; set; }
}
