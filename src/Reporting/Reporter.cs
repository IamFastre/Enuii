using System.Text;
using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Reports;

public class Reporter(IEnumerable<Error>? errors = null)
{
    public List<Error> Errors { get; } = errors?.ToList() ?? [];

    public void Add(Error error)
        => Errors.Add(error);
    
    public Error? Pop()
    {
        var error = Errors.LastOrDefault();
        Errors.RemoveAt(Errors.Count - 1);
        return error;
    }

    private Error Report(ErrorKind kind, string message, Span span)
    {
        var error = new Error(kind, message, span);
        Add(error);
        return error;
    }

    internal void ReportUnterminatedQuote(TokenKind kind, Span span)
        => Report(ErrorKind.SyntaxError, $"Unterminated {kind.ToString().ToLower()} literal", span);

    internal void ReportUnrecognizedToken(StringBuilder value, Span span)
        => Report(ErrorKind.SyntaxError, $"Unrecognized token: {value}", span);
}
