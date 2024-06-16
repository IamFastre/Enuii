using Enuii.General.Colors;
using Enuii.General.Positioning;

namespace Enuii.Reports;

public class Error(ErrorKind kind = ErrorKind.UnknownError, string message = "An unknown error has occurred", Span? span = null)
{
    public ErrorKind Kind    { get; } = kind;
    public string    Message { get; } = message;
    public Span      Span    { get; } = span ?? new();


    public override string ToString()
    {
        string str = $"{C.BOLD}• {C.RED}{Kind}{C.END}: {C.RED2}{Message}.{C.END}";

        if (Kind is not ErrorKind.InternalError)
        {
            str += "\n    ";
            str += C.YELLOW2 + (Span.IsShort ? "at" : "between") + C.END;
            str += $" {Span}";
        }

        return str;
    }
}
