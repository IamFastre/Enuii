namespace Enuii.General.Positioning;

public class Position(int ln = 1, int col = 1, int index = 0)
{
    public int Line   { get; } = ln;
    public int Column { get; } = col;
    public int Index  { get; } = index;

    // Turn a position into a short span
    public Span ToSpan(Position? end = null)
        => new(this, end);

    public override string ToString()
        => $"{Line}:{Column}";
}
