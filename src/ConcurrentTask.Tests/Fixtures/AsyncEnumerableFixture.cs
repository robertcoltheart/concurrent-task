using System.Runtime.CompilerServices;

namespace System.Threading.Tasks.Tests.Fixtures;

public static class AsyncEnumerableFixture
{
    public static async IAsyncEnumerable<int> Wrap(IEnumerable<int> values, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var i in values)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            await Task.Delay(1);

            yield return i;
        }
    }

    public static async Task Test()
    {
        var results = Concurrent.ForEachWithResultAsync(Enumerable.Range(1, 10), async (i, token) =>
        {
            await Task.Yield();

            return i;
        });

        await foreach (var result in results)
        {
            Console.WriteLine(result.Result);
        }
    }
}
