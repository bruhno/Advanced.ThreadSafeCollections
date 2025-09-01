using System.Collections.Concurrent;

var bag = new ConcurrentBag<int>();

var count = 0;

var tasks = Enumerable.Range(0, 3).Select(i => Task.Run(() =>
{

    for (var j = 0; j < 3; j++)
    {
        var item = i * 10 + j + 1;

        bag.Add(item);
        Console.WriteLine($"{item} produced by {Environment.CurrentManagedThreadId}");

        if (bag.TryTake(out item))
        {
            Console.WriteLine($"{item} consumed by {Environment.CurrentManagedThreadId}");
            Interlocked.Increment(ref count);
        }
    }

}))
.ToArray();

Task.WaitAll(tasks);

Console.WriteLine($"Consumed count: {count}");