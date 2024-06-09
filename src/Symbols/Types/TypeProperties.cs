namespace Enuii.Symbols.Typing;

public class TypeProperties
    (
        uint                                   ArgSize     = 0u,
        TypeSymbol?                            ElementType = null,
        TypeSymbol[]?                          Parameters  = null,
        Func<TypeSymbol, string>?              CustomName  = null,
        IEnumerable<(TypeSymbol, TypeSymbol)>? Indexing    = null
    )
{
    public TypeSymbol?                           ElementType { get; } = ElementType;
    public TypeSymbol[]                          Parameters  { get; } = Parameters ?? new TypeSymbol[ArgSize];
    public Func<TypeSymbol, string>?             CustomName  { get; } = CustomName;
    public IEnumerable<(TypeSymbol, TypeSymbol)> Indexing    { get; } = Indexing ?? [];

    public uint                                  ArgSize     => (uint) Parameters.Length;


    public static readonly TypeProperties Blank  = new();

    public static readonly TypeProperties String = new
    (
        ElementType: TypeSymbol.Char,
        Indexing:
        [
            (TypeSymbol.Integer, TypeSymbol.Char),
            (TypeSymbol.Range, TypeSymbol.String),
        ]
    );

    public static readonly TypeProperties Range = new
    (
        ElementType: TypeSymbol.Number,
        Indexing:
        [
            (TypeSymbol.Number, TypeSymbol.Number),
            (TypeSymbol.Range, TypeSymbol.Range),
        ]
    );

    public static readonly TypeProperties List  = new
    (
        ArgSize: 1,
        ElementType: TypeSymbol.Any,
        CustomName: symbol => symbol.Properties.ElementType!.ToString() + "[]",
        Indexing:
        [
            (TypeSymbol.Integer, TypeSymbol.Unknown),
            (TypeSymbol.Range, TypeSymbol.List),
        ]
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
