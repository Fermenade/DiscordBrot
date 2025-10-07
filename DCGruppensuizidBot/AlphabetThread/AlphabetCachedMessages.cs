using Discord;

namespace DGruppensuizidBot.AlphabetThread;

public class AlphabetCachedMessages
{
    private MessageCache collection = new MessageCache();
    
    private async Task GetBotUpToDate(List<> collection)
    {

        Streak CurrentStreak = new();
        Streak TopStreak = new();
        foreach (IMessage msg in collection)
        {

            if (Streak.currentIndex == 0)
            {
                CurrentStreak.currentCombination = GetCombination(msg);
            }
            else if (CurrentStreak.currentCombination != GetLastCombination(msg))
            {
                CurrentStreak.streak = 0;
                CurrentStreak.currentCombination = GetCombination(msg);
            }
            else if (CurrentStreak.currentCombination == GetLastCombination(msg))
            {
                CurrentStreak.currentCombination = GetCombination(msg);
            }

            CurrentStreak.streak++;
            Streak.currentIndex++;
            if (CurrentStreak.streak > TopStreak.streak)
                TopStreak = new(CurrentStreak.streak, CurrentStreak.currentCombination);
        }

        int getIndexLastTopStreak = messages.ToList()
            .FindIndex(x => CheckFormat(x) && GetCombination(x) == TopStreak.currentCombination);
        string s = TopStreak.currentCombination;
        for (int i = 0; getIndexLastTopStreak != i; i++)
        {
            s = GetNextCombination(s);
        }

        _lastUserMessageFallback = new(messages.First().Author, s);
        Console.WriteLine(getIndexLastTopStreak);
        Console.WriteLine(s);
    }
    internal class Streak // COMBOOOO!!!
    {
        public static byte currentIndex = 0;
        public byte streak = 0;
        public string currentCombination = "";

        public Streak()
        {
        }

        public Streak(byte Streak, string CurrentCombination)
        {
            streak = Streak;
            currentCombination = CurrentCombination;
        }
    }
}