using Enuii.General.Colors;
using Enuii.General.Positioning;

namespace Enuii.Reports;

public abstract class Report(string message = "??", Span? span = null)
{
    public string    Message { get; } = message;
    public Span      Span    { get; } = span ?? new();
}

public class Error(ErrorKind kind = ErrorKind.UnknownError, string message = "An unknown error has occurred", Span? span = null)
    : Report(message, span)
{
    public ErrorKind Kind { get; } = kind;

    public override string ToString()
    {
        string str = $"{C.BOLD}• {C.RED2}{Kind}{C.END}: {C.RED}{Message}.{C.END}";

        if (Kind is not ErrorKind.InternalError)
        {
            str += "\n    ";
            str += C.YELLOW + (Span.IsShort ? "at" : "between") + C.END;
            str += $" {Span}";
        }

        return str;
    }
}

public class Warning(WarningKind kind = WarningKind.UnknownWarning, string message = "This invokes a warning", Span? span = null)
    : Report(message, span)
{
    public WarningKind Kind { get; } = kind;

    public override string ToString()
    {
        string str = $"{C.BOLD}• {C.YELLOW2}{Kind}{C.END}: {C.YELLOW}{Message}.{C.END}";
        str += "\n    ";
        str += C.BLACK2 + (Span.IsShort ? "at" : "between") + C.END;
        str += $" {Span}";

        return str;
    }
}
