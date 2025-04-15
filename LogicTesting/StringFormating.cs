using System.Text.RegularExpressions;
namespace LogicTesting;
public static class StringFormating
{
    public static string[] Explode(string input)
    {
        var regex = new Regex("\"([^\"]*)\"|'([^']*)'|\\S+"); //damit leerzeichen mit """ umgangen werden können
        var matches = regex.Matches(input);

        var parts = matches.Cast<Match>().Select(m =>
            m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Success ? m.Groups[2].Value : m.Value).ToArray();
        return parts;
    }

    public static string[] SmalBoom(string input)
    {
        var regex = new Regex("\"([^\"]*)\"|'([^']*)'|\\S+");
        var matches = regex.Matches(input);

        var parts = matches.Cast<Match>().Select(m =>
            m.Groups[1].Success ? $"\"{m.Groups[1].Value}\"" : // Keep double quotes
            m.Groups[2].Success ? $"'{m.Groups[2].Value}'" : // Keep single quotes
            m.Value).ToArray(); // For unquoted strings, keep as is

        return parts;
    }
    public static string RemoveQuotes(string part)
    {
        if ($"{part[0]}" == "\"" && $"{part[^1]}"=="\"")
        {
            return part[1..^1]; // Remove double quotes
        }
        else if ($"{part[0]}" == "'" && $"{part[^1]}"=="'")
        {
            return part[1..^1]; // Remove single quotes
        }
        return part; // Return the part as is if no quotes
    }

}