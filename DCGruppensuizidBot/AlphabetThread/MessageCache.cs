using Discord;

namespace DGruppensuizidBot.AlphabetThread;

internal class MessageCache : FixedSizeCollection<AlphabetEntry>
{

    public MessageCache(List<AlphabetEntry> list): base(list,100)
    {

    }
    public bool Add(AlphabetMessage message)
    {
        if (Count == 0) throw new Exception("");
        char[] combo = GetCombo(1);
        AlphabetEntry entry = new(message, combo);
        return entry.actuallCombination == entry.message.GetCombination();
    }

    public bool Update(AlphabetMessage previous, AlphabetMessage current)
    {
        (int Index, AlphabetEntry Item)[] entryIndex = base.Collection.Index().Where(x=> x.Item.message == previous).ToArray();
        if (entryIndex.Length != 1)
        {
            throw new ArgumentException("Expected 1 returned entry but it where 2");
        }
        (int Index, AlphabetEntry Item) entry = entryIndex[0];
        bool e = entry.Item.Update(current);

        base.UpdateAtIndex(entry.Index,entry.Item);
        return ;
    }

    public Combination? GetCombo(int offset)
    {
        Combination currentCombo = GetCurrentCombo();
        currentCombo.GetCombo(offset);
        return currentCombo;
    }
    
    public Combination GetCurrentCombo()
    {
        AlphabetEntry newestItem = base[^1];
        return newestItem.actuallCombination;
    }
}