namespace BelegtesBrot.Guild.Channels.AlphabetMode.Cache;

internal class OrderMessageCache<T, TDataype> : FixedSizeCollection<AlphabetEntry<T, TDataype>>
    where T : ICombination<T, TDataype>
{
    public OrderMessageCache(List<AlphabetEntry<T, TDataype>> list) : base(list, 100)
    {
    }

    public FailureCase Add(AlphabetMessage<T, TDataype> message)
    {
        if (Count == 0) throw new Exception("Cache size was 0, can't calculate the next combination");
        var lastmessage = GetLastestEntry();
        AlphabetEntry<T, TDataype> entry = new(message, lastmessage.actuallCombination.GetCombo(-1));

        base.Add(entry);
        var e = entry.message.Combination;

        return T.AddRule(lastmessage, entry);
    }

    public FailureCase Update(ulong id, AlphabetMessage<T, TDataype> current)
    {
        (int Index, AlphabetEntry<T, TDataype> Item)? found = GetItem(id);
        if (!found.HasValue) return FailureCase.NonExistent;
        
        var foundPrevious = new AlphabetEntry<T, TDataype>(found.Value.Item);
        found.Value.Item.Update(current);
        UpdateAtIndex(found.Value.Index, found.Value.Item);
        return T.UpdateRule(foundPrevious, found.Value.Item);;
    }

    public FailureCase Delete(ulong deletedId)
    {
        var e = GetItem(deletedId);
        if (e.HasValue)
        {
            RemoveAt(e.Value.index);

            return FailureCase.None;
        }

        return FailureCase.NonExistent;
    }

    private (int index, AlphabetEntry<T, TDataype> item)? GetItem(ulong id)
    {
        var entryIndex = Collection.Index().Where(x => Equals(x.Item.message.Id, id))?.ToArray();
        if (entryIndex?.Length > 1)
            throw new ArgumentException("Expected 1 returned entry but it where " + entryIndex.Length);

        return entryIndex?.Length == 0 ? null : entryIndex?[0];
    }

    public AlphabetEntry<T, TDataype> GetLastestEntry()
    {
        return base[^1];
    }
}