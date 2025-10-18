namespace BelegtesBrot;

public sealed class HiddenLink
{
    public string DisplayText { get; private set; }
    public string Link { get; private set; }
    public HiddenLink(string displayText, string link)
    {
        DisplayText = displayText;
        Link = link;
    }
    public override string ToString()
    {
        return $"[{DisplayText}]({Link})";
    }
}