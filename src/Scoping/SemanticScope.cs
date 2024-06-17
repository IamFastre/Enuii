using Enuii.Symbols;
using Enuii.Symbols.Names;

namespace Enuii.Scoping;

public class SemanticScope(SemanticScope? parent = null)
{
    public SemanticScope?                 Parent { get; }              = parent;
    public Dictionary<string, NameSymbol> Names  { get; private set; } = parent is null ? DeclareBuiltins() : [];

    public NameSymbol this[string variable] => Names[variable];

    public static Dictionary<string, NameSymbol> DeclareBuiltins()
    {
        var dict = new Dictionary<string, NameSymbol>();
        foreach (var e in Builtins.GetBuiltinSemantics())
            dict.Add(e.Name, e);
        return dict;
    }

    public bool TryDeclare(NameSymbol name, bool hasErrors = false)
    {
        if (Names.ContainsKey(name.Name))
            return false;

        if (!hasErrors)
            Names.Add(name.Name, name);

        return true;
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
