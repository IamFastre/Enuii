using Enuii.Symbols.Names;
using Enuii.General.Positioning;

namespace Enuii.Semantics;

public class SemanticFunctionStatement(FunctionSymbol function, SemanticStatement body, Span span)
    : SemanticStatement
{
    public FunctionSymbol    Function { get; } = function;
    public SemanticStatement Body     { get; } = body;

    public override SemanticKind Kind { get; } = SemanticKind.FunctionStatement;
    public override Span         Span { get; } = span;
}
