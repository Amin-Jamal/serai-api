namespace Serai.AuthApi.Services.Interfaces;

public interface IKavenegarSmsService
{
    Task SendVerificationCodeAsync(string phoneNumber, string code, CancellationToken cancellationToken);
}
