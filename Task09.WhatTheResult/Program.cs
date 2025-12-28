using System.Collections.Concurrent;
var d = new ConcurrentDictionary<int, string>();
int i = 1;
d.TryAdd(i, i.ToString());
i++;
foreach (var kv in d)
{
    d.TryAdd(i, i.ToString());
    i++;
    Console.WriteLine(kv.Key);
}

Console.WriteLine("Final:"+i);