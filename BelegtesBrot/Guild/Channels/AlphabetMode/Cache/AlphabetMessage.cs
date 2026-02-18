using Discord;

namespace BelegtesBrot.Channels.Cache;

public class AlphabetMessage<T, TDatatype>(IMessage message) where T : ICombination<T, TDatatype>
{
    public ulong Id => message.Id;
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

    public Task DeleteAsync()
    {
        return message.DeleteAsync();
    }

    public ICombination<T, TDatatype>? Combination => T.GetCombination(Content);
}