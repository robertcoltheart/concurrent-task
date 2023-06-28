namespace System.Threading.Tasks;

public static partial class Concurrent
{
    public static Task ForEachAsync<T>(
        IAsyncEnumerable<T> source,
        Func<T, CancellationToken, ValueTask> action)
    {
        return ForEachAsync(source, Environment.ProcessorCount, CancellationToken.None, action);
    }

    public static Task ForEachAsync<T>(
        IAsyncEnumerable<T> source,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask> action)
    {
        return ForEachAsync(source, Environment.ProcessorCount, cancellationToken, action);
    }

    public static Task ForEachAsync<T>(
        IAsyncEnumerable<T> source,
        int maxParallelism,
        Func<T, CancellationToken, ValueTask> action)
    {
        return ForEachAsync(source, maxParallelism, CancellationToken.None, action);
    }

    public static async Task ForEachAsync<T>(
        IAsyncEnumerable<T> source,
        int maxParallelism,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask> action)
    {
        var queue = new Queue<ValueTask>();

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

        foreach (var task in queue)
        {
            await task.ConfigureAwait(false);
        }
    }
}
