using System.ComponentModel.DataAnnotations;

namespace Serai.AuthApi.Dtos.Auth;

public sealed class VerifyOtpRequest
{
    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = default!;

    [Required]
    [MinLength(4)]
    [MaxLength(6)]
    public string Code { get; set; } = default!;
}
