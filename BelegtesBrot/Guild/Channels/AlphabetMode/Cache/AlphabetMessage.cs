using Discord;

namespace BelegtesBrot.Channels.Cache;

public record AlphabetMessage<TCombination, TDatatype> where TCombination : ICombination<TCombination, TDatatype>
{
    public AlphabetMessage(IMessage Message)
    {
        this.Message = Message;
        AlphabetLogMessage.LogMessage(this.Message,
            $"{(this.Message.Content.Length >= 10 ? this.Message.Content.Substring(0, 10) : this.Message.Content)} - {(Combination == null ? "???" : Combination)}");
    }

    public ulong Id => Message.Id;
    public string Content => Message.Content;
    public DateTimeOffset Timestamp => Message.Timestamp;
    public DateTimeOffset? EditedTimestamp => Message.EditedTimestamp;
    public IUser Author => Message.Author;
    public IMessageChannel Channel => Message.Channel;
    public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => Message.Reactions;

    public ICombination<TCombination, TDatatype>? Combination => TCombination.GetCombination(Content);
    private IMessage Message { get; init; }

    public Task AddReactionAsync(IEmote emote, RequestOptions options = null)
    {
        return Message.AddReactionAsync(emote, options);
    }

    public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null)
    {
        return Message.RemoveReactionAsync(emote, user, options);
    }

    public Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions options = null)
    {
        return Message.RemoveReactionAsync(emote, userId, options);
    }

    public Task DeleteAsync()
    {
        return Message.DeleteAsync();
    }
}