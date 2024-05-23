namespace Enuii.General.Positioning;

public class Position(int ln = 1, int col = 1, int index = 0)
{
    public int Line   { get; } = ln;
    public int Column { get; } = col;
    public int Index  { get; } = index;

    public Span ToSpan(Position? end = null)
        => new(this, end);

    public char In(string text)
        => text.Replace("\r", "").Split('\n')[Line - 1][Column - 1];

    public char In(string[] lines)
        => lines[Line - 1][Column - 1];

    public override string ToString()
        => $"{Line}:{Column}";
}
