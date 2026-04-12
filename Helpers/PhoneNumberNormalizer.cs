using System.Text.RegularExpressions;

namespace Serai.AuthApi.Helpers;

public static class PhoneNumberNormalizer
{
    public static bool TryNormalizeIranPhoneNumber(string input, out string normalized)
    {
        normalized = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var raw = input.Trim().Replace(" ", "").Replace("-", "");

        if (Regex.IsMatch(raw, "^09\\d{9}$"))
        {
            normalized = $"+98{raw[1..]}";
            return true;
        }

        if (Regex.IsMatch(raw, "^989\\d{9}$"))
        {
            normalized = $"+{raw}";
            return true;
        }

        if (Regex.IsMatch(raw, "^\\+989\\d{9}$"))
        {
            normalized = raw;
            return true;
        }

        return false;
    }
}
