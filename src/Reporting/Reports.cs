using Enuii.General.Colors;
using Enuii.General.Positioning;

namespace Enuii.Reports;

public class Error(ErrorKind kind = ErrorKind.UnknownError, string message = "Unknown error has occurred", Span? span = null)
{
    public ErrorKind Kind    { get; } = kind;
    public string    Message { get; } = message;
    public Span      Span    { get; } = span ?? new();


    public override string ToString()
        => $"{C.BOLD}• {C.RED}{Kind}{C.END}: {C.RED2}{Message}{C.END}\n    {C.YELLOW2}{(Span.IsShort ? "at" : "between")}{C.END} {Span}";
}
