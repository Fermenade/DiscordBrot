using System.Collections.Frozen;
using System.Reflection;

namespace BelegtesBrot.Guild.Channels;

public static class ModeChannelMapper
{
    public static FrozenDictionary<string, ModeProperties> Map
    {
        get
        {
            if (field != null) return field;

            var types = Assembly.GetExecutingAssembly().GetTypes();
            Dictionary<string, ModeProperties> prepare = new();
            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<ModeName>();
                if (attribute == null) continue;

                prepare.Add(attribute.Name, new ModeProperties(type, attribute.Description));
            }

            field = prepare.ToFrozenDictionary();

            return field;
        }
    }

    public static Type? GetMode(string modeName)
    {
        return Map.GetValueOrDefault(modeName)?.Type;
    }

    public class ModeProperties(Type type, string description)
    {
        public Type Type => type;
        public string Description => description;
    }
}