
namespace Enuii.General.Positioning;

public class Span
{
    public Position Start   { get; private set; }
    public Position End     { get; private set; }
    public bool     IsShort => Start.Index == End.Index;

    public Span()
    {
        Start = new();
        End   = Start;
    }

    public Span(int index)
    {
        Start = new(index);
        End   = Start;
    }

    public Span(Position start, Position? end = null)
    {
        Start = start;
        End   = end ?? start;
    }

    public Span(Span start, Span? end = null)
    {
        Start = start.Start;
        End   = end?.End ?? start.End;
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