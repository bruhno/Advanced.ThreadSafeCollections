using System.Collections.Concurrent;

var N = 100;

var current = 0;

var stack = new ConcurrentStack<int>();

Task.WaitAll(Enumerable.Range(1, 9).Select(Producer));

Task.WaitAll(Enumerable.Range(1, 2).Select(Consumer));

Task Consumer(int worker)
{
    while (true)
    {
        var item = -1;

        SpinWait.SpinUntil(() => stack.TryPop(out item));

        Console.WriteLine($"{item} is consumed ");
    }
}

Task Producer(int num) => Task.Run(async () =>
{
    while (true)
    {
        await Task.Delay(100);

        var item = Interlocked.Increment(ref current);

        if (item > N) break;

        stack.Push(item);
    }
});