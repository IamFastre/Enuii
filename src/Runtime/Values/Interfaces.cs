using Enuii.Scoping;

namespace Enuii.Runtime.Evaluation;

public interface IEnumerableValue<out E>
    where E : RuntimeValue
{
    public double Length { get; }
    public E ElementAt(int index);
    public bool Contains(RuntimeValue value);
}

public interface ICallable
{
    public Scope Scope { get; }
    public RuntimeValue Call(Evaluator evaluator, RuntimeValue[] arguments);
}
