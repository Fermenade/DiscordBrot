
using Discord;

namespace BelegtesBrot.Channels.Cache;

public class AlphabetMessage<T, TDatatype>(IMessage message) where T : ICombination<T, TDatatype>
{
    public string Content => message.Content;
    public DateTimeOffset Timestamp => message.Timestamp;
    public DateTimeOffset? EditedTimestamp => message.EditedTimestamp;
    public IUser Author => message.Author;
    public IMessageChannel Channel => message.Channel;
    public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => message.Reactions;
    public Task AddReactionAsync(IEmote emote, RequestOptions options = null) => message.AddReactionAsync(emote, options);
    public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null) => message.RemoveReactionAsync(emote, user, options);
    public Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions options = null) => message.RemoveReactionAsync(emote, userId, options);
    public ICombination<T, TDatatype>? GetCombination()
    {
        return T.GetCombination(Content);
    }
}