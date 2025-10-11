
namespace DGruppensuizidBot.AlphabetThread;

internal class AlphabetEntry<T, TDataype> where T : ICombination<TDataype>
{
    public AlphabetMessage<T,TDataype> message;

    public readonly ICombination<TDataype> actuallCombination;

    public AlphabetEntry(AlphabetMessage<T, TDataype> message, ICombination<TDataype> actuallCombination)
    {
        this.message = message;
        this.actuallCombination = actuallCombination;
    }
    internal bool Update(AlphabetMessage<T, TDataype> message)
    {
        this.message = message;
        return message.GetCombination() == actuallCombination;
    }
}