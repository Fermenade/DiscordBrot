namespace DGruppensuizidBot.AlphabetThread;

public interface ICombination<TDatatype> : IReadOnlyCollection<TDatatype>
{
    public ICombination<TDatatype> combination => this;
    public ICombination<TDatatype> GetCombo() => GetCombo(0);

    public ICombination<TDatatype> GetCombo(int offset);
    public static abstract ICombination<TDatatype> GetCombination(string input);
    public static abstract bool CheckFormat(IList<TDatatype> combination);
    public bool CheckFormat();

}