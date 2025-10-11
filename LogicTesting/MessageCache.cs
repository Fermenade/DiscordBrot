
using LogicTesting;

namespace DGruppensuizidBot.AlphabetThread;

internal class MessageCache<T, TDataype> : FixedSizeCollection<AlphabetEntry<T, TDataype>> where T : ICombination<T, TDataype>
{

    public MessageCache(List<AlphabetEntry<T, TDataype>> list) : base(list, 100)
    { }
    public FailureCase Add(AlphabetMessage<T, TDataype> message)
    {
        if (Count == 0) throw new Exception("Cache size was 0, can't calculate the next combination");
        AlphabetEntry<T, TDataype> lastmessage = GetLastestEntry();
        AlphabetEntry<T, TDataype> entry = new(message, lastmessage.actuallCombination.GetCombo(1));

        base.Add(entry);
        var e = entry.message.GetCombination();


        return T.AddRule(lastmessage, entry);
    }
    public FailureCase Update(AlphabetMessage<T, TDataype> previous, AlphabetMessage<T, TDataype> current)
    {
        (int Index, AlphabetEntry<T, TDataype> Item)[] entryIndex = base.Collection.Index().Where(x => Equals(x.Item.message, current)).ToArray();
        if (entryIndex.Length > 1)
        {
            throw new ArgumentException("Expected 1 returned entry but it where " + entryIndex.Length);
        }
        (int Index, AlphabetEntry<T, TDataype> Item) entry = entryIndex[0];
        entry.Item.Update(current);

        FailureCase e = T.UpdateRule(entryIndex[0].Item, entry.Item);

        base.UpdateAtIndex(entry.Index, entry.Item);

        return e;
    }
    public AlphabetEntry<T, TDataype> GetLastestEntry()
    {
        AlphabetEntry<T, TDataype> newestItem = base[^1];
        return newestItem;
    }
}