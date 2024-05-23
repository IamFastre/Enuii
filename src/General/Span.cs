
namespace Enuii.General.Positioning;

public class Span
{
    public Position Start   { get; private set; }
    public Position End     { get; private set; }
    public bool     IsShort => Start.Index == End.Index;

    public Span(Position? start = null, Position? end = null)
    {
        Start = start ?? new();
        End   = end   ?? start ?? new();
    }

    public Span(int index)
    {
        Start = new(index);
        End   = Start;
    }

    // Method to set end and return self for various uses
    public Span SetEnd(Position position)
    {
        End = position;
        return this;
    }

    public override string ToString()
        => IsShort ? $"{Start}" : $"{Start} => {End}";
}