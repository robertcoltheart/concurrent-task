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

public class ConcurrentResult<T, TResult> : ConcurrentResult<T>
{
    internal ConcurrentResult(T? input, TResult? result, Exception? exception = null)
        : base(input, exception)
    {
        Result = result;
    }

    internal ConcurrentResult(T? input, Exception? exception = null)
        : base(input, exception)
    {
    }

    public TResult? Result { get; }
}
