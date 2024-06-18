namespace Enuii.Symbols.Types;

public class TypeProperties
    (
        uint                                   ArgSize     = 0u,
        TypeSymbol?                            ElementType = null,
        TypeSymbol[]?                          Parameters  = null,
        IEnumerable<(TypeSymbol, TypeSymbol)>? Indexing    = null,
        Func<TypeSymbol, string>?              CustomName  = null
    )
{
    public TypeSymbol?                           ElementType { get; } = ElementType;
    public TypeSymbol[]                          Parameters  { get; } = Parameters ?? new TypeSymbol[ArgSize];
    public Func<TypeSymbol, string>?             CustomName  { get; } = CustomName;
    public IEnumerable<(TypeSymbol, TypeSymbol)> Indexing    { get; } = Indexing ?? [];

    public uint                                  ArgSize     => (uint) Parameters.Length;


    public static TypeProperties Blank  => new();

    public static TypeProperties String => new
    (
        ElementType: TypeSymbol.Char,
        Indexing:
        [
            (TypeSymbol.Integer, TypeSymbol.Char),
            (TypeSymbol.Range, TypeSymbol.String),
        ]
    );

    public static TypeProperties Range => new
    (
        ElementType: TypeSymbol.Number,
        Indexing:
        [
            (TypeSymbol.Number, TypeSymbol.Number),
            (TypeSymbol.Range, TypeSymbol.Range),
        ]
    );

    public static TypeProperties Function  => new
    (
        ArgSize: 256,
        CustomName: symbol => $"({string.Join(", ", symbol.Properties.Parameters[1..].Select(e => e.ToString()))}) -> {symbol.Properties.Parameters[0]}"
    );

    public static TypeProperties TypedList(TypeSymbol self, TypeSymbol element) => new
    (
        ElementType: element,
        Parameters: [element],
        CustomName: symbol => element.ToString() + "[]",
        Indexing:
        [
            (TypeSymbol.Integer, element),
            (TypeSymbol.Range, self),
        ]
    );
}
