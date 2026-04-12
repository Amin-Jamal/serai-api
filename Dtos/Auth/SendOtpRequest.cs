using System.ComponentModel.DataAnnotations;

namespace Serai.AuthApi.Dtos.Auth;

public sealed class SendOtpRequest
{
    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = default!;
}
