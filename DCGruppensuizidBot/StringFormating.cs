using System.Text.RegularExpressions;
namespace DGruppensuizidBot;
public static class StringFormating
{
    public static string[] Explode(string input)
    {
        Regex regex = new Regex("\"([^\"]*)\"|'([^']*)'|\\S+"); //damit leerzeichen mit """ umgangen werden können
        MatchCollection matches = regex.Matches(input);

        var parts = matches.Cast<Match>().Select(m =>
            m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Success ? m.Groups[2].Value : m.Value).ToArray();
        return parts;
    }
}