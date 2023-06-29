namespace System.Threading.Tasks;

public class ConcurrentResult<T, TResult>
{
    internal ConcurrentResult(T? input, TResult? result, Exception? exception = null)
    {
        Input = input;
        Result = result;
        Exception = exception;
    }

    internal ConcurrentResult(T? input, Exception? exception = null)
    {
        Input = input;
        Exception = exception;
    }

    public T? Input { get; }

    public TResult? Result { get; }

    public bool IsCompleted => true;

    public bool IsCompletedSuccessfully => IsCompleted && !IsFaulted;

    public bool IsFaulted => Exception != null;

    public Exception? Exception { get; }
}
