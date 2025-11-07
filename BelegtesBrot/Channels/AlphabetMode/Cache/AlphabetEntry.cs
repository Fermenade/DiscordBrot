namespace BelegtesBrot.Channels.Cache;

public class AlphabetEntry<T, TDataype> where T : ICombination<T, TDataype>
{
    public AlphabetMessage<T, TDataype> message;

    public readonly ICombination<T, TDataype> actuallCombination;

    public AlphabetEntry(AlphabetMessage<T, TDataype> message, ICombination<T, TDataype> actuallCombination)
    {
        this.message = message;
        this.actuallCombination = actuallCombination;
    }

    internal void Update(AlphabetMessage<T, TDataype> message)
    {
        this.message = message;
    }
}