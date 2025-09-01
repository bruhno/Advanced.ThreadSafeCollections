using System.Collections.Concurrent;

var N = 100;

var current = 0;

var queue = new ConcurrentQueue<int>();

Enumerable.Range(1, 9).Select(Producer).ToList();

var tasks = Enumerable.Range(1, 2).Select(Consumer).ToList();

Task.WaitAll(tasks);

Task Consumer(int worker)
{
    while (true)
    {
        var item = -1;

        SpinWait.SpinUntil(() => queue.TryDequeue(out item));
        
        Console.WriteLine($"{item} is consumed ");
    }        
}

Task Producer(int num) => Task.Run(async () =>
{
    while (true)
    {
        await Task.Delay(100);

        var item = Interlocked.Increment(ref current);

        if (item>N) break;

        queue.Enqueue(item);
    }
});