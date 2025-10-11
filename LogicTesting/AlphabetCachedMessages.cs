using LogicTesting;

namespace DGruppensuizidBot.AlphabetThread;

public class AlphabetCachedMessages<T, TDataype> where T : ICombination<T, TDataype>
{
    public AlphabetCachedMessages()
    {
        var e = new AlphabetMessage<T, TDataype>("ZZZ");

        collection = new MessageCache<T, TDataype>([new AlphabetEntry<T, TDataype>(e, e.GetCombination())]);
    }
    public FailureCase Add(AlphabetMessage<T, TDataype> message)
    {
        return collection.Add(message);
    }
    private MessageCache<T, TDataype> collection;
    private List<AlphabetEntry<T, TDataype>> GetBotUpToDate()
    {
        List<AlphabetMessage<T, TDataype>> messages = new List<AlphabetMessage<T, TDataype>>();
        Streak<T, TDataype> CurrentStreak = new();
        Streak<T, TDataype> TopStreak = new();
        foreach (AlphabetMessage<T, TDataype> msg in messages)
        {
            if (msg.GetCombination() == null)
            {
                CurrentStreak.streak = 0;
            }
            else if (CurrentStreak.currentCombination != msg.GetCombination().GetCombo(-1))
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
            Streak<T, TDataype>.currentIndex++;
            if (CurrentStreak.streak > TopStreak.streak)
                TopStreak = new Streak<T, TDataype>(CurrentStreak.streak, CurrentStreak.currentCombination);
        }

        int getIndexLastTopStreak = messages
            .FindIndex(x => x.GetCombination() == TopStreak.currentCombination);
        ICombination<T, TDataype> s = TopStreak.currentCombination;
        List<AlphabetEntry<T, TDataype>> list = new List<AlphabetEntry<T, TDataype>>(messages.Count);

        for (int i = 0; i < messages.Count; i++)
        {
            list.Add(new(messages[i], (ICombination<T, TDataype>)TopStreak.currentCombination.GetCombo(i - getIndexLastTopStreak)));
        }

        return list;
    }
}
internal class Streak<T, TDataype> where T : ICombination<T, TDataype>
{
    public static byte currentIndex = 0;
    public byte streak = 0;
    public ICombination<T, TDataype> currentCombination;

    public Streak()
    {
    }

    public Streak(byte Streak, ICombination<T, TDataype> CurrentCombination)
    {
        streak = Streak;
        currentCombination = CurrentCombination;
    }
}