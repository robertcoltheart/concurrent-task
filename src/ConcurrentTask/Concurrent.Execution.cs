namespace System.Threading.Tasks;

public static partial class Concurrent
{
    private static async ValueTask Execute(ValueTask task, SemaphoreSlim semaphore)
    {
        await task;

        semaphore.Release();
    }

    private static async ValueTask<ConcurrentResult<T>> Execute<T>(
        T input,
        Func<T, CancellationToken, ValueTask> action,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken)
    {
        try
        {
            await action(input, cancellationToken).ConfigureAwait(false);

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
            var output = await action(input, cancellationToken).ConfigureAwait(false);

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
}
