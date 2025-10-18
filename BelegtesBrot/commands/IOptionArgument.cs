namespace BelegtesBrot;

public interface IOptionArgument
{
    string Name { get; }
    string Description { get; }
    string Usage => $"-{Name}" + (TakesParameter switch
    {
        TakesParameter.Required => $" {Parameter}",
        TakesParameter.Optional => $" ({Parameter})",
        _ => ""
    });

    /// <summary>
    /// Custom TakesParameter Name for the optionArgument.
    /// </summary>
    string Parameter => "parameter(s)";
    TakesParameter TakesParameter { get; }
    bool Visibility => true;

    object? Execute(string? command);
}