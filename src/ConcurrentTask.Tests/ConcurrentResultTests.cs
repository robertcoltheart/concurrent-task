
namespace System.Threading.Tasks.Tests;

public class ConcurrentResultTests
{
    [Test]
    public async Task PropertiesSet()
    {
        var input = new object();
        var result = new object();
        var exception = new Exception();

        var concurrentResult = new ConcurrentResult<object, object>(input, result, exception);

        await Assert.That(concurrentResult.Input).IsSameReferenceAs(input);
        await Assert.That(concurrentResult.Result).IsSameReferenceAs(result);
        await Assert.That(concurrentResult.Exception).IsSameReferenceAs(exception);
    }

    [Test]
    public async Task IsAlwaysCompleted()
    {
        var result = new ConcurrentResult<object>(null);

        await Assert.That(result.IsCompleted).IsTrue();
    }

    [Test]
    public async Task IsFaultedIfExceptionSet()
    {
        var result = new ConcurrentResult<object>(null, new Exception());

        await Assert.That(result.IsFaulted).IsTrue();
        await Assert.That(result.IsCompletedSuccessfully).IsFalse();

    }

    [Test]
    public async Task IsNotFaultedIfNoExceptionSet()
    {
        var result = new ConcurrentResult<object>(null);

        await Assert.That(result.IsFaulted).IsFalse();
        await Assert.That(result.IsCompletedSuccessfully).IsTrue();
    }
}
