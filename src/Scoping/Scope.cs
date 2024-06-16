using Enuii.Runtime.Evaluation;
using Enuii.Symbols;

namespace Enuii.Scoping;

public class Scope(Scope? parent = null)
{
    public Scope?                           Parent    { get; }              = parent;
    public Dictionary<string, RuntimeValue> Variables { get; private set; } = parent is null ? Builtins.GetBuiltins() : [];

    public RuntimeValue this[string variable] => Variables[variable];

    public bool TryDeclare(string variable, RuntimeValue value)
    {
        if (Variables.ContainsKey(variable))
            return false;

        Variables.Add(variable, value);
        return true;
    }

    public bool TryAssign(string variable, RuntimeValue value)
    {
        if (Variables.ContainsKey(variable))
        {
            Variables[variable] = value;
            return true;
        }

        if (Parent is not null)
            return Parent.TryAssign(variable, value);

        return false;
    }

    public bool TryGet(string variable, out RuntimeValue value)
    {
        if (Variables.TryGetValue(variable, out value!))
            return true;

        if (Parent is not null)
            return Parent.TryGet(variable, out value);

        value = null!;
        return false;
    }

    public bool Contains(string variable)
        => Variables.ContainsKey(variable);

    public void Flush()
    {
        if (Parent is null)
            Variables = Builtins.GetBuiltins();
        else
            Variables.Clear();
    }
}
