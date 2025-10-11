
namespace DGruppensuizidBot.AlphabetThread;

public class AlphabetMessage<T, TDatatype>(string message) where T : ICombination<T, TDatatype>
{
    public string Content => message;
    public ICombination<T, TDatatype>? GetCombination()
    {
        return T.GetCombination(Content);
    }
}