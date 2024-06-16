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

    public bool TryDeclare(string variable, NameSymbol name)
    {
        if (Names.ContainsKey(variable))
            return false;

        Names.Add(variable, name);
        return true;
    }

    public bool TryAssign(string variable, NameSymbol name)
    {
        if (Names.ContainsKey(variable))
        {
            Names[variable] = name;
            return true;
        }

        if (Parent is not null)
            return Parent.TryAssign(variable, name);

        return false;
    }

    public bool TryGet(string variable, out NameSymbol name)
    {
        if (Names.TryGetValue(variable, out name!))
            return true;

        if (Parent is not null)
            return Parent.TryGet(variable, out name);

        name = null!;
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
