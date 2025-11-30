using System.Collections.Concurrent;

namespace System.Threading.Tasks.Tests;

public class ConcurrentEnumerableTests
{
    [Test]
    public async Task EachItemIsProcessedOnce()
    {
        var values = Enumerable.Range(1, 100).ToArray();
        var used = new ConcurrentBag<int>();

        async ValueTask Action(int i, CancellationToken token)
        {
            await Task.Delay(10);
            used.Add(i);
        }

        await Concurrent.ForEachAsync(values, Action);

        await Assert.That(used).Count().IsEqualTo(100);
        await Assert.That(values).IsEquivalentTo(used);
    }

    [Test]
    public async Task MaxParallelismIsObeyed()
    {
        const int maxParallelism = 10;

        var values = Enumerable.Range(1, 100);

        var maxTasks = 0;
        var concurrentCount = 0;

        async ValueTask Action(int i, CancellationToken token)
        {
            Interlocked.Increment(ref concurrentCount);

            if (concurrentCount > maxTasks)
            {
                maxTasks = concurrentCount;
            }

            await Task.Delay(10);

            await Assert.That(concurrentCount).IsLessThanOrEqualTo(maxParallelism);

            Interlocked.Decrement(ref concurrentCount);
        }

        await Concurrent.ForEachAsync(values, maxParallelism, Action);

        await Assert.That(maxTasks).IsEqualTo(maxParallelism);
    }

    [Test]
    public async Task CanCancelExecution()
    {
        var values = Enumerable.Range(1, 100);
        var cancellation = new CancellationTokenSource();
        var used = new ConcurrentBag<int>();

        async ValueTask Action(int i, CancellationToken token)
        {
            await Task.Delay(50);

            used.Add(i);
        }

        var task = Concurrent.ForEachAsync(values, cancellation.Token, Action);
        cancellation.CancelAfter(1);

        await task;

        await Assert.That(used).Count().IsBetween(0, 100);
    }
}
