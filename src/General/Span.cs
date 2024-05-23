
namespace Enuii.General.Positioning;

public class Span
{
    public Position Start  { get; private set; }
    public Position End    { get; private set; }
    public bool     IsSingle => Start.Index == End.Index;

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
        => IsSingle ? $"{Start}" : $"{Start} => {End}";
}