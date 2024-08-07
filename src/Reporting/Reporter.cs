using Enuii.General.Exceptions;
using Enuii.General.Positioning;
using Enuii.Syntax.Lexing;

namespace Enuii.Reports;

public class Reporter(bool inRuntime = false)
{
    public List<Error>   Errors    { get; }      = [];
    public List<Warning> Warnings  { get; }      = [];
    public bool          InRuntime { get; set; } = inRuntime;

    public bool HasReports => Errors.Count > 0 || Warnings.Count > 0;

    public void Add(Report report)
    {

        switch (report)
        {
            case Warning warning:
                var lastWarning = Warnings.LastOrDefault();

                if (lastWarning is null || lastWarning.Message != warning.Message && lastWarning.Span != warning.Span)
                Warnings.Add(warning);
                break;

            case Error error:
                var lastError = Errors.LastOrDefault();

                if (lastError is null || lastError.Message != error.Message && lastError.Span != error.Span)
                    Errors.Add(error);

                if (InRuntime)
                    throw new EnuiiRuntimeException();
                break;
        }
    }

    // Remove last error from error list
    public Error? PopError()
    {
        var error = Errors.LastOrDefault();
        Errors.RemoveAt(Errors.Count - 1);
        return error;
    }

    // Resets the error list
    public void Flush()
    {
        Errors.Clear();
        Warnings.Clear();
    }

    // Make a new error and add it to error list
    private void Report(ErrorKind kind, string message, Span span)
        => Add(new Error(kind, message, span));

    // Make a new warning and add it to warning list
    private void Report(WarningKind kind, string message, Span span)
        => Add(new Warning(kind, message, span));

    /* ====================================================================== */
    /*                             Report Methods                             */
    /* ====================================================================== */

    /* =============================== Errors =============================== */

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
        => Report(ErrorKind.SyntaxError, $"Invalid syntax{(value.Length > 0 ? $" '{value}'" : "")}", span);

    internal void ReportInvalidClassMember(string value, Span span)
        => Report(ErrorKind.SyntaxError, $"Invalid syntax{(value.Length > 0 ? $" '{value}'" : "")} in class member declaration", span);

    internal void ReportEndOfFile(Span span)
        => Report(ErrorKind.SyntaxError, $"Unexpected end of input", span);

    internal void ReportExpressionExpectedAfter(string after, Span span)
        => Report(ErrorKind.SyntaxError, $"Expected an expression after '{after}'", span);

    internal void ReportExpectedToken(string needed, string given, Span span)
        => Report(ErrorKind.SyntaxError, $"Expected {("euioa".Contains(needed[0]) ? "an" : "a")} '{needed}' got '{given}' instead", span);

    internal void ReportInvalidAssignee(Span span)
        => Report(ErrorKind.SyntaxError, $"Invalid left-hand side of assignment", span);

    internal void ReportValueMissing(bool isConst, Span span)
        => Report(ErrorKind.SyntaxError, $"Initializer expression missing{(isConst ? " from const declaration" : "")}", span);

    internal void ReportInvalidCount(TokenKind op, Span span)
        => Report(ErrorKind.SyntaxError, $"Invalid operand of {(op is TokenKind.PlusPlus ? "increment" : "decrement")} operator", span);

    internal void ReportDefaultlessParameter(string name, Span span)
        => Report(ErrorKind.SyntaxError, $"Parameter '{name}' has no default following one that does", span);

    internal void ReportInvalidArgumentCount(string function, int needed, int given, Span span)
        => Report(ErrorKind.SyntaxError, $"Function '{function}' takes in at least ({needed}) arguments, ({given}) were given", span);

    internal void ReportCannotConvert(string type1, string type2, Span span)
        => Report(ErrorKind.TypeError, $"Cannot convert from '{type1}' to '{type2}'", span);

    internal void ReportInvalidUnaryOperator(string op, string type, Span span)
        => Report(ErrorKind.TypeError, $"Operator '{op}' cannot be applied on operand of type '{type}'", span);

    internal void ReportInvalidBinaryOperator(string op, string left, string right, Span span)
        => Report(ErrorKind.TypeError, $"Operator '{op}' cannot be applied to operands of type '{left}' and '{right}'", span);

    internal void ReportInvalidCountingOperator(string operate, string type, Span span)
        => Report(ErrorKind.TypeError, $"Cannot {operate.ToLower()} value of type '{type}'", span);

    internal void ReportUnexpectedType(string needed, string given, Span span)
        => Report(ErrorKind.TypeError, $"Expected expression of type '{needed}' got '{given}' instead", span);

    internal void ReportTypesDoNotMatch(string needed, string given, Span span)
        => Report(ErrorKind.TypeError, $"Types '{needed}' and '{given}' do not match", span);

    internal void ReportTernaryTypesDoNotMatch(string first, string second, Span span)
        => Report(ErrorKind.TypeError, $"Types '{first}' and '{second}' don't match in ternary operation", span);

    internal void ReportHeteroList(string type1, string type2, Span span)
        => Report(ErrorKind.TypeError, $"Typed list can't have '{type1}' and '{type2}'", span);

    internal void ReportWrongTypeParametersCount(string type, int needed, int given, Span span)
        => Report(ErrorKind.TypeError, $"Generic type '{type}' takes in <{needed}> parameters, <{given}> were given", span);

    internal void ReportArgumentlessGenericType(string type, Span span)
        => Report(ErrorKind.TypeError, $"Generic type '{type}' was given no arguments", span);

    internal void ReportCannotIterate(string type, Span span)
        => Report(ErrorKind.TypeError, $"Type '{type}' is not iterable", span);

    internal void ReportNotCallable(string type, Span span)
        => Report(ErrorKind.TypeError, $"Type '{type}' is not callable", span);

    internal void ReportTypeNotGeneric(string type, Span span)
        => Report(ErrorKind.SymbolError, $"Type '{type}' is not generic", span);

    internal void ReportUnusableType(string type, Span span)
        => Report(ErrorKind.SymbolError, $"Unusable type '{type}'", span);

    internal void ReportNameNotDefined(string name, Span span)
        => Report(ErrorKind.SymbolError, $"Name '{name}' is not defined", span);

    internal void ReportNameAlreadyDeclared(string name, Span span)
        => Report(ErrorKind.SymbolError, $"Name '{name}' is already defined", span);

    internal void ReportCannotAssignToConst(string constant, Span span)
        => Report(ErrorKind.SymbolError, $"Cannot assign to constant '{constant}'", span);

    internal void ReportZeroStepRange(Span span)
        => Report(ErrorKind.MathError, $"Range doesn't step much", span);

    internal void ReportBadRangeDirection(Span span)
        => Report(ErrorKind.MathError, $"Range's end point is opposite to step direction", span);

    internal void ReportInfiniteRange(Span span)
        => Report(ErrorKind.MathError, $"Cannot convert infinite range into a list", span);

    internal void ReportInternalParsingError(string value, string type, Span span)
        => Report(ErrorKind.InternalError, $"An internal error has occurred trying to parse: <{value}> (allegedly) of type '{type}'", span);

    /* ============================== Warnings ============================== */

    internal void ReportValueNotNullable(Span span)
        => Report(WarningKind.TypeWarning, $"Redundant null-forgiving; value can never be null", span);

    internal void ReportUseOfNullable(Span span)
        => Report(WarningKind.TypeWarning, $"Possibly a null reference", span);
}
