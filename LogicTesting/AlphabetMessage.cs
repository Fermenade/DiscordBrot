
namespace DGruppensuizidBot.AlphabetThread;

public class AlphabetMessage<T,TDatatype>(string message) where T : ICombination<TDatatype>
{
    public string Content => message;
    public ICombination<TDatatype> GetCombination()
    {
        return T.GetCombination(Content);
    }
}