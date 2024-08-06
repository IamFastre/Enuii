namespace Enuii.Symbols.Types;

public class TypeProperties
    (
        uint                                   ArgSize     = 0u,
        TypeSymbol?                            ElementType = null,
        TypeSymbol[]?                          Parameters  = null,
        IEnumerable<(TypeSymbol, TypeSymbol)>? Indexing    = null,
        string?                                CustomName  = null,
        Dictionary<string, object>?            Extras      = null
    )
{
    public TypeSymbol?                           ElementType { get; } = ElementType;
    public TypeSymbol[]                          Parameters  { get; } = Parameters ?? new TypeSymbol[ArgSize];
    public string?                               CustomName  { get; } = CustomName;
    public IEnumerable<(TypeSymbol, TypeSymbol)> Indexing    { get; } = Indexing ?? [];
    public Dictionary<string, object>?           Extras      { get; } = Extras;

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
}
