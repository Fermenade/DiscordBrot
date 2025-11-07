using Newtonsoft.Json;

namespace BelegtesBrot.Channels;

/// <summary>
/// Attribute to flag a class as a message channel handler.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class MessageChannelHandler : Attribute;