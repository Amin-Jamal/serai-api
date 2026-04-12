using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serai.AuthApi.Data;
using Serai.AuthApi.Dtos.Auth;
using Serai.AuthApi.Entities;
using Serai.AuthApi.Helpers;
using Serai.AuthApi.Options;
using Serai.AuthApi.Services.Interfaces;

namespace Serai.AuthApi.Services;

public sealed class OtpService : IOtpService
{
    private const int OtpExpirationSeconds = 120;
    private const int ResendCooldownSeconds = 90;
    private const int MaxAttempts = 5;

    private readonly AppDbContext _dbContext;
    private readonly IKavenegarSmsService _smsService;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<OtpService> _logger;

    public OtpService(
        AppDbContext dbContext,
        IKavenegarSmsService smsService,
        ITokenService tokenService,
        IOptions<JwtOptions> jwtOptions,
        ILogger<OtpService> logger)
    {
        _dbContext = dbContext;
        _smsService = smsService;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<SendOtpResponse> SendOtpAsync(
        string phoneNumber,
        CancellationToken cancellationToken)
    {
        if (!PhoneNumberNormalizer.TryNormalizeIranPhoneNumber(phoneNumber, out var normalizedPhone))
        {
            throw new InvalidOperationException("Invalid Iranian phone number.");
        }

        var now = DateTime.UtcNow;

        var lastOtp = await _dbContext.OtpCodes
            .Where(x => x.PhoneNumber == normalizedPhone)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastOtp is not null &&
            !lastOtp.IsUsed &&
            now < lastOtp.CreatedAtUtc.AddSeconds(ResendCooldownSeconds))
        {
            var remaining = (int)Math.Ceiling(
                (lastOtp.CreatedAtUtc.AddSeconds(ResendCooldownSeconds) - now).TotalSeconds);

            return new SendOtpResponse
            {
                Success = false,
                Message = $"Please wait {remaining} seconds before requesting a new code.",
                ExpiresInSeconds = OtpExpirationSeconds,
                ResendAfterSeconds = remaining
            };
        }

        var code = GenerateOtpCode();
        var codeHash = OtpHasher.Hash(code, _jwtOptions.SecretKey);

        var otp = new OtpCode
        {
            PhoneNumber = normalizedPhone,
            CodeHash = codeHash,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddSeconds(OtpExpirationSeconds),
            AttemptCount = 0,
            IsUsed = false,
            ProviderName = "kavenegar"
        };

        _dbContext.OtpCodes.Add(otp);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _smsService.SendVerificationCodeAsync(normalizedPhone, code, cancellationToken);

        _logger.LogInformation("OTP generated for {PhoneNumber}", normalizedPhone);

        return new SendOtpResponse
        {
            Success = true,
            Message = "OTP sent successfully.",
            ExpiresInSeconds = OtpExpirationSeconds,
            ResendAfterSeconds = ResendCooldownSeconds
        };
    }

    public async Task<AuthResponse> VerifyOtpAsync(
        string phoneNumber,
        string code,
        CancellationToken cancellationToken)
    {
        if (!PhoneNumberNormalizer.TryNormalizeIranPhoneNumber(phoneNumber, out var normalizedPhone))
        {
            throw new InvalidOperationException("Invalid Iranian phone number.");
        }

        var now = DateTime.UtcNow;

        var otp = await _dbContext.OtpCodes
            .Where(x => x.PhoneNumber == normalizedPhone)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp is null)
        {
            throw new InvalidOperationException("No OTP request was found for this phone number.");
        }

        if (otp.IsUsed)
        {
            throw new InvalidOperationException("This OTP has already been used.");
        }

        if (otp.ExpiresAtUtc < now)
        {
            throw new InvalidOperationException("OTP has expired.");
        }

        if (otp.AttemptCount >= MaxAttempts)
        {
            throw new InvalidOperationException("Too many invalid attempts. Please request a new OTP.");
        }

        var incomingHash = OtpHasher.Hash(code, _jwtOptions.SecretKey);

        if (!string.Equals(otp.CodeHash, incomingHash, StringComparison.OrdinalIgnoreCase))
        {
            otp.AttemptCount += 1;
            await _dbContext.SaveChangesAsync(cancellationToken);

            throw new InvalidOperationException("Invalid OTP code.");
        }

        otp.IsUsed = true;
        otp.VerifiedAtUtc = now;

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.PhoneNumber == normalizedPhone, cancellationToken);

        var isNewUser = false;

        if (user is null)
        {
            user = new AppUser
            {
                PhoneNumber = normalizedPhone,
                IsPhoneVerified = true,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            _dbContext.Users.Add(user);
            isNewUser = true;
        }
        else
        {
            user.IsPhoneVerified = true;
            user.UpdatedAtUtc = now;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var token = _tokenService.CreateAccessToken(user);

        return new AuthResponse
        {
            Success = true,
            Message = isNewUser ? "Signup successful." : "Login successful.",
            AccessToken = token,
            IsNewUser = isNewUser,
            User = new UserDto
            {
                Id = user.Id,
                PhoneNumber = user.PhoneNumber,
                IsPhoneVerified = user.IsPhoneVerified
            }
        };
    }

    private static string GenerateOtpCode()
    {
        return RandomNumberGenerator.GetInt32(10000, 99999).ToString();
    }
}
