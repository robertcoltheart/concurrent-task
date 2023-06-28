using System.Runtime.CompilerServices;

namespace System.Threading.Tasks;

public static class Concurrent
{
    #region ForEachAsync (IEnumerable)

    public static Task ForEachAsync<T>(IEnumerable<T> source, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> action)
    {
        return ForEachAsync(source, Environment.ProcessorCount, action, cancellationToken);
    }

    public static Task ForEachAsync<T>(IEnumerable<T> source, Func<T, CancellationToken, ValueTask> action)
    {
        return ForEachAsync(source, Environment.ProcessorCount, action, CancellationToken.None);
    }

    public static Task ForEachAsync<T>(IEnumerable<T> source, int maxParallelism, Func<T, CancellationToken, ValueTask> action)
    {
        return ForEachAsync(source, maxParallelism, action, CancellationToken.None);
    }

    private static async Task ForEachAsync<T>(IEnumerable<T> source, int maxParallelism, Func<T, CancellationToken, ValueTask> action, CancellationToken cancellationToken)
    {
        var queue = new Queue<Task>();

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

        await Task.WhenAll(queue);
    }

    #endregion

    #region ForEachAsync (IAsyncEnumerable)

    public static Task ForEachAsync<T>(IAsyncEnumerable<T> source, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> action)
    {
        return ForEachAsync(source, Environment.ProcessorCount, action, cancellationToken);
    }

    public static Task ForEachAsync<T>(IAsyncEnumerable<T> source, Func<T, CancellationToken, ValueTask> action)
    {
        return ForEachAsync(source, Environment.ProcessorCount, action, CancellationToken.None);
    }

    public static Task ForEachAsync<T>(IAsyncEnumerable<T> source, int maxParallelism, Func<T, CancellationToken, ValueTask> action)
    {
        return ForEachAsync(source, maxParallelism, action, CancellationToken.None);
    }

    private static async Task ForEachAsync<T>(IAsyncEnumerable<T> source, int maxParallelism, Func<T, CancellationToken, ValueTask> action, CancellationToken cancellationToken)
    {
        var queue = new Queue<Task>();

        using var semaphore = new SemaphoreSlim(maxParallelism, maxParallelism);

        await foreach (var element in source.WithCancellation(cancellationToken))
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

        await Task.WhenAll(queue);
    }

    #endregion

    #region ForEachWithResult<T> (IEnumerable)

    public static IAsyncEnumerable<ConcurrentResult<T>> ForEachWithResultAsync<T>(IEnumerable<T> source, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> action)
    {
        return ForEachWithResultAsync(source, Environment.ProcessorCount, action, cancellationToken);
    }

    public static IAsyncEnumerable<ConcurrentResult<T>> ForEachWithResultAsync<T>(IEnumerable<T> source, Func<T, CancellationToken, ValueTask> action)
    {
        return ForEachWithResultAsync(source, Environment.ProcessorCount, action, CancellationToken.None);
    }

    public static IAsyncEnumerable<ConcurrentResult<T>> ForEachWithResultAsync<T>(IEnumerable<T> source, int maxParallelism, Func<T, CancellationToken, ValueTask> action)
    {
        return ForEachWithResultAsync(source, maxParallelism, action, CancellationToken.None);
    }

    #endregion

    #region ForEachWithResult<T> (IAsyncEnumerable)

    public static IAsyncEnumerable<ConcurrentResult<T>> ForEachWithResultAsync<T>(IAsyncEnumerable<T> source, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> action)
    {
        return ForEachWithResultAsync(source, Environment.ProcessorCount, action, cancellationToken);
    }

    public static IAsyncEnumerable<ConcurrentResult<T>> ForEachWithResultAsync<T>(IAsyncEnumerable<T> source, Func<T, CancellationToken, ValueTask> action)
    {
        return ForEachWithResultAsync(source, Environment.ProcessorCount, action, CancellationToken.None);
    }

    public static IAsyncEnumerable<ConcurrentResult<T>> ForEachWithResultAsync<T>(IAsyncEnumerable<T> source, int maxParallelism, Func<T, CancellationToken, ValueTask> action)
    {
        return ForEachWithResultAsync(source, maxParallelism, action, CancellationToken.None);
    }

    #endregion

    #region ForEahWithResult<T, TResult> (IEnumerable)

    public static IAsyncEnumerable<ConcurrentResult<T, TResult>> ForEachWithResultAsync<T, TResult>(IEnumerable<T> source, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask<TResult>> action)
    {
        return ForEachWithResultAsync(source, Environment.ProcessorCount, action, cancellationToken);
    }

    public static IAsyncEnumerable<ConcurrentResult<T, TResult>> ForEachWithResultAsync<T, TResult>(IEnumerable<T> source, Func<T, CancellationToken, ValueTask<TResult>> action)
    {
        return ForEachWithResultAsync(source, Environment.ProcessorCount, action, CancellationToken.None);
    }

    public static IAsyncEnumerable<ConcurrentResult<T, TResult>> ForEachWithResultAsync<T, TResult>(IEnumerable<T> source, int maxParallelism, Func<T, CancellationToken, ValueTask<TResult>> action)
    {
        return ForEachWithResultAsync(source, maxParallelism, action, CancellationToken.None);
    }

    private static async IAsyncEnumerable<TResult> ForEachWithResultAsync<T, TResult>(IEnumerable<T> source, int maxParallelism, Func<T, SemaphoreSlim, CancellationToken, ValueTask<TResult>> action, CancellationToken cancellationToken)
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

    #endregion

    #region ForEahWithResult<T, TOutput> (IAsyncEnumerable)

    public static IAsyncEnumerable<ConcurrentResult<T, TResult>> ForEachWithResultAsync<T, TResult>(
        IAsyncEnumerable<T> source,
        Func<T, CancellationToken, ValueTask<TResult>> action)
    {
        return ForEachAsyncWithResult(source, Environment.ProcessorCount, CancellationToken.None, action);
    }

    public static IAsyncEnumerable<ConcurrentResult<T, TResult>> ForEachWithResultAsync<T, TResult>(
        IAsyncEnumerable<T> source,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask<TResult>> action)
    {
        return ForEachAsyncWithResult(source, Environment.ProcessorCount, CancellationToken.None, action);
    }

    public static IAsyncEnumerable<ConcurrentResult<T, TResult>> ForEachAsyncWithResult<T, TResult>(
        IAsyncEnumerable<T> source,
        int maxParallelism,
        Func<T, CancellationToken, ValueTask<TResult>> action)
    {
        return ForEachAsyncWithResult(source, maxParallelism, CancellationToken.None, action);
    }

    public static IAsyncEnumerable<ConcurrentResult<T, TResult>> ForEachAsyncWithResult<T, TResult>(
        IAsyncEnumerable<T> source,
        int maxParallelism,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask<TResult>> action)
    {
        return ForEachAsyncWithResult(source, maxParallelism, (e, semaphore, token) => Execute(e, action, semaphore, token), cancellationToken);
    }

    private static async IAsyncEnumerable<TResult> ForEachAsyncWithResult<T, TResult>(
        IAsyncEnumerable<T> source,
        int maxParallelism,
        Func<T, SemaphoreSlim, CancellationToken, ValueTask<TResult>> action,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var queue = new Queue<ValueTask<TResult>>();

        using var semaphore = new SemaphoreSlim(maxParallelism);

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

    #endregion

    #region Execution

    private static async ValueTask Execute(ValueTask task, SemaphoreSlim semaphore)
    {
        await task;

        semaphore.Release();
    }

    private static async ValueTask<ConcurrentResult<T>> Execute<T>(T input, Func<T, CancellationToken, ValueTask> action, SemaphoreSlim semaphore, CancellationToken cancellationToken)
    {
        try
        {
            await action(input, cancellationToken);

            return new ConcurrentResult<T>(input);
        }
        catch (Exception ex)
        {
            return new ConcurrentResult<T>(input, ex);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private static async ValueTask<ConcurrentResult<T, TResult>> Execute<T, TResult>(
        T input,
        Func<T, CancellationToken, ValueTask<TResult>> action,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await action(input, cancellationToken);

            return new ConcurrentResult<T, TResult>(input, output);
        }
        catch (Exception e)
        {
            return new ConcurrentResult<T, TResult>(input, e);
        }
        finally
        {
            semaphore.Release();
        }
    }

    #endregion
}
