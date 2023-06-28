using System.Runtime.CompilerServices;

namespace System.Threading.Tasks;

public static partial class Concurrent
{
    public static IAsyncEnumerable<ConcurrentResult<T>> ForEachWithResultAsync<T>(
        IAsyncEnumerable<T> source,
        Func<T, CancellationToken, ValueTask<T>> action)
    {
        return ForEachWithResultAsync<T>(source, Environment.ProcessorCount, CancellationToken.None, action);
    }

    public static IAsyncEnumerable<ConcurrentResult<T>> ForEachWithResultAsync<T>(
        IAsyncEnumerable<T> source,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask<T>> action)
    {
        return ForEachWithResultAsync<T>(source, Environment.ProcessorCount, CancellationToken.None, action);
    }

    public static IAsyncEnumerable<ConcurrentResult<T>> ForEachWithResultAsync<T>(
        IAsyncEnumerable<T> source,
        int maxParallelism,
        Func<T, CancellationToken, ValueTask<T>> action)
    {
        return ForEachWithResultAsync<T>(source, maxParallelism, CancellationToken.None, action);
    }

    public static IAsyncEnumerable<ConcurrentResult<T>> ForEachWithResultAsync<T>(
        IAsyncEnumerable<T> source,
        int maxParallelism,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask<T>> action)
    {
        return ForEachWithResultAsync(source, maxParallelism, (e, semaphore, token) => Execute(e, action, semaphore, token), cancellationToken);
    }

    public static IAsyncEnumerable<ConcurrentResult<T, TResult>> ForEachWithResultAsync<T, TResult>(
        IAsyncEnumerable<T> source,
        Func<T, CancellationToken, ValueTask<TResult>> action)
    {
        return ForEachWithResultAsync(source, Environment.ProcessorCount, CancellationToken.None, action);
    }

    public static IAsyncEnumerable<ConcurrentResult<T, TResult>> ForEachWithResultAsync<T, TResult>(
        IAsyncEnumerable<T> source,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask<TResult>> action)
    {
        return ForEachWithResultAsync(source, Environment.ProcessorCount, CancellationToken.None, action);
    }

    public static IAsyncEnumerable<ConcurrentResult<T, TResult>> ForEachWithResultAsync<T, TResult>(
        IAsyncEnumerable<T> source,
        int maxParallelism,
        Func<T, CancellationToken, ValueTask<TResult>> action)
    {
        return ForEachWithResultAsync(source, maxParallelism, CancellationToken.None, action);
    }

    public static IAsyncEnumerable<ConcurrentResult<T, TResult>> ForEachWithResultAsync<T, TResult>(
        IAsyncEnumerable<T> source,
        int maxParallelism,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask<TResult>> action)
    {
        return ForEachWithResultAsync(source, maxParallelism, (e, semaphore, token) => Execute(e, action, semaphore, token), cancellationToken);
    }

    private static async IAsyncEnumerable<TResult> ForEachWithResultAsync<T, TResult>(
        IAsyncEnumerable<T> source,
        int maxParallelism,
        Func<T, SemaphoreSlim, CancellationToken, ValueTask<TResult>> action,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var queue = new Queue<ValueTask<TResult>>();

        using var semaphore = new SemaphoreSlim(maxParallelism, maxParallelism);

        await foreach (var input in source.WithCancellation(cancellationToken))
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

            queue.Enqueue(action(input, semaphore, cancellationToken));

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
