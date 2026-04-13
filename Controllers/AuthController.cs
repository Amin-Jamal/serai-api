using Microsoft.AspNetCore.Mvc;
using Serai.AuthApi.Dtos.Auth;
using Serai.AuthApi.Services.Interfaces;

namespace Serai.AuthApi.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IOtpService _otpService;

    public AuthController(IOtpService otpService)
    {
        _otpService = otpService;
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp(
        [FromBody] SendOtpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _otpService.SendOtpAsync(request.PhoneNumber, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(
        [FromBody] VerifyOtpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _otpService.VerifyOtpAsync(
                request.PhoneNumber,
                request.Code,
                cancellationToken);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}