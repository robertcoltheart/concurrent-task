using System.Collections.Concurrent;

namespace System.Threading.Tasks.Tests;

public class ConcurrentResultEnumerableTests
{
    [Test]
    public async Task EachItemIsProcessedOnce()
    {
        var values = Enumerable.Range(1, 100).ToArray();
        var used = new ConcurrentBag<ConcurrentResult<int>>();

        async ValueTask Action(int i, CancellationToken token)
        {
            await Task.Delay(10);
        }

        await foreach (var value in Concurrent.ForEachWithResultAsync(values, Action))
        {
            used.Add(value);
        }

        await Assert.That(used).Count().IsEqualTo(100);
        await Assert.That(values).IsEquivalentTo(used.Select(x => x.Input));
        await Assert.That(used).All(x => x.IsCompletedSuccessfully);
    }

    [Test]
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

            await Task.Delay(10);

            await Assert.That(concurrentCount).IsLessThanOrEqualTo(maxParallelism);

            Interlocked.Decrement(ref concurrentCount);
        }

        await foreach (var _ in Concurrent.ForEachWithResultAsync(values, maxParallelism, Action))
        {
        }

        await Assert.That(maxTasks).IsEqualTo(maxParallelism);
    }

    [Test]
    public async Task CanCancelExecution()
    {
        var values = Enumerable.Range(1, 100);
        var cancellation = new CancellationTokenSource();
        var used = new ConcurrentBag<ConcurrentResult<int>>();

        async ValueTask Action(int i, CancellationToken token)
        {
            await Task.Delay(10);
        }

        cancellation.CancelAfter(10);

        await foreach (var value in Concurrent.ForEachWithResultAsync(values, cancellation.Token, Action))
        {
            used.Add(value);
        }

        await Assert.That(used).Count().IsBetween(0, 100);
    }

    [Test]
    public async Task ExceptionsIncludedInResults()
    {
        var values = Enumerable.Range(1, 100);
        var used = new ConcurrentBag<ConcurrentResult<int>>();

        async ValueTask Action(int i, CancellationToken token)
        {
            await Task.Delay(10);

            if (i % 10 == 0)
            {
                throw new Exception("Bad Leroy");
            }
        }

        await foreach (var value in Concurrent.ForEachWithResultAsync(values, Action))
        {
            used.Add(value);
        }

        await Assert.That(used).Count().IsEqualTo(100);
        await Assert.That(used.Where(x => x.IsCompletedSuccessfully)).Count().IsEqualTo(90);
        await Assert.That(used.Where(x => !x.IsCompletedSuccessfully)).Count().IsEqualTo(10);
        await Assert.That(used.First(x => !x.IsCompletedSuccessfully).Exception).IsNotNull().And.HasMessageEqualTo("Bad Leroy");
    }
}
