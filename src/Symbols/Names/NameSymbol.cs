using Enuii.Symbols.Types;

namespace Enuii.Symbols.Names;

public class NameSymbol(string name, TypeSymbol? type = null, bool isConst = false)
    : Symbol(name)
{
    public TypeSymbol Type       { get; } = type ?? TypeSymbol.Any;
    public bool       IsConstant { get; } = isConst;

    public override int  GetHashCode()       => Name.GetHashCode();
    public override bool Equals(object? obj) => obj is NameSymbol n
                                             && Name == n.Name
                                             && Type == n.Type;
}
