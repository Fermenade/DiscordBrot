using System.Collections.ObjectModel;

namespace DGruppensuizidBot.AlphabetThread;

public class Combination : ReadOnlyCollection<char>
{
    public Combination(char[] combination) : base(combination)
    {

    }

    public Combination GetCombo() => GetCombo(0);

    public Combination GetCombo(int offset)
    {
        Combination currentCombo = this;

        switch (offset)
        {
            case 0:
                return currentCombo;
            case > 0:
            {
                int counter = 0;
                for (char first = currentCombo[0]; first >= 'A'; first--)
                for (char second = currentCombo[1]; second >= 'A'; second--)
                for (char third = currentCombo[2]; third >= 'A'; third--)
                {
                    counter++;
                    if (counter == offset) return new Combination([first, second, third]);
                    if (counter >= offset) throw new Exception("Counter was greater than offset (for some reason unknown) THIS SHOULD NEVER HAPPEN!!");
                }

                break;
            }
            case < 0:
            {
                int counter = -0;
                for (char first = currentCombo[0]; first <= 'Z'; first++)
                for (char second = currentCombo[1]; second <= 'Z'; second++)
                for (char third = currentCombo[2]; third <= 'Z'; third++)
                {
                    counter--;
                    if (counter == offset) return new Combination([first, second, third]);
                    if (counter <= offset) throw new Exception("Counter was smaller than offset (for some reason unknown) THIS SHOULD NEVER HAPPEN!!");
                }

                break;
            }
        }
        throw new Exception("This should never happen.");
    }
}
public class AlphabetCachedMessages
{
    private MessageCache collection = new MessageCache();
    
    private List<AlphabetEntry> GetBotUpToDate()
    {
        Combination e = new Combination();
        List<AlphabetMessage> messages = new List<AlphabetMessage>();
        Streak CurrentStreak = new();
        Streak TopStreak = new();
        foreach (AlphabetMessage msg in messages)
        {
            if (msg.GetCombination() == null)
            {
                CurrentStreak.streak = 0;
            }
            else if(CurrentStreak.currentCombination != msg.GetCombination().GetCombo(-1))
            {
                CurrentStreak.streak = 0;
                CurrentStreak.currentCombination = msg.GetCombination();
            }
            else if (CurrentStreak.streak == 0)
            {
                CurrentStreak.currentCombination = msg.GetCombination();
            }
            else if (CurrentStreak.currentCombination == msg.GetCombination().GetCombo(-1))
            {
                CurrentStreak.currentCombination = msg.GetCombination();
            }

            CurrentStreak.streak++;
            Streak.currentIndex++;
            if (CurrentStreak.streak > TopStreak.streak)
                TopStreak = new Streak(CurrentStreak.streak, CurrentStreak.currentCombination);
        }

        int getIndexLastTopStreak = messages
            .FindIndex(x => x.GetCombination() == TopStreak.currentCombination);
        Combination s = TopStreak.currentCombination;
        List<AlphabetEntry> list = new List<AlphabetEntry>(messages.Count);

        for (int i = 0; i < messages.Count; i++)
        {
            list.Add(new(messages[i],TopStreak.currentCombination.GetCombo(i-getIndexLastTopStreak)));
        }

        return list;
    }
    internal class Streak // COMBOOOO!!!
    {
        public static byte currentIndex = 0;
        public byte streak = 0;
        public Combination currentCombination;

        public Streak()
        {
        }

        public Streak(byte Streak, Combination CurrentCombination)
        {
            streak = Streak;
            currentCombination = CurrentCombination;
        }
    }
}