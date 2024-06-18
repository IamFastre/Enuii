using Enuii.Runtime.Evaluation;
using Enuii.Symbols.Types;

namespace Enuii.Symbols.Names;

public sealed class ParameterSymbol(string name, TypeSymbol? type = null, RuntimeValue? value = null)
    : NameSymbol(name, type ?? value?.Type, false)
{
    public RuntimeValue? Value { get; } = value;
}
