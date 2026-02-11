namespace BelegtesBrot.Channels.Cache;

public class AlphabetEntry<T, TDataype> where T : ICombination<T, TDataype>
{
    public readonly ICombination<T, TDataype> actuallCombination;
    public AlphabetMessage<T, TDataype> message;

    public AlphabetEntry(AlphabetMessage<T, TDataype> message, ICombination<T, TDataype> actuallCombination)
    {
        this.message = message;
        this.actuallCombination = actuallCombination;
    }

    public AlphabetEntry(AlphabetEntry<T, TDataype> message)
    {
        this.message = message.message;
        actuallCombination = message.actuallCombination;
    }

    internal void Update(AlphabetMessage<T, TDataype> message)
    {
        this.message = message;
    }
}