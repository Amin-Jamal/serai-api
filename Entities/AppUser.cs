namespace Serai.AuthApi.Entities;

public sealed class AppUser
{
    public long Id { get; set; }

    public string PhoneNumber { get; set; } = default!;

    public bool IsPhoneVerified { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
