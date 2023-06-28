using Xunit;

namespace System.Threading.Tasks.Tests;

public class ConcurrentTests
{
    [Fact]
    public async Task CanAwaitConcurrentTasks()
    {
        var items = new[] { "1" };

        //await Parallel.ForEachAsync(items, new ParallelOptions(), async (s, token) => await Task.Yield());
        await Parallel.ForEachAsync(items, CancellationToken.None, async (s, token) => await Task.Yield());
        await Parallel.ForEachAsync(items, async(s, token) => await Task.Yield());

        await Concurrent.ForEachAsync(items, async x => await Task.Yield());
    }
}
