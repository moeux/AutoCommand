namespace AutoCommand.Utils;

public static class EnvironmentUtils
{
    public static string GetVariable(string name, string fallback = "")
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }
}