namespace AutoCommand.Utils;

/// <summary>
///     Utility class to parse enum values from strings
/// </summary>
public static class EnumParser
{
    /// <summary>
    ///     Parses a <see cref="string" /> to the desired enum type
    /// </summary>
    /// <param name="value">The value to parse</param>
    /// <param name="ignoreCase">Whether to ignore case when parsing, <c>true</c> by default</param>
    /// <typeparam name="T">Enum type to parse to</typeparam>
    /// <returns>
    ///     An instance of <c>T</c> if the value could be parsed, <c>null</c> otherwise or if the value is <c>null</c> or
    ///     empty
    /// </returns>
    public static T? ParseToEnumValue<T>(string? value, bool ignoreCase = true) where T : struct
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (Enum.TryParse<T>(value, ignoreCase, out var enumValue))
            return enumValue;

        return null;
    }

    /// <summary>
    ///     Parses multiple <see cref="string" />s to the desired enum types
    /// </summary>
    /// <param name="values">The values to parse</param>
    /// <param name="ignoreCase">Whether to ignore case when parsing, <c>true</c> by default</param>
    /// <returns>An instance of <see cref="List{T}" /> containing all values which could be parsed</returns>
    /// <remarks>If values is <c>null</c> this method returns an empty <see cref="List{T}" /></remarks>
    public static List<T> ParseToEnumValues<T>(IEnumerable<string>? values, bool ignoreCase = true) where T : struct
    {
        List<T> enumValues = [];

        if (values == null)
            return enumValues;

        foreach (var str in values)
        {
            if (string.IsNullOrWhiteSpace(str))
                continue;

            if (Enum.TryParse<T>(str, ignoreCase, out var value))
                enumValues.Add(value);
        }

        return enumValues;
    }
}