using System.Collections.Concurrent;

var current = 0;

var random = new Random();

var collection = new BlockingCollection<int>(new ConcurrentQueue<int>(), 20);

Enumerable.Range(1, 9).Select(Producer).ToList();

Enumerable.Range(1, 2).Select(Consumer).ToList();

while (current < 100)
{
    await Task.Delay(1000);
}
collection.CompleteAdding();

Console.ReadKey();

async Task Consumer(int worker)
{
    foreach (var value in collection.GetConsumingEnumerable())
    {
        await Task.Delay(50);
        Console.WriteLine($"{worker} consumed {value} [{collection.Count} left]");
    }

    Console.WriteLine($"{worker} consuming done");
}

Task Producer(int num) => Task.Run(async () =>
{
    while (true)
    {
        await Task.Delay(100);

        collection.Add(Interlocked.Increment(ref current));
    }
});


