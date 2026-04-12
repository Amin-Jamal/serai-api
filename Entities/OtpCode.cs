namespace Serai.AuthApi.Entities;

public sealed class OtpCode
{
    public long Id { get; set; }

    public string PhoneNumber { get; set; } = default!;

    public string CodeHash { get; set; } = default!;

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? VerifiedAtUtc { get; set; }

    public int AttemptCount { get; set; }

    public bool IsUsed { get; set; }

    public string ProviderName { get; set; } = "kavenegar";

    public string? ProviderReference { get; set; }
}
