using System.Collections.Concurrent;
using Xunit;

namespace System.Threading.Tasks.Tests;

public class ConcurrentTests
{
    [Fact]
    public async Task EachItemIsProcessedOnce()
    {
        var values = Enumerable.Range(1, 100).ToArray();
        var used = new ConcurrentBag<int>();

        async ValueTask Action(int i, CancellationToken token)
        {
            await Task.Delay(10, token);
            used.Add(i);
        }

        await Concurrent.ForEachAsync(values, Action);

        Assert.Equal(100, used.Count);
        Assert.Equal(values, used.OrderBy(x => x));
    }

    [Fact]
    public async Task MaxParallelismIsObeyed()
    {
        const int MaxParallelism = 10;

        var values = Enumerable.Range(1, 100);

        var maxThreads = 0;
        var concurrentLock = 0;

        async ValueTask Action(int i, CancellationToken token)
        {
            Interlocked.Increment(ref concurrentLock);

            if (concurrentLock > maxThreads)
            {
                maxThreads = concurrentLock;
            }

            await Task.Delay(10, token);

            Assert.True(concurrentLock <= MaxParallelism);

            Interlocked.Decrement(ref concurrentLock);
        }

        await Concurrent.ForEachAsync(values, MaxParallelism, Action);

        Assert.Equal(MaxParallelism, maxThreads);
    }
}
