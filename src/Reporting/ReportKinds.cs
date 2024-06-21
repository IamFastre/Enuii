namespace Enuii.Reports;

public enum ErrorKind
{
    UnknownError,
    InternalError,
    SyntaxError,
    TypeError,
    SymbolError,
    MathError,
}

public enum WarningKind
{
    UnknownWarning,
    TypeWarning,
}
