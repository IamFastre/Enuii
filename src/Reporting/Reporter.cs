using System.Text;
using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Reports;

public class Reporter(IEnumerable<Error>? errors = null)
{
    public List<Error> Errors { get; } = errors?.ToList() ?? [];

    // Add new error to error list
    public void Add(Error error)
        => Errors.Add(error);

    // Remove last error from error list
    public Error? Pop()
    {
        var error = Errors.LastOrDefault();
        Errors.RemoveAt(Errors.Count - 1);
        return error;
    }

    // Make a new error and add it to error list
    private Error Report(ErrorKind kind, string message, Span span)
    {
        var error = new Error(kind, message, span);
        Add(error);
        return error;
    }

    /* =========================== Report Methods =========================== */

    internal void ReportUnterminatedQuote(TokenKind kind, Span span)
        => Report(ErrorKind.SyntaxError, $"Unterminated {kind.ToString().ToLower()} literal", span);

    internal void ReportUnrecognizedToken(StringBuilder value, Span span)
        => Report(ErrorKind.SyntaxError, $"Unrecognized token: {value}", span);
}
