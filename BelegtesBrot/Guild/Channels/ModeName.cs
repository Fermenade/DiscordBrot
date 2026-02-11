namespace BelegtesBrot.Channels;

[AttributeUsage(AttributeTargets.Class)]
public class ModeName(string name, string description) : Attribute
{
    public string Name => name;
    public string Description => description;
}