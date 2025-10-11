using BelegtesBrot.Channels.Alphabet;

namespace BelegtesBrot;

internal class AlphabetEntry(AlphabetMessage message, char[] actuallCombination)
{
    public AlphabetMessage message = message;

    public readonly char[] actuallCombination = actuallCombination;

    internal bool Update(AlphabetMessage message)
    {
        this.message = message;
        return message.GetCombination() == actuallCombination;
    }
}