using System.Collections.Concurrent;
using System.Diagnostics;

var sw = Stopwatch.StartNew();

var dict = new ConcurrentDictionary<int, string>();



// Index 
dict[1] = "abc";


// Update 

// вопрос: в каком порядке запускаются add и update
// ответ: сначала проверяется ключ, если ключа нет вызывается add, иначе update

// вопрос: два таска одновременно пытаются добавить значение - кто победит
// ответ: победит тот - кто первый получит значение для addValueFactory.  
//        при этом проигравший получает шанс на выполнение updateValueFactory - но только после завершения своего addValueFactory

// вопрос: два таска конкурентно запускают updateValueFactory - кто кто победит
// ответ:  значение будет записано от того кто первый завершит updateValueFactory.
//         остальные будут вынужены заново запустить updateValueFactory с новым значением.

Task.WaitAll(
    [
        Task.Run(AddOrUpdateA),
        Task.Run(AddOrUpdateB),
        Task.Run(AddOrUpdateC),
    ]);

void AddOrUpdateA()
{
    Thread.Sleep(500);
    dict.AddOrUpdate(2,
        key =>
        {
            Console.WriteLine($"A: {sw.ElapsedMilliseconds}: addFactory started");
            return $"initial value [A] for {key}";
        },
        (key, oldValue) =>
        {
            Console.WriteLine($"A: {sw.ElapsedMilliseconds}: updateFactory started");
            Console.WriteLine($"A: {sw.ElapsedMilliseconds}: updateFactory got value");
            var newValue = $"updated value [A] for {key}. Old value was: {oldValue}";
            return newValue;
        });
}

void AddOrUpdateB()
{

    dict.AddOrUpdate(2,
        key =>
        {
            Console.WriteLine($"B: {sw.ElapsedMilliseconds}: addFactory started");
            Thread.Sleep(1000);
            return $"initial value [B] for {key}";
        }, 
        (key,oldValue) =>
        {
            Console.WriteLine($"B: {sw.ElapsedMilliseconds}: updateFactory started");
            Thread.Sleep(100);
            Console.WriteLine($"B: {sw.ElapsedMilliseconds}: updateFactory got value");
            var newValue =  $"updated value [B] for {key}. Old value was: {oldValue}";
            return newValue;
        });
}

void AddOrUpdateC()
{

    dict.AddOrUpdate(2,
        key =>
        {
            Console.WriteLine($"B: {sw.ElapsedMilliseconds}: addFactory started");
            Thread.Sleep(1000);
            return $"initial value [C] for {key}";
        },
        (key, oldValue) =>
        {
            Console.WriteLine($"C: {sw.ElapsedMilliseconds}: updateFactory started");
            var newValue = $"updated value [C] for {key}. Old value was: {oldValue}";
            Thread.Sleep(500);
            Console.WriteLine($"C: {sw.ElapsedMilliseconds}: updateFactory got value");
            
            return newValue;
        });
}

string Init(int key)
{
    Console.WriteLine("init started");
    Thread.Sleep(1000);
    return $"initial value for {key}";
}

string Update(int key, string oldValue)
{
    Console.WriteLine("update started");
    return $"updated value for {key}. Old value was: {oldValue}";
}



///////////////////////////////////////////

foreach (var item in dict)
{
    Console.WriteLine($"{item.Key} : {item.Value}");
}






