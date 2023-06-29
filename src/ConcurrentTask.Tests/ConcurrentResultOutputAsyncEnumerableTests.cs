using System.Collections.Concurrent;
using System.Threading.Tasks.Tests.Fixtures;
using Xunit;

namespace System.Threading.Tasks.Tests;

public class ConcurrentResultOutputAsyncEnumerableTests
{
    [Fact]
    public async Task EachItemIsProcessedOnce()
    {
        var values = Enumerable.Range(1, 100).ToArray();
        var used = new ConcurrentBag<ConcurrentResult<int, int>>();

        async ValueTask<int> Action(int i, CancellationToken token)
        {
            await Task.Delay(10);

            return i;
        }

        await foreach (var value in Concurrent.ForEachWithResultAsync(AsyncEnumerableFixture.Wrap(values), Action))
        {
            used.Add(value);
        }

        Assert.Equal(100, used.Count);
        Assert.Equal(values, used.Select(x => x.Input).OrderBy(x => x));
        Assert.Equal(values, used.Select(x => x.Input).OrderBy(x => x));
        Assert.All(used, x => Assert.True(x.IsCompletedSuccessfully));
    }

    [Fact]
    public async Task MaxParallelismIsObeyed()
    {
        const int maxParallelism = 10;

        var values = Enumerable.Range(1, 100).ToArray();

        var maxTasks = 0;
        var concurrentCount = 0;

        async ValueTask<int> Action(int i, CancellationToken token)
        {
            Interlocked.Increment(ref concurrentCount);

            if (concurrentCount > maxTasks)
            {
                maxTasks = concurrentCount;
            }

            await Task.Delay(200);

            Assert.True(concurrentCount <= maxParallelism);

            Interlocked.Decrement(ref concurrentCount);

            return i;
        }

        await foreach (var _ in Concurrent.ForEachWithResultAsync(AsyncEnumerableFixture.Wrap(values), maxParallelism, Action))
        {
        }

        Assert.Equal(maxParallelism, maxTasks);
    }

    [Fact]
    public async Task CanCancelExecution()
    {
        var values = Enumerable.Range(1, 100);
        var cancellation = new CancellationTokenSource();
        var used = new ConcurrentBag<ConcurrentResult<int, int>>();

        async ValueTask<int> Action(int i, CancellationToken token)
        {
            await Task.Delay(200);

            return i;
        }

        cancellation.CancelAfter(500);

        await foreach (var value in Concurrent.ForEachWithResultAsync(AsyncEnumerableFixture.Wrap(values), cancellation.Token, Action))
        {
            used.Add(value);
        }

        Assert.True(used.Count < 100);
        Assert.True(used.Count > 0);
    }

    [Fact]
    public async Task ExceptionsIncludedInResults()
    {
        var values = Enumerable.Range(1, 100);
        var used = new ConcurrentBag<ConcurrentResult<int, int>>();

        async ValueTask<int> Action(int i, CancellationToken token)
        {
            await Task.Delay(10);

            if (i % 10 == 0)
            {
                throw new Exception("Bad Leroy");
            }

            return i;
        }

        await foreach (var value in Concurrent.ForEachWithResultAsync(AsyncEnumerableFixture.Wrap(values), Action))
        {
            used.Add(value);
        }

        Assert.Equal(100, used.Count);
        Assert.Equal(90, used.Count(x => x.IsCompletedSuccessfully));
        Assert.Equal(10, used.Count(x => !x.IsCompletedSuccessfully));
        Assert.NotNull(used.First(x => !x.IsCompletedSuccessfully).Exception);
        Assert.Equal("Bad Leroy", used.First(x => !x.IsCompletedSuccessfully).Exception!.Message);
    }
}
