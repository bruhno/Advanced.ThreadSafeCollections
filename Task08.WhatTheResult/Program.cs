using System.Collections.Concurrent;

using static System.Console;

const int PARTICIPANTS = 4;

var collection = new ConcurrentDictionary<object, int>();

using var barrier = new Barrier(PARTICIPANTS);

var addCallCount = 0;
var updateCallCount = 0;

object key = new();

var threads = Enumerable.Range(0, PARTICIPANTS)
    .Select(
        _ =>
        {
            var thread = new Thread(addOrUpdate);
            thread.Start();
            return thread;
        })
    .ToList();

threads.ForEach(thread => thread.Join());

WriteLine(addCallCount);
WriteLine(updateCallCount);

int add(object _)
{
    Thread.Sleep(
        TimeSpan.FromSeconds(1));

    Interlocked.Increment(ref addCallCount);

    return 1;
}

int update(object _, int oldValue)
{
    Thread.Sleep(
        TimeSpan.FromSeconds(1));

    Interlocked.Increment(ref updateCallCount);

    return oldValue + 1;
}

void addOrUpdate()
{
    barrier.SignalAndWait();
    collection.AddOrUpdate(key, add, update);
}