using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace NMB_HLabSys.Validation;

/// <summary>
/// Validates a South African 13-digit identity number, including the embedded
/// birth date and the Luhn check digit.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class SouthAfricanIdNumberAttribute : ValidationAttribute
{
    public SouthAfricanIdNumberAttribute()
        : base("Enter a valid 13-digit South African ID number with a valid date of birth and checksum.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is not string id || id.Length != 13 || id.Any(c => c < '0' || c > '9'))
            return false;

        if (!DateTime.TryParseExact(id[..6], "yyMMdd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var embeddedDate))
            return false;

        // Resolve the two-digit year using the current date and reject future births.
        var yy = int.Parse(id[..2], CultureInfo.InvariantCulture);
        var currentTwoDigitYear = DateTime.Today.Year % 100;
        var century = yy <= currentTwoDigitYear ? DateTime.Today.Year / 100 * 100
                                                : (DateTime.Today.Year / 100 - 1) * 100;
        var year = century + yy;
        if (year > DateTime.Today.Year)
            return false;

        var month = int.Parse(id.Substring(2, 2), CultureInfo.InvariantCulture);
        var day = int.Parse(id.Substring(4, 2), CultureInfo.InvariantCulture);
        if (month is < 1 or > 12 || day < 1 || day > DateTime.DaysInMonth(year, month))
            return false;

        // Luhn checksum for South African ID numbers.
        var sum = 0;
        var doubleDigit = false;
        for (var i = 11; i >= 0; i--)
        {
            var digit = id[i] - '0';
            if (doubleDigit)
            {
                digit *= 2;
                if (digit > 9) digit -= 9;
            }
            sum += digit;
            doubleDigit = !doubleDigit;
        }

        var checkDigit = (10 - (sum % 10)) % 10;
        return checkDigit == id[12] - '0';
    }

    public static DateTime? TryGetDateOfBirth(string? id)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Length != 13 ||
            id.Any(c => c < '0' || c > '9'))
            return null;

        var yy = int.Parse(id[..2], CultureInfo.InvariantCulture);
        var currentTwoDigitYear = DateTime.Today.Year % 100;
        var century = yy <= currentTwoDigitYear ? DateTime.Today.Year / 100 * 100
                                                : (DateTime.Today.Year / 100 - 1) * 100;
        var year = century + yy;
        var month = int.Parse(id.Substring(2, 2), CultureInfo.InvariantCulture);
        var day = int.Parse(id.Substring(4, 2), CultureInfo.InvariantCulture);

        if (year > DateTime.Today.Year || month is < 1 or > 12 ||
            day < 1 || day > DateTime.DaysInMonth(year, month))
            return null;

        var date = new DateTime(year, month, day);
        return date <= DateTime.Today ? date : null;
    }
}
