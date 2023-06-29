## ConcurrentTask

Why use concurrent tasks?

Typically, `Parallel.ForEach` is used for running parallel tasks that are CPU-bound. There is a chance that using
the same method for I/O-bound operations can lead to port exhaustion, such as for RESTful calls or web methods.

`ConcurrentTask` solves this problem by allowing async tasks to run concurrently, and allowing the consumer
to specify how many tasks to run at the same time.

## Usage
Install the package from NuGet with `dotnet add package ConcurrentTask`.

You can run your tasks concurrently using the below:

```csharp
var maxParallelTasks = 10;
var values = Enumerable.Range(1, 10);

await Concurrent.ForEachAsync(values, maxParallelTasks, async (i, token) =>
{
    await Task.Yield();
});
```

To return results with your tasks, use the below:

```csharp
var values = Enumerable.Range(1, 10);

var results = Concurrent.ForEachWithResultAsync(values, async (i, token) =>
{
    await Task.Yield();

    return i;
});

await foreach (var result in results)
{
    // do something
}
```

By default, `Environment.ProcessorCount` is used to limit the number of concurrent tasks.

## Acknowledgements
Borrowed from the excellent [SafeParallelAsync](https://github.com/NewOrbit/SafeParallelAsync) :heart:

## License
Inject is released under the [MIT License](https://opensource.org/license/mit)
