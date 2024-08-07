using Enuii.Symbols.Types;

namespace Enuii.Symbols.Names;

public sealed class ClassSymbol(string name, IEnumerable<ParameterSymbol> parameters, TypeSymbol classType, bool isConst = false)
    : NameSymbol(name, TypeSymbol.Class([classType, ..parameters.Select(p => p.HasDefaultValue ? p.Type.Nullify() : p.Type)]), isConst)
{
    public ParameterSymbol[] Parameters { get; } = [..parameters];
    public TypeSymbol        ClassType  { get; } = classType;
}
