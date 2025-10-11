using System.Collections.ObjectModel;

namespace DGruppensuizidBot.AlphabetThread;

public class Combination : ReadOnlyCollection<char>, ICombination<char>
{
    public Combination(char[] combination) : base(combination)
    { }
    public ICombination<char> GetCombo(int offset)
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
                                if (counter == offset) return new Combination([first, second, third]);
                                if (counter >= offset) throw new Exception("Counter was greater than offset (for some reason unknown) THIS SHOULD NEVER HAPPEN!!");
                                counter++;
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
                                if (counter == offset) return new Combination([first, second, third]);
                                if (counter <= offset) throw new Exception("Counter was smaller than offset (for some reason unknown) THIS SHOULD NEVER HAPPEN!!");
                                counter--;
                            }

                    break;
                }
        }
        throw new Exception("This should never happen.");
    }
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            for (int i = 0; i < Count; i++)
            {
                hash = hash * 23 + this[i]; // Prime numbers help distribute hash codes
            }
            return hash;
        }
    }
    public static ICombination<char> GetCombination(string input) => new Combination(input.ToCharArray(0, 3));

    public static bool CheckFormat(IList<char> combination) => combination.Count == 3 && combination.All(x => 65 <= x && x >= 90);
    public bool CheckFormat() => CheckFormat(base.Items);
}