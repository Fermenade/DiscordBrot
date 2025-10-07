namespace DGruppensuizidBot.AlphabetThread;

internal class FixedSizeCollection<T>(int capacity = 100)
{
     private readonly List<T> _collection = new(capacity);

    /// <summary>
    /// This method adds an item to the collection.
    /// This method needs   
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item)
    {
        if (_collection.Count > capacity) _collection.RemoveAt(0);

        _collection.Add(item);
    }

    public void Remove(T item)
    {
        _collection.Remove(item);
    }

    public void Update(T oldItem,T newItem)
    {
        int index = _collection.IndexOf(oldItem);
        Update(index,newItem);
    }
    public void Update(int index, T newItem)
    {
        _collection[index] = newItem;
    }

    public List<T> GetCollection() => [.._collection]; // Return a copy to prevent external modification
    public int Count => _collection.Count;
}