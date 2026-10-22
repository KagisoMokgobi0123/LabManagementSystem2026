using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace NMB_HLabSys.Validation;

/// <summary>
/// Validates a South African 13-digit identity number, including its encoded
/// date of birth (YYMMDD), rejecting future/invalid dates and checking the Luhn digit.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class SouthAfricanIdNumberAttribute : ValidationAttribute
{
    public SouthAfricanIdNumberAttribute()
        : base("Enter a valid 13-digit South African ID number with a valid, non-future date of birth and checksum.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is not string id || !TryGetDateOfBirth(id, out _) || !HasValidChecksum(id))
            return false;

        return true;
    }

    /// <summary>Returns the DOB encoded in the first six digits when the ID's date is valid.
    /// The full ID checksum is checked separately by IsValid.</summary>
    public static DateTime? TryGetDateOfBirth(string? id)
    {
        return TryGetDateOfBirth(id, out var date) ? date : null;
    }

    private static bool TryGetDateOfBirth(string? id, out DateTime date)
    {
        date = default;
        if (string.IsNullOrWhiteSpace(id) || id.Length != 13 || id.Any(c => c < '0' || c > '9'))
            return false;

        var yy = int.Parse(id.AsSpan(0, 2), CultureInfo.InvariantCulture);
        var month = int.Parse(id.AsSpan(2, 2), CultureInfo.InvariantCulture);
        var day = int.Parse(id.AsSpan(4, 2), CultureInfo.InvariantCulture);

        // Resolve YY to the current or previous century. This avoids assigning a future year.
        var today = DateTime.Today;
        var currentCentury = today.Year / 100 * 100;
        var currentTwoDigitYear = today.Year % 100;
        var year = yy <= currentTwoDigitYear ? currentCentury + yy : currentCentury - 100 + yy;

        if (year < 1800 || year > today.Year || month is < 1 or > 12)
            return false;

        var daysInMonth = DateTime.DaysInMonth(year, month);
        if (day < 1 || day > daysInMonth)
            return false;

        date = new DateTime(year, month, day);
        return date <= today;
    }

    private static bool HasValidChecksum(string id)
    {
        // Luhn calculation across the first 12 digits. Starting at index 11,
        // double every second digit, summing the digits of products over 9.
        var sum = 0;
        for (var i = 0; i < 12; i++)
        {
            var digit = id[i] - '0';
            if (i % 2 == 1)
            {
                digit *= 2;
                if (digit > 9) digit -= 9;
            }
            sum += digit;
        }

        var expectedCheckDigit = (10 - (sum % 10)) % 10;
        return expectedCheckDigit == id[12] - '0';
    }
}
