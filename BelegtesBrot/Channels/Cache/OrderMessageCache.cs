namespace BelegtesBrot.Channels.Cache;

internal class OrderMessageCache<T, TDataype> : FixedSizeCollection<AlphabetEntry<T, TDataype>> where T : ICombination<T, TDataype>
{

    public OrderMessageCache(List<AlphabetEntry<T, TDataype>> list) : base(list, 100)
    { }
    public FailureCase Add(AlphabetMessage<T, TDataype> message)
    {
        if (Count == 0) throw new Exception("Cache size was 0, can't calculate the next combination");
        AlphabetEntry<T, TDataype> lastmessage = GetLastestEntry();
        AlphabetEntry<T, TDataype> entry = new(message, lastmessage.actuallCombination.GetCombo(1));

        base.Add(entry);
        ICombination<T, TDataype>? e = entry.message.GetCombination();

        return T.AddRule(lastmessage, entry);
    }
    public FailureCase Update(AlphabetMessage<T, TDataype> previous, AlphabetMessage<T, TDataype> current)
    {
        (int Index, AlphabetEntry<T, TDataype> Item)? found = GetItem(previous);
        (int Index, AlphabetEntry<T, TDataype> Item)? foundPrevious = found;
        if (found.HasValue) // TODO: FIXME - why am I always true?
        {
            found.Value.Item.Update(current);

            base.UpdateAtIndex(found.Value.Index, found.Value.Item);
        }

        FailureCase e = T.UpdateRule(foundPrevious.Value.Item, found.Value.Item);
        return e;
    }

    public FailureCase Delete(AlphabetMessage<T, TDataype> deleted)
    {
        (int index, AlphabetEntry<T, TDataype> item)? e = GetItem(deleted);
        if (e.HasValue)
        {
            base.RemoveAt(e.Value.index);

            return FailureCase.None;
        }
        else
        {
            return FailureCase.NonExistent;
        }
    }

    private (int index, AlphabetEntry<T, TDataype> item)? GetItem(AlphabetMessage<T, TDataype> message)
    {
        (int Index, AlphabetEntry<T, TDataype> Item)[]? entryIndex = base.Collection.Index().Where(x => Equals(x.Item.message, message))?.ToArray();
        if (entryIndex?.Length > 1)
        {
            throw new ArgumentException("Expected 1 returned entry but it where " + entryIndex.Length);
        }
        return entryIndex?[0];
    }
    public AlphabetEntry<T, TDataype> GetLastestEntry()
    {
        AlphabetEntry<T, TDataype> newestItem = base[^1];
        return newestItem;
    }
}