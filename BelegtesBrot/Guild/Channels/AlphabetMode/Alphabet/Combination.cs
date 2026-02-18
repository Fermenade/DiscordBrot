using System.Collections.ObjectModel;
using BelegtesBrot.Channels.Cache;

namespace BelegtesBrot.Channels.Alphabet;

public class Combination(char[] combination) : ReadOnlyCollection<char>(combination), ICombination<Combination, char>
{
    public static FailureCase AddRule(AlphabetEntry<Combination, char> previousMessage,
        AlphabetEntry<Combination, char> currentMessage)
    {
        if (previousMessage.message.Author.Id == currentMessage.message.Author.Id) return FailureCase.DuplicateAuthor;
        var e = currentMessage.message.Combination;

        if (e == null) return FailureCase.NotCombination;
        if (currentMessage.actuallCombination.ToString() == e.ToString()) return FailureCase.None;

        return FailureCase.WrongCombination;
    }

    public static FailureCase UpdateRule(AlphabetEntry<Combination, char>? previousMessage,
        AlphabetEntry<Combination, char> currentMessage)
    {
        if (previousMessage == null) return FailureCase.NonExistent;
        var e = currentMessage.message.Combination;

        if (e == null) return FailureCase.NotCombination;
        if (currentMessage.actuallCombination.ToString() == e.ToString()) return FailureCase.None;

        return FailureCase.WrongCombination;
    }

    public ICombination<Combination, char> GetCombo(int offset)
    {
        char[] currentCombo = [..this];
        switch (offset)
        {
            case >= 0:
            {
                for (var x = 0; x < offset; x++)
                for (var i = 2; i >= 0; i--)
                {
                    if (currentCombo[i] < 'Z')
                    {
                        currentCombo[i]++;
                        break;
                    }

                    currentCombo[i] = 'A'; // reset and carry over
                }

                break;
            }
            case <= 0:
            {
                for (var x = 0; x > offset; x--)
                for (var i = 2; i >= 0; i--)
                {
                    if (currentCombo[i] > 'A')
                    {
                        currentCombo[i]--;
                        break;
                    }

                    currentCombo[i] = 'Z'; // reset and carry over
                }

                break;
            }
        }

        return new Combination(currentCombo);
    }

    public static ICombination<Combination, char>? GetCombination(string input)
    {
        try
        {
            var e = input.ToCharArray(0, 3);
            if (e.All(x => 65 <= x && x <= 90))
                return new Combination(e);
            return null;
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
    }

    public static bool CheckFormat(IList<char> combination)
    {
        return combination.Count == 3 && combination.All(x => 65 <= x && x <= 90);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }


    public override string ToString()
    {
        return new string(Items.ToArray());
    }

    public bool CheckFormat()
    {
        return CheckFormat(Items);
    }
}