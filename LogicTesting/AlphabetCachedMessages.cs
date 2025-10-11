using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DGruppensuizidBot.AlphabetThread;

public class AlphabetCachedMessages<T, TDataype> where T : ICombination<TDataype>
{
    public AlphabetCachedMessages()
    {
        var e = new AlphabetMessage<T, TDataype>("ZZZ");

        collection = new MessageCache<T, TDataype>([new AlphabetEntry<T, TDataype>(e, e.GetCombination())]);
    }
    public bool Add(AlphabetMessage<T,TDataype> message)
    {
        return collection.Add(message);
    }
    private MessageCache<T,TDataype> collection;
    private List<AlphabetEntry<T, TDataype>> GetBotUpToDate()
    {
        List<AlphabetMessage<T,TDataype>> messages = new List<AlphabetMessage<T, TDataype>>();
        Streak<TDataype> CurrentStreak = new();
        Streak<TDataype> TopStreak = new();
        foreach (AlphabetMessage<T,TDataype> msg in messages)
        {
            if (msg.GetCombination() == null)
            {
                CurrentStreak.streak = 0;
            }
            else if(CurrentStreak.currentCombination != msg.GetCombination().GetCombo(-1))
            {
                CurrentStreak.streak = 0;
                CurrentStreak.currentCombination = msg.GetCombination(); //Cannot Convert
            }
            else if (CurrentStreak.streak == 0)
            {
                CurrentStreak.currentCombination = msg.GetCombination(); //Cannot Convert
            }
            else if (CurrentStreak.currentCombination == msg.GetCombination().GetCombo(-1))
            {
                CurrentStreak.currentCombination = msg.GetCombination(); //Cannot Convert
            }

            CurrentStreak.streak++;
            Streak<T>.currentIndex++;
            if (CurrentStreak.streak > TopStreak.streak)
                TopStreak = new Streak<TDataype>(CurrentStreak.streak, CurrentStreak.currentCombination);
        }

        int getIndexLastTopStreak = messages
            .FindIndex(x => x.GetCombination() == TopStreak.currentCombination);
        ICombination<TDataype> s = TopStreak.currentCombination;
        List<AlphabetEntry<T, TDataype>> list = new List<AlphabetEntry<T, TDataype>>(messages.Count);

        for (int i = 0; i < messages.Count; i++)
        {
            list.Add(new(messages[i], (ICombination<TDataype>)TopStreak.currentCombination.GetCombo(i - getIndexLastTopStreak)));
        }

        return list;
    }
}
internal class Streak<TDataype> // COMBOOOO!!!
{
    public static byte currentIndex = 0;
    public byte streak = 0;
    public ICombination<TDataype> currentCombination;

    public Streak()
    {
    }

    public Streak(byte Streak, ICombination<TDataype> CurrentCombination)
    {
        streak = Streak;
        currentCombination = CurrentCombination;
    }
}