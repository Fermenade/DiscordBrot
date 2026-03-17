using System.Collections.ObjectModel;

namespace BelegtesBrot.Guild.Channels.AlphabetMode.Cache;

internal class FixedSizeCollection<T>
{
    public readonly int Capacity;

    private readonly List<T> List;

    public FixedSizeCollection(List<T> list, int capacity)
    {
        ArgumentNullException.ThrowIfNull(list);
        Capacity = capacity;


        List = list;
        List.Capacity = Capacity;
    }

    public int Count => List.Count;
    public ReadOnlyCollection<T> Collection => [..List];

    public T this[int index]
    {
        get => List[index];
        init => throw new NotSupportedException();
    }

    public void Add(T item)
    {
        if (List.Count >= Capacity) List.RemoveAt(0);
        List.Add(item);
    }

    public void Update(T old, T current)
    {
        var index = List.FindIndex(x => x.Equals(old));
        if (index == -1) throw new ArgumentException("Tried to update a item that is not present in collection");
        UpdateAtIndex(index, current);
    }

    public void UpdateAtIndex(int index, T item)
    {
        List[index] = item;
    }

    public bool Remove(T item)
    {
        return List.Remove(item);
    }

    public void RemoveAt(int index)
    {
        List.RemoveAt(index);
    }
}