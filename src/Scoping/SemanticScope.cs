using Enuii.Symbols;
using Enuii.Symbols.Names;

namespace Enuii.Scoping;

public class SemanticScope(SemanticScope? parent = null)
{
    public SemanticScope?                 Parent    { get; }              = parent;
    public Dictionary<string, NameSymbol> Variables { get; private set; } = parent is null ? DeclareBuiltins() : [];

    public NameSymbol this[string variable] => Variables[variable];

    public static Dictionary<string, NameSymbol> DeclareBuiltins()
    {
        var dict = new Dictionary<string, NameSymbol>();
        foreach (var e in Builtins.GetBuiltinSemantics())
            dict.Add(e.Name, e);
        return dict;
    }

    public bool TryDeclare(NameSymbol variable)
    {
        if (Variables.ContainsKey(variable.Name))
            return false;

        Variables.Add(variable.Name, variable);
        return true;
    }

    public bool TryGet(string variable, out NameSymbol name)
    {
        if (Variables.TryGetValue(variable, out name!))
            return true;

        if (Parent is not null)
            return Parent.TryGet(variable, out name);

        name = null!;
        return false;
    }

    public bool TryDelete(string variable)
    {
        if (Variables.Remove(variable))
            return true;

        if (Parent is not null)
            return Parent.TryDelete(variable);

        return false;
    }

    public bool Contains(string variable)
        => Variables.ContainsKey(variable);

    public void Flush()
    {
        if (Parent is null)
            Variables = DeclareBuiltins();
        else
            Variables.Clear();
    }

    internal void Sync(Scope scope)
    {
        if (scope.Variables.Count != Variables.Count)
        {
            foreach (var (name, _) in Variables)
                if (!scope.Contains(name))
                    Variables.Remove(name);
        }
    }
}
