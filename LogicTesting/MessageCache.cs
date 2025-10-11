
namespace DGruppensuizidBot.AlphabetThread;

internal class MessageCache<T, TDataype> : FixedSizeCollection<AlphabetEntry<T, TDataype>> where T : ICombination<TDataype>
{

    public MessageCache(List<AlphabetEntry<T, TDataype>> list) : base(list, 100)
    { }
    public bool Add(AlphabetMessage<T, TDataype> message)
    {
        if (Count == 0) throw new Exception("Cache size was 0, can't calculate the next combination");
        ICombination<TDataype> combo = GetCombo(1);
        AlphabetEntry<T, TDataype> entry = new(message, combo);

        base.Add(entry);

        return entry.actuallCombination.GetHashCode() == entry.message.GetCombination().GetHashCode();
    }

    public bool Update(AlphabetMessage<T, TDataype> previous, AlphabetMessage<T, TDataype> current)
    {
        (int Index, AlphabetEntry<T, TDataype> Item)[] entryIndex = base.Collection.Index().Where(x => Equals(x.Item.message, current)).ToArray();
        if (entryIndex.Length != 1)
        {
            throw new ArgumentException("Expected 1 returned entry but it where 2");
        }
        (int Index, AlphabetEntry<T, TDataype> Item) entry = entryIndex[0];
        bool e = entry.Item.Update(current);

        base.UpdateAtIndex(entry.Index, entry.Item);
        return e;
    }

    public ICombination<TDataype> GetCombo(int offset)
    {
        ICombination<TDataype> currentCombo = GetCurrentCombo();
        return currentCombo.GetCombo(offset);
    }

    public ICombination<TDataype> GetCurrentCombo()
    {
        AlphabetEntry<T, TDataype> newestItem = base[^1];
        return newestItem.actuallCombination;
    }
}