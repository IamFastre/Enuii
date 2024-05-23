namespace Enuii.Reports;

public class Error(ErrorKind kind = ErrorKind.UnknownError, string message = "Unknown error has occurred")
{
    public ErrorKind Kind    { get; } = kind;
    public string    Message { get; } = message;
}
