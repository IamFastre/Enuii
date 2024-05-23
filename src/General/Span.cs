
namespace Enuii.General.Positioning;

public class Span
{
    public Position Start { get; private set; }
    public Position End   { get; private set; }

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

    public Span SetEnd(Position position)
    {
        End = position;
        return this;
    }

    public override string ToString()
        => Start.Index == End.Index ? $"{Start}" : $"{Start} => {End}";
}