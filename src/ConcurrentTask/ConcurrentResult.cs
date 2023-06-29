namespace System.Threading.Tasks;

public class ConcurrentResult<T>
{
    internal ConcurrentResult(T? input, Exception? exception = null)
    {
        Input = input;
        Exception = exception;
    }

    public T? Input { get; }

    public bool IsCompleted => true;

    public bool IsCompletedSuccessfully => IsCompleted && !IsFaulted;

    public bool IsFaulted => Exception != null;

    public Exception? Exception { get; }
}
