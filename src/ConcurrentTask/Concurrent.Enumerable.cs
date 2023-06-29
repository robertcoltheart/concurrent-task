namespace System.Threading.Tasks;

public static partial class Concurrent
{
    /// <summary>
    /// Executes a <c>for-each</c> operation on an <see cref="IEnumerable{T}" /> in which tasks run concurrently.
    /// </summary>
    /// <typeparam name="T">The type of the data in the source</typeparam>
    /// <param name="source">An enumerable data source</param>
    /// <param name="action">An asynchronous delegate that is invoked once per element in the data source</param>
    /// <returns>A task that represents the entire <c>for-each</c> operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="action"/> is <see langword="null" /></exception>
    public static Task ForEachAsync<T>(
        IEnumerable<T> source,
        Func<T, CancellationToken, ValueTask> action)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        return ForEachAsync(source, Environment.ProcessorCount, CancellationToken.None, action);
    }

    /// <summary>
    /// Executes a <c>for-each</c> operation on an <see cref="IEnumerable{T}" /> in which tasks run concurrently.
    /// </summary>
    /// <typeparam name="T">The type of the data in the source</typeparam>
    /// <param name="source">An enumerable data source</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <param name="action">An asynchronous delegate that is invoked once per element in the data source</param>
    /// <returns>A task that represents the entire <c>for-each</c> operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="action"/> is <see langword="null" /></exception>
    public static Task ForEachAsync<T>(
        IEnumerable<T> source,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask> action)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        return ForEachAsync(source, Environment.ProcessorCount, cancellationToken, action);
    }

    /// <summary>
    /// Executes a <c>for-each</c> operation on an <see cref="IEnumerable{T}" /> in which tasks run concurrently.
    /// </summary>
    /// <typeparam name="T">The type of the data in the source</typeparam>
    /// <param name="source">An enumerable data source</param>
    /// <param name="maxParallelism">The maximum number of concurrent tasks</param>
    /// <param name="action">An asynchronous delegate that is invoked once per element in the data source</param>
    /// <returns>A task that represents the entire <c>for-each</c> operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="action"/> is <see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxParallelism"/> is less than or equal to 0</exception>
    public static Task ForEachAsync<T>(
        IEnumerable<T> source,
        int maxParallelism,
        Func<T, CancellationToken, ValueTask> action)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (maxParallelism <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxParallelism));
        }

        return ForEachAsync(source, maxParallelism, CancellationToken.None, action);
    }

    /// <summary>
    /// Executes a <c>for-each</c> operation on an <see cref="IEnumerable{T}" /> in which tasks run concurrently.
    /// </summary>
    /// <typeparam name="T">The type of the data in the source</typeparam>
    /// <param name="source">An enumerable data source</param>
    /// <param name="maxParallelism">The maximum number of concurrent tasks</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <param name="action">An asynchronous delegate that is invoked once per element in the data source</param>
    /// <returns>A task that represents the entire <c>for-each</c> operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="action"/> is <see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxParallelism"/> is less than or equal to 0</exception>
    public static async Task ForEachAsync<T>(
        IEnumerable<T> source,
        int maxParallelism,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask> action)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (maxParallelism <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxParallelism));
        }

        var queue = new Queue<ValueTask>();

        using var semaphore = new SemaphoreSlim(maxParallelism, maxParallelism);

        foreach (var element in source)
        {
            try
            {
                await semaphore.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var task = action(element, cancellationToken);

            queue.Enqueue(Execute(task, semaphore));

            while (queue.TryPeek(out var queuedTask) && queuedTask.IsCompleted)
            {
                await queue.Dequeue();
            }
        }

        foreach (var task in queue)
        {
            await task.ConfigureAwait(false);
        }
    }
}
