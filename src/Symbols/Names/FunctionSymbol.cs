using Enuii.Symbols.Types;

namespace Enuii.Symbols.Names;

public sealed class FunctionSymbol(string name, IEnumerable<ParameterSymbol> parameters, TypeSymbol returnType, bool isConst = false)
    : NameSymbol(name, TypeSymbol.Function([returnType, ..parameters.Select(p => p.HasDefaultValue ? p.Type.Nullify() : p.Type)]), isConst)
{
    public ParameterSymbol[] Parameters { get; } = [..parameters];
    public TypeSymbol        ReturnType { get; } = returnType;

    public int OptionalArgument => Parameters.Count(e => e.HasDefaultValue);
}
