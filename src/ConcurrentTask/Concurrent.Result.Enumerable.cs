using System.Runtime.CompilerServices;

namespace System.Threading.Tasks;

public static partial class Concurrent
{
    /// <summary>
    /// Executes a <c>for-each</c> operation on an <see cref="IEnumerable{T}" /> in which tasks run concurrently and returns the results of each task.
    /// </summary>
    /// <typeparam name="T">The type of the data in the source</typeparam>
    /// <param name="source">An enumerable data source</param>
    /// <param name="action">An asynchronous delegate that is invoked once per element in the data source</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that represents the results of the <c>for-each</c> operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="action"/> is <see langword="null" /></exception>
    public static IAsyncEnumerable<ConcurrentResult<T>> ForEachWithResultAsync<T>(
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

        return ForEachWithResultAsync(source, Environment.ProcessorCount, CancellationToken.None, action);
    }

    /// <summary>
    /// Executes a <c>for-each</c> operation on an <see cref="IEnumerable{T}" /> in which tasks run concurrently and returns the results of each task.
    /// </summary>
    /// <typeparam name="T">The type of the data in the source</typeparam>
    /// <param name="source">An enumerable data source</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <param name="action">An asynchronous delegate that is invoked once per element in the data source</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that represents the results of the <c>for-each</c> operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="action"/> is <see langword="null" /></exception>
    public static IAsyncEnumerable<ConcurrentResult<T>> ForEachWithResultAsync<T>(
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

        return ForEachWithResultAsync(source, Environment.ProcessorCount, cancellationToken, action);
    }

    /// <summary>
    /// Executes a <c>for-each</c> operation on an <see cref="IEnumerable{T}" /> in which tasks run concurrently and returns the results of each task.
    /// </summary>
    /// <typeparam name="T">The type of the data in the source</typeparam>
    /// <param name="source">An enumerable data source</param>
    /// <param name="maxParallelism">The maximum number of concurrent tasks</param>
    /// <param name="action">An asynchronous delegate that is invoked once per element in the data source</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that represents the results of the <c>for-each</c> operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="action"/> is <see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxParallelism"/> is less than or equal to 0</exception>
    public static IAsyncEnumerable<ConcurrentResult<T>> ForEachWithResultAsync<T>(
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

        return ForEachWithResultAsync(source, maxParallelism, CancellationToken.None, action);
    }

    /// <summary>
    /// Executes a <c>for-each</c> operation on an <see cref="IEnumerable{T}" /> in which tasks run concurrently and returns the results of each task.
    /// </summary>
    /// <typeparam name="T">The type of the data in the source</typeparam>
    /// <param name="source">An enumerable data source</param>
    /// <param name="maxParallelism">The maximum number of concurrent tasks</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <param name="action">An asynchronous delegate that is invoked once per element in the data source</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that represents the results of the <c>for-each</c> operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="action"/> is <see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxParallelism"/> is less than or equal to 0</exception>
    public static IAsyncEnumerable<ConcurrentResult<T>> ForEachWithResultAsync<T>(
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

        return ForEachWithResultAsync(source, maxParallelism, (e, semaphore, token) => Execute(e, action, semaphore, token), cancellationToken);
    }

    /// <summary>
    /// Executes a <c>for-each</c> operation on an <see cref="IEnumerable{T}" /> in which tasks run concurrently and returns the results of each task.
    /// </summary>
    /// <typeparam name="T">The type of the data in the source</typeparam>
    /// <typeparam name="TResult">The type of the data to return from each task</typeparam>
    /// <param name="source">An enumerable data source</param>
    /// <param name="action">An asynchronous delegate that is invoked once per element in the data source</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that represents the results of the <c>for-each</c> operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="action"/> is <see langword="null" /></exception>
    public static IAsyncEnumerable<ConcurrentResult<T, TResult>> ForEachWithResultAsync<T, TResult>(
        IEnumerable<T> source,
        Func<T, CancellationToken, ValueTask<TResult>> action)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        return ForEachWithResultAsync(source, Environment.ProcessorCount, CancellationToken.None, action);
    }

    /// <summary>
    /// Executes a <c>for-each</c> operation on an <see cref="IEnumerable{T}" /> in which tasks run concurrently and returns the results of each task.
    /// </summary>
    /// <typeparam name="T">The type of the data in the source</typeparam>
    /// <typeparam name="TResult">The type of the data to return from each task</typeparam>
    /// <param name="source">An enumerable data source</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <param name="action">An asynchronous delegate that is invoked once per element in the data source</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that represents the results of the <c>for-each</c> operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="action"/> is <see langword="null" /></exception>
    public static IAsyncEnumerable<ConcurrentResult<T, TResult>> ForEachWithResultAsync<T, TResult>(
        IEnumerable<T> source,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask<TResult>> action)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        return ForEachWithResultAsync(source, Environment.ProcessorCount, cancellationToken, action);
    }

    /// <summary>
    /// Executes a <c>for-each</c> operation on an <see cref="IEnumerable{T}" /> in which tasks run concurrently and returns the results of each task.
    /// </summary>
    /// <typeparam name="T">The type of the data in the source</typeparam>
    /// <typeparam name="TResult">The type of the data to return from each task</typeparam>
    /// <param name="source">An enumerable data source</param>
    /// <param name="maxParallelism">The maximum number of concurrent tasks</param>
    /// <param name="action">An asynchronous delegate that is invoked once per element in the data source</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that represents the results of the <c>for-each</c> operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="action"/> is <see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxParallelism"/> is less than or equal to 0</exception>
    public static IAsyncEnumerable<ConcurrentResult<T, TResult>> ForEachWithResultAsync<T, TResult>(
        IEnumerable<T> source,
        int maxParallelism,
        Func<T, CancellationToken, ValueTask<TResult>> action)
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

        return ForEachWithResultAsync(source, maxParallelism, CancellationToken.None, action);
    }

    /// <summary>
    /// Executes a <c>for-each</c> operation on an <see cref="IEnumerable{T}" /> in which tasks run concurrently and returns the results of each task.
    /// </summary>
    /// <typeparam name="T">The type of the data in the source</typeparam>
    /// <typeparam name="TResult">The type of the data to return from each task</typeparam>
    /// <param name="source">An enumerable data source</param>
    /// <param name="maxParallelism">The maximum number of concurrent tasks</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <param name="action">An asynchronous delegate that is invoked once per element in the data source</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that represents the results of the <c>for-each</c> operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="action"/> is <see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxParallelism"/> is less than or equal to 0</exception>
    public static IAsyncEnumerable<ConcurrentResult<T, TResult>> ForEachWithResultAsync<T, TResult>(
        IEnumerable<T> source,
        int maxParallelism,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask<TResult>> action)
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

        return ForEachWithResultAsync(source, maxParallelism, (e, semaphore, token) => Execute(e, action, semaphore, token), cancellationToken);
    }

    private static async IAsyncEnumerable<TResult> ForEachWithResultAsync<T, TResult>(
        IEnumerable<T> source,
        int maxParallelism,
        Func<T, SemaphoreSlim, CancellationToken, ValueTask<TResult>> action,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var queue = new Queue<ValueTask<TResult>>();

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

            queue.Enqueue(action(element, semaphore, cancellationToken));

            while (queue.TryPeek(out var task) && task.IsCompleted)
            {
                yield return await queue.Dequeue();
            }
        }

        foreach (var task in queue)
        {
            yield return await task;
        }
    }
}
