using Serai.AuthApi.Dtos.Auth;

namespace Serai.AuthApi.Services.Interfaces;

public interface IOtpService
{
    Task<SendOtpResponse> SendOtpAsync(string phoneNumber, CancellationToken cancellationToken);

    Task<AuthResponse> VerifyOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken);
}
