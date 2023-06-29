using System.Collections.Concurrent;
using System.Threading.Tasks.Tests.Fixtures;
using Xunit;

namespace System.Threading.Tasks.Tests;

public class ConcurrentAsyncEnumerableTests
{
    [Fact]
    public async Task EachItemIsProcessedOnce()
    {
        var values = Enumerable.Range(1, 100).ToArray();
        var used = new ConcurrentBag<int>();

        async ValueTask Action(int i, CancellationToken token)
        {
            await Task.Delay(10);
            used.Add(i);
        }

        await Concurrent.ForEachAsync(AsyncEnumerableFixture.Wrap(values), Action);

        Assert.Equal(100, used.Count);
        Assert.Equal(values, used.OrderBy(x => x));
    }

    [Fact]
    public async Task MaxParallelismIsObeyed()
    {
        const int maxParallelism = 10;

        var values = Enumerable.Range(1, 100).ToArray();

        var maxTasks = 0;
        var concurrentCount = 0;

        async ValueTask Action(int i, CancellationToken token)
        {
            Interlocked.Increment(ref concurrentCount);

            if (concurrentCount > maxTasks)
            {
                maxTasks = concurrentCount;
            }

            await Task.Delay(200);

            Assert.True(concurrentCount <= maxParallelism);

            Interlocked.Decrement(ref concurrentCount);
        }

        await Concurrent.ForEachAsync(AsyncEnumerableFixture.Wrap(values), maxParallelism, Action);

        Assert.Equal(maxParallelism, maxTasks);
    }

    [Fact]
    public async Task CanCancelExecution()
    {
        var values = Enumerable.Range(1, 100);
        var cancellation = new CancellationTokenSource();
        var used = new ConcurrentBag<int>();

        async ValueTask Action(int i, CancellationToken token)
        {
            await Task.Delay(200);

            used.Add(i);
        }

        var task = Concurrent.ForEachAsync(AsyncEnumerableFixture.Wrap(values), cancellation.Token, Action);
        cancellation.CancelAfter(500);

        await task;

        Assert.True(used.Count < 100);
        Assert.True(used.Count > 0);
    }
}
