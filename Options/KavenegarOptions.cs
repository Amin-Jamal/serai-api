namespace Serai.AuthApi.Options;

public sealed class KavenegarOptions
{
    public const string SectionName = "Kavenegar";

    public string ApiKey { get; set; } = default!;

    public string TemplateName { get; set; } = default!;
}
