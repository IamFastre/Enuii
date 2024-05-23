namespace Enuii.Reports;

public class Reporter(IEnumerable<Error>? errors = null)
{
    public List<Error> Errors { get; } = errors?.ToList() ?? [];

    public void Add(Error error)
        => Errors.Add(error);
    
    public Error? Pop()
    {
        var error = Errors.LastOrDefault();
        Errors.RemoveAt(Errors.Count - 1);
        return error;
    }

    public Error Report(ErrorKind kind, string message)
    {
        var error = new Error(kind, message);
        Add(error);
        return error;
    }
}
