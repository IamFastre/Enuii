namespace Enuii.Symbols.Types;

public class TypeProperties
    (
        uint                                   ArgSize     = 0u,
        TypeSymbol?                            elementType = null,
        TypeSymbol[]?                          Parameters  = null,
        Func<TypeSymbol, string>?              CustomName  = null,
        IEnumerable<(TypeSymbol, TypeSymbol)>? Indexing    = null
    )
{
    public TypeSymbol?                           ElementType { get; } = elementType;
    public TypeSymbol[]                          Parameters  { get; } = Parameters ?? new TypeSymbol[ArgSize];
    public Func<TypeSymbol, string>?             CustomName  { get; } = CustomName;
    public IEnumerable<(TypeSymbol, TypeSymbol)> Indexing    { get; } = Indexing ?? [];

    public uint                                  ArgSize     => (uint) Parameters.Length;


    public static TypeProperties Blank  => new();

    public static TypeProperties String => new
    (
        elementType: TypeSymbol.Char,
        Indexing:
        [
            (TypeSymbol.Integer, TypeSymbol.Char),
            (TypeSymbol.Range, TypeSymbol.String),
        ]
    );

    public static TypeProperties Range => new
    (
        elementType: TypeSymbol.Number,
        Indexing:
        [
            (TypeSymbol.Number, TypeSymbol.Number),
            (TypeSymbol.Range, TypeSymbol.Range),
        ]
    );

    public static TypeProperties List  => new
    (
        ArgSize: 1,
        elementType: TypeSymbol.Any,
        CustomName: symbol => symbol.Properties.ElementType!.ToString() + "[]",
        Indexing:
        [
            (TypeSymbol.Integer, TypeSymbol.Unknown),
            (TypeSymbol.Range, TypeSymbol.List),
        ]
    );

    public static TypeProperties TypedList(TypeSymbol self, TypeSymbol element) => new
    (
        elementType: element,
        Parameters: [element],
        CustomName: symbol => element.ToString() + "[]",
        Indexing:
        [
            (TypeSymbol.Integer, element),
            (TypeSymbol.Range, self),
        ]
    );
}
