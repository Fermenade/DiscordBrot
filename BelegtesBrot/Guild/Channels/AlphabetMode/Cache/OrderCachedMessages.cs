using System.Collections.ObjectModel;

namespace BelegtesBrot.Guild.Channels.AlphabetMode.Cache;

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

    public AlphabetEntry<T, TDataype> GetLastestEntry() => collection.GetLastestEntry();
    private List<AlphabetEntry<T, TDataype>> GetBotUpToDate(List<AlphabetMessage<T, TDataype>> messages)
    {
        Streak<T, TDataype> currentStreak = new();
        Streak<T, TDataype> topStreak = new();
        foreach (var msg in messages)
        {
            if (msg.Combination == null)
            {
                currentStreak.CombinationStreak = 0;
                continue;
            }


            if (currentStreak.CombinationStreak == 0)
            {
                currentStreak.CurrentCombination = msg.Combination!; //Cannot Convert
            }
            else
            {
                var e = msg.Combination?.GetCombo(1);
                var x = currentStreak.CurrentCombination?.ToString();
                var y = e!.ToString();
                if (x != y)
                {
                    currentStreak.CombinationStreak = 0;
                    currentStreak.CurrentCombination = msg.Combination!;
                }
                else if (x == y)
                {
                    currentStreak.CurrentCombination = msg.Combination!;
                }
            }


            currentStreak.CombinationStreak++;
            if (currentStreak.CombinationStreak > topStreak.CombinationStreak)
            {
                topStreak = new Streak<T, TDataype>(currentStreak.CombinationStreak, currentStreak.CurrentCombination);
                Logger.LogMessage($"New top streak {topStreak.CurrentCombination}:  {topStreak.CombinationStreak}");
            }
        }

        var getIndexLastTopStreak =
            messages.FindIndex(x => x.Combination?.ToString() == topStreak.CurrentCombination.ToString());

        Logger.LogMessage($"Last index of top Streak {getIndexLastTopStreak}");
        var s = topStreak.CurrentCombination;
        var list = new List<AlphabetEntry<T, TDataype>>(messages.Count);

        for (var i = 0; i < messages.Count; i++)
        {
            var o = topStreak.CurrentCombination.GetCombo(-i + getIndexLastTopStreak);
            list.Add(new AlphabetEntry<T, TDataype>(messages[i], o));
            var e = messages[i].Combination;
            var x = e == null ? "***" : e.ToString()!;
            Logger.LogMessage($"{-i + getIndexLastTopStreak}: {x}: {o}");
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