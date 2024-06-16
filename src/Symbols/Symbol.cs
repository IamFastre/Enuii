namespace Enuii.Symbols;

public abstract class Symbol(string name)
{
    public string Name { get; } = name;

    public override string ToString()
        => Name;

    public override int  GetHashCode()       => Name.GetHashCode();
    public override bool Equals(object? obj) => obj is Symbol sym && Name == sym.Name;
}
