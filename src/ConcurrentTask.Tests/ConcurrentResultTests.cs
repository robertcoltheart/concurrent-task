using Xunit;

namespace System.Threading.Tasks.Tests;

public class ConcurrentResultTests
{
    [Fact]
    public void PropertiesSet()
    {
        var input = new object();
        var result = new object();
        var exception = new Exception();

        var concurrentResult = new ConcurrentResult<object, object>(input, result, exception);

        Assert.Same(input, concurrentResult.Input);
        Assert.Same(result, concurrentResult.Result);
        Assert.Same(exception, concurrentResult.Exception);
    }

    [Fact]
    public void IsAlwaysCompleted()
    {
        var result = new ConcurrentResult<object>(null);

        Assert.True(result.IsCompleted);
    }

    [Fact]
    public void IsFaultedIfExceptionSet()
    {
        var result = new ConcurrentResult<object>(null, new Exception());

        Assert.True(result.IsFaulted);
        Assert.False(result.IsCompletedSuccessfully);
    }

    [Fact]
    public void IsNotFaultedIfNoExceptionSet()
    {
        var result = new ConcurrentResult<object>(null);

        Assert.False(result.IsFaulted);
        Assert.True(result.IsCompletedSuccessfully);
    }
}
