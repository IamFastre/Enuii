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

    // Resets the error list
    public void Flush()
        => Errors.Clear();

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

    internal void ReportZeroLengthChar(Span span)
        => Report(ErrorKind.SyntaxError, $"Character literal can't be empty", span);

    internal void ReportFatChar(Span span)
        => Report(ErrorKind.SyntaxError, $"Character literal can't be more than one character", span);

    internal void ReportUnrecognizedEscapeSequence(Span span)
        => Report(ErrorKind.SyntaxError, $"Invalid escape sequence", span);

    internal void ReportUnrecognizedEscapeSequence(string seq, Span span)
        => Report(ErrorKind.SyntaxError, $"Invalid escape sequence '{seq}'", span);

    internal void ReportUnrecognizedChar(string value, Span span)
        => Report(ErrorKind.SyntaxError, $"Unrecognized character '{value}'", span);

    internal void ReportInvalidSyntax(string value, Span span)
        => Report(ErrorKind.SyntaxError, $"Invalid syntax {(value.Length > 0 ? $"'{value}'" : "")}", span);

    internal void ReportEndOfFile(Span span)
        => Report(ErrorKind.SyntaxError, $"Unexpected end of input", span);

    internal void ReportExpressionExpectedAfter(string after, Span span)
        => Report(ErrorKind.SyntaxError, $"Expected an expression after '{after}'", span);

    internal void ReportExpectedToken(string needed, string given, Span span)
        => Report(ErrorKind.SyntaxError, $"Expected {("euioa".Contains(needed[0]) ? "an" : "a")} '{needed}' got '{given}' instead", span);

    internal void ReportInvalidAssignee(Span span)
        => Report(ErrorKind.SyntaxError, $"Invalid left-hand side of assignment", span);

    internal void ReportInvalidUnaryOperator(string op, string type, Span span)
        => Report(ErrorKind.TypeError, $"Operator '{op}' cannot be applied on operand of type '{type}'", span);

    internal void ReportInvalidBinaryOperator(string op, string left, string right, Span span)
        => Report(ErrorKind.TypeError, $"Operator '{op}' cannot be applied to operands of type '{left}' and '{right}'", span);

    internal void ReportUnexpectedType(string needed, string given, Span span)
        => Report(ErrorKind.TypeError, $"Expected expression of type '{needed}' got '{given}' instead", span);

    internal void ReportTypesDoNotMatch(string needed, string given, Span span)
        => Report(ErrorKind.TypeError, $"Types '{needed}' and '{given}' do not match", span);

    internal void ReportTernaryTypesDoNotMatch(string first, string second, Span span)
        => Report(ErrorKind.TypeError, $"Types '{first}' and '{second}' don't match in ternary operation", span);

    internal void ReportHeteroList(string type1, string type2, Span span)
        => Report(ErrorKind.TypeError, $"Typed list can't have '{type1}' and '{type2}'", span);

    internal void ReportCannotConvert(string type1, string type2, Span span)
        => Report(ErrorKind.TypeError, $"Cannot convert from '{type1}' to '{type2}'", span);

    internal void ReportWrongTypeParametersCount(string type, int needed, int given, Span span)
        => Report(ErrorKind.TypeError, $"Generic type '{type}' takes in <{needed}> parameters, <{given}> were given", span);

    internal void ReportCannotIterate(string type, Span span)
        => Report(ErrorKind.TypeError, $"Type '{type}' is not iterable", span);

    internal void ReportTypeNotGeneric(string type, Span span)
        => Report(ErrorKind.SymbolError, $"Type '{type}' is not generic", span);

    internal void ReportUnusableType(string type, Span span)
        => Report(ErrorKind.SymbolError, $"Unusable type '{type}'", span);

    internal void ReportNameNotDefined(string name, Span span)
        => Report(ErrorKind.SymbolError, $"Name '{name}' is not defined", span);

    internal void ReportNameAlreadyDeclared(string name, Span span)
        => Report(ErrorKind.SymbolError, $"Name '{name}' is already defined", span);

    internal void ReportZeroStepRange(Span span)
        => Report(ErrorKind.MathError, $"Range doesn't step much", span);

    internal void ReportBadRangeDirection(Span span)
        => Report(ErrorKind.MathError, $"Range's end point is opposite to step direction", span);

    internal void ReportInfiniteRange(Span span)
        => Report(ErrorKind.MathError, $"Cannot convert infinite range into a list", span);

    internal void ReportInternalParsingError(string value, string type, Span span)
        => Report(ErrorKind.InternalError, $"An internal error has occurred trying to parse: <{value}> (allegedly) of type '{type}'", span);
}
