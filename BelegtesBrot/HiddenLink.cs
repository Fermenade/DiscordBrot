namespace BelegtesBrot;

public sealed class HiddenLink
{
    public HiddenLink(string displayText, string link)
    {
        DisplayText = displayText;
        Link = link;
    }

    public string DisplayText { get; }
    public string Link { get; }

    public override string ToString()
    {
        return $"[{DisplayText}]({Link})";
    }
}