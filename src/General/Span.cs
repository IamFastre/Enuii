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

    // Method to set end and return self for various uses
    public Span SetEnd(Position position)
    {
        End = position;
        return this;
    }
    
    public Span SetEnd(Span span)
    {
        End = span.End;
        return this;
    }

    // Turn two positions into a span
    public Span To(Span end)
        => new(Start, end.End);

    public Span Copy()
        => new(Start, End);

    public override string ToString()
        => IsShort ? $"{Start}" : $"{Start} -> {End}";
}
