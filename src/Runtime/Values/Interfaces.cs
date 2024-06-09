namespace Enuii.Runtime.Evaluation;

public interface IEnumerableValue<E>
    where E : RuntimeValue
{
    public double Length { get; }
    public E ElementAt(int index);
    public bool Contains(E value);
}
