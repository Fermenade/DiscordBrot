using Discord;

namespace DGruppensuizidBot.AlphabetThread;

public class AlphabetMessage(IMessage message)
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
    public Combination? GetCombination()
    {
        char[] chars = Content.ToCharArray(0, 3);
        return CheckFormat(chars) ? new Combination(chars) : null;
    }

    /// <summary>
    /// Checks if the given Combination is of length 3 and if the letters are uppercase.
    /// </summary>
    /// <param name="combination"></param>
    /// <returns>true if all conditions are fulfilled</returns>
    private static bool CheckFormat(char[] combination) => combination.Length == 3 && combination.All(x => 65 <= x && x >= 90);
}