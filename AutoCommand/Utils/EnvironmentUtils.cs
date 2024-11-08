namespace AutoCommand.Utils;

/// <summary>
///     Utility class to retrieve environment variables
/// </summary>
public static class EnvironmentUtils
{
    /// <summary>
    ///     Retrieves the environment variable with an optional fallback value
    /// </summary>
    /// <param name="name">The environment variable to retrieve</param>
    /// <param name="fallback">The fallback value</param>
    /// <returns>The environment variable or the fallback value if the environment variable is empty</returns>
    public static string GetVariable(string name, string fallback = "")
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }
}