namespace Enuii.Symbols;

public abstract class Symbol(string name)
{
    public string Name { get; } = name;

    public override string ToString()
        => Name;
}
