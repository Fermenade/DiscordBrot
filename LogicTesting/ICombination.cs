using LogicTesting;

namespace DGruppensuizidBot.AlphabetThread;

public interface ICombination<T, TDatatype> : IReadOnlyCollection<TDatatype> where T : ICombination<T, TDatatype>
{
    public ICombination<T, TDatatype> GetCombo() => GetCombo(0);

    public ICombination<T, TDatatype> GetCombo(int offset);
    public static abstract ICombination<T, TDatatype>? GetCombination(string input);
    public static abstract bool CheckFormat(IList<TDatatype> combination);
    public bool CheckFormat();
    public static abstract FailureCase AddRule(AlphabetEntry<T, TDatatype> previousMessage, AlphabetEntry<T, TDatatype> currentMessage);
    public static abstract FailureCase UpdateRule(AlphabetEntry<T, TDatatype> previousMessage, AlphabetEntry<T, TDatatype> currentMessage);
}