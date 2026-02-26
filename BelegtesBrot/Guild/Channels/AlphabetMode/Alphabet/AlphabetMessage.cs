using Discord;

namespace BelegtesBrot.Guild.Channels.AlphabetMode.Alphabet;

public class AlphabetMessage(IMessage message)
{
    public string Content => message.Content;
    public DateTimeOffset Timestamp => message.Timestamp;
    public DateTimeOffset? EditedTimestamp => message.EditedTimestamp;
    public IUser Author => message.Author;
    public IMessageChannel Channel => message.Channel;
    public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => message.Reactions;

    public Task AddReactionAsync(IEmote emote, RequestOptions options = null)
    {
        return message.AddReactionAsync(emote, options);
    }

    public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null)
    {
        return message.RemoveReactionAsync(emote, user, options);
    }

    public Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions options = null)
    {
        return message.RemoveReactionAsync(emote, userId, options);
    }

    public char[]? GetCombination()
    {
        var chars = Content.ToCharArray(0, 3);
        return CheckFormat(chars) ? chars : [(char)0, (char)0, (char)0];
    }

    /// <summary>
    ///     Checks if the given combination is of length 3 and if the letters are uppercase.
    /// </summary>
    /// <param name="combination"></param>
    /// <returns>true if all conditions are fulfilled</returns>
    private static bool CheckFormat(char[] combination)
    {
        return combination.Length == 3 && combination.All(x => 65 <= x && x >= 90);
    }
}