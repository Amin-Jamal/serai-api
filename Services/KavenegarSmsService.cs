using Serai.AuthApi.Services.Interfaces;

namespace Serai.AuthApi.Services;

public sealed class KavenegarSmsService : IKavenegarSmsService
{
    private readonly ILogger<KavenegarSmsService> _logger;

    public KavenegarSmsService(ILogger<KavenegarSmsService> logger)
    {
        _logger = logger;
    }

    public Task SendVerificationCodeAsync(
        string phoneNumber,
        string code,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("FAKE OTP for {PhoneNumber}: {Code}", phoneNumber, code);
        return Task.CompletedTask;
    }
}
