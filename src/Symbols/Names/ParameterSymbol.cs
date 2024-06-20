using Enuii.Semantics;
using Enuii.Symbols.Types;

namespace Enuii.Symbols.Names;

public sealed class ParameterSymbol(string name, TypeSymbol? type = null, SemanticExpression? value = null)
    : NameSymbol(name, type ?? value?.Type, false)
{
    public SemanticExpression? Value { get; } = value;
    public bool HasDefaultValue => Value is not null;
}
