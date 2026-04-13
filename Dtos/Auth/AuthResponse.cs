namespace Serai.AuthApi.Dtos.Auth;

public sealed class AuthResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = default!;

    public string AccessToken { get; set; } = default!;

    public bool IsNewUser { get; set; }

    public UserDto User { get; set; } = default!;
}

public sealed class UserDto
{
    public long Id { get; set; }

    public string PhoneNumber { get; set; } = default!;

    public string? FullName { get; set; }

    public bool IsPhoneVerified { get; set; }
}