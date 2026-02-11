using System.Collections.ObjectModel;

namespace BelegtesBrot.Channels.Cache;

public class OrderCachedMessages<T, TDataype> where T : ICombination<T, TDataype>
{
    private readonly OrderMessageCache<T, TDataype> collection;

    public OrderCachedMessages(List<AlphabetMessage<T, TDataype>> messages)
    {
        var e = GetBotUpToDate(messages);
        collection = new OrderMessageCache<T, TDataype>(e);
    }

    public int Count => collection.Count;
    public ReadOnlyCollection<AlphabetEntry<T, TDataype>> Collection => collection.Collection;

    public FailureCase Add(AlphabetMessage<T, TDataype> message)
    {
        return collection.Add(message);
    }

    public FailureCase Update(ulong id, AlphabetMessage<T, TDataype> current)
    {
        return collection.Update(id, current);
    }

    public FailureCase Delete(ulong deletedId)
    {
        return collection.Delete(deletedId);
    }

    private List<AlphabetEntry<T, TDataype>> GetBotUpToDate(List<AlphabetMessage<T, TDataype>> messages)
    {
        Streak<T, TDataype> currentStreak = new();
        Streak<T, TDataype> topStreak = new();
        foreach (var msg in messages)
        {
            if (msg.GetCombination() == null)
            {
                currentStreak.CombinationStreak = 0;
                Console.WriteLine($"Combination of message {msg.Content} was null");
                continue;
            }


            if (currentStreak.CombinationStreak == 0)
            {
                currentStreak.CurrentCombination = msg.GetCombination()!; //Cannot Convert
                Console.WriteLine($"New streak {msg.GetCombination()}");
            }
            else
            {
                var e = msg.GetCombination()?.GetCombo(1);
                var x = currentStreak.CurrentCombination?.GetHashCode();
                var y = e!.GetHashCode();
                if (x != y)
                {
                    currentStreak.CombinationStreak = 0;
                    currentStreak.CurrentCombination = msg.GetCombination()!;
                    Console.WriteLine(
                        $"Expected Combination {currentStreak.CurrentCombination.GetCombo(-1)} but combination was {msg.GetCombination()}");
                }
                else if (x == y)
                {
                    Console.WriteLine(
                        $"Combination {currentStreak.CurrentCombination.GetCombo(-1)} matched {msg.GetCombination()}");
                    currentStreak.CurrentCombination = msg.GetCombination()!;
                }
            }


            currentStreak.CombinationStreak++;
            if (currentStreak.CombinationStreak > topStreak.CombinationStreak)
            {
                topStreak = new Streak<T, TDataype>(currentStreak.CombinationStreak, currentStreak.CurrentCombination);
                Console.WriteLine($"New top streak {topStreak.CurrentCombination}:  {topStreak.CombinationStreak}");
            }
        }

        var getIndexLastTopStreak = messages
            .FindIndex(x => x.GetCombination()?.GetHashCode() == topStreak.CurrentCombination.GetHashCode());
        Console.WriteLine($"Last index of top Streak {getIndexLastTopStreak}");
        var s = topStreak.CurrentCombination;
        var list = new List<AlphabetEntry<T, TDataype>>(messages.Count);

        for (var i = 0; i < messages.Count; i++)
        {
            var o = topStreak.CurrentCombination.GetCombo(-i + getIndexLastTopStreak);
            list.Add(new AlphabetEntry<T, TDataype>(messages[i], o));
            var e = messages[i].GetCombination();
            var x = e == null ? "***" : e.ToString()!;
            Console.WriteLine($"{x}: {o}");
        }

        return list;
    }
}

internal class Streak<T, TDataype> where T : ICombination<T, TDataype>
{
    public byte CombinationStreak;
    public ICombination<T, TDataype> CurrentCombination;

    public Streak()
    {
    }

    public Streak(byte combinationStreak, ICombination<T, TDataype> currentCombination)
    {
        CombinationStreak = combinationStreak;
        CurrentCombination = currentCombination;
    }
}