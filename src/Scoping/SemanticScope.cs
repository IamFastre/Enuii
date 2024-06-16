using Enuii.Symbols;
using Enuii.Symbols.Names;

namespace Enuii.Scoping;

public class SemanticScope(SemanticScope? parent = null)
{
    public SemanticScope? Parent { get; } = parent;
    public Dictionary<string, NameSymbol> Names { get; private set; } = parent is null ? DeclareBuiltins() : [];

    public NameSymbol this[string variable] => Names[variable];

    public static Dictionary<string, NameSymbol> DeclareBuiltins()
    {
        var dict = new Dictionary<string, NameSymbol>();
        foreach (var e in Builtins.GetBuiltinSemantics())
            dict.Add(e.Name, e);
        return dict;
    }

    public bool TryDeclare(string variable, NameSymbol value)
    {
        if (Names.ContainsKey(variable))
            return false;

        Names.Add(variable, value);
        return true;
    }

    public bool TryAssign(string variable, NameSymbol value)
    {
        if (Names.ContainsKey(variable))
        {
            Names[variable] = value;
            return true;
        }

        if (Parent is not null)
            return Parent.TryAssign(variable, value);

        return false;
    }

    public bool TryGet(string variable, out NameSymbol value)
    {
        if (Names.TryGetValue(variable, out value!))
            return true;

        if (Parent is not null)
            return Parent.TryGet(variable, out value);

        value = null!;
        return false;
    }

    public bool Contains(string variable)
        => Names.ContainsKey(variable);

    public void Flush()
    {
        if (Parent is null)
            Names = DeclareBuiltins();
        else
            Names.Clear();
    }
}
