namespace Enuii.Runtime.Evaluation;

public interface IEnumerableValue<out E>
    where E : RuntimeValue
{
    public double Length { get; }
    public E ElementAt(int index);
    public bool Contains(RuntimeValue value);
}
