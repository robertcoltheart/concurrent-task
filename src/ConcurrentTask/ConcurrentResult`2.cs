namespace System.Threading.Tasks;

/// <summary>
/// Provides completion status on the execution of a <see cref="Concurrent"/> invocation.
/// </summary>
/// <typeparam name="T">The type of the data in the source</typeparam>
/// <typeparam name="TResult">The type of the data in the result</typeparam>
public readonly struct ConcurrentResult<T, TResult>
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

    /// <summary>
    /// Gets the input data of the task
    /// </summary>
    public T? Input { get; }

    /// <summary>
    /// Gets the result value of the task
    /// </summary>
    public TResult? Result { get; }

    /// <summary>
    /// Gets a value that indicates whether the task has completed
    /// </summary>
    public bool IsCompleted => true;

    /// <summary>
    /// Gets a value that indicates whether the task has completed without fault
    /// </summary>
    public bool IsCompletedSuccessfully => IsCompleted && !IsFaulted;

    /// <summary>
    /// Gets a value that indicates whether the task has completed with an exception
    /// </summary>
    public bool IsFaulted => Exception != null;

    /// <summary>
    /// Gets the exception that caused the task to end prematurely. If the task completed successfully, this will return <see langword="null" />.
    /// </summary>
    public Exception? Exception { get; }
}
