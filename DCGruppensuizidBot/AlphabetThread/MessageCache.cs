using Discord;

namespace DGruppensuizidBot.AlphabetThread;

internal class MessageCache() : FixedSizeCollection<AlphabetEntry>(100)
{
    public bool Add(AlphabetMessage message)
    {
        char[] combo = GetCombo(1);
        AlphabetEntry entry = new(message, combo);
        Add(entry);
        return entry.actuallCombination == entry.combination;
    }

    public bool Update(AlphabetMessage previous, AlphabetMessage current)
    {
        List<AlphabetEntry> x = GetCollection();
        int entryIndex = GetCollection().FindIndex(x=> x.message == previous);
        if (entryIndex == -1) throw new Exception("This should never happen!");
        AlphabetEntry entry = x[entryIndex];
        var i = entry.Update(current);
        Update(entryIndex,entry);

        return i;
    }

    public char[]? GetCombo(int offset)
    {
        char[]? currentCombo = GetCurrentCombo();
        return currentCombo == null ? null : GetCombo(currentCombo, offset);
    }
    public char[] GetCombo(char[] combo, int offset)
    {
        char[]? currentCombo = combo;

        switch (offset)
        {
            case 0:
                return currentCombo;
            case > 0:
            {
                int counter = 0;
                for (char first = currentCombo[0]; first >= 'A'; first--)
                for (char second = currentCombo[1]; second >= 'A'; second--)
                for (char third = currentCombo[2]; third >= 'A'; third--)
                {
                    counter++;
                    if (counter == offset) return [first, second, third];
                    if (counter >= offset) throw new Exception("Counter was greater than offset (for some reason unknown) THIS SHOULD NEVER HAPPEN!!");
                }

                break;
            }
            case < 0:
            {
                int counter = -0;
                for (char first = currentCombo[0]; first <= 'Z'; first++)
                for (char second = currentCombo[1]; second <= 'Z'; second++)
                for (char third = currentCombo[2]; third <= 'Z'; third++)
                {
                    counter--;
                    if (counter == offset) return [first, second, third];
                    if (counter <= offset) throw new Exception("Counter was smaller than offset (for some reason unknown) THIS SHOULD NEVER HAPPEN!!");
                }

                break;
            }
        }
        throw new Exception("This should never happen.");
    }
    public char[]? GetCurrentCombo()
    {
        AlphabetEntry? newestItem = GetCollection().Last();

        return newestItem.actuallCombination;
    }
    //public static List<char[]?> GetCombination(List<IMessage> message) =>
    //    (List<char[]?>)message.Select(GetCombination);

}