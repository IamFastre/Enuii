using Enuii.General.Positioning;

namespace Enuii.Reports;

public class Error(ErrorKind kind = ErrorKind.UnknownError, string message = "Unknown error has occurred", Span? span = null)
{
    public ErrorKind Kind    { get; } = kind;
    public string    Message { get; } = message;
    public Span      Span    { get; } = span ?? new();


    public override string ToString()
        => $"{Kind}: {Message}\n  {(Span.IsSingle ? "at" : "between")} {Span}";
}
