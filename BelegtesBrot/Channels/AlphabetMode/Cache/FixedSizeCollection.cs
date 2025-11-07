using System.Collections.ObjectModel;

namespace BelegtesBrot.Channels.Cache;

internal class FixedSizeCollection<T>
{
    //Maybe reverse the order of the collection (first item is newest)
    private readonly List<T> List;
    public readonly int Capacity;
    public FixedSizeCollection(List<T> list, int capacity)
    {
        ArgumentNullException.ThrowIfNull(list);
        Capacity = capacity;


        this.List = list;
        List.Capacity = Capacity;
    }

    public void Add(T item)
    {
        if (List.Count >= Capacity)
        {
            List.RemoveAt(0);
        }
        List.Add(item);
    }
    public void Update(T old, T current)
    {
        int index = List.FindIndex(x => x.Equals(old));
        if (index == -1)
        {
            throw new ArgumentException("Tried to update a item that is not present in collection");
        }
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
    public int Count => List.Count;
    public ReadOnlyCollection<T> Collection => List.AsReadOnly();

    public T this[int index]
    {
        get => List[index];
        init => throw new NotSupportedException();
    }
}